namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Content;
    using Utilities;
    using Effects;

    /// <summary>
    /// The standard effect library contains effect byte code that do not require a content repository and can be used to create new effect instances. 
    /// The standard library can be extended to include effect byte code from other sources.
    /// </summary>
    public sealed class StandardEffectLibrary : BaseDisposable
    {
        private readonly object _sync;
        private Dictionary<string, Entry> _effects;
        private Dictionary<string, Entry> _tempEffects;
        private readonly Dictionary<string, IEffectByteCodeProvider> _providers;
        private readonly IRenderSystem _renderSystem;

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardEffectLibrary"/> class.
        /// </summary>
        /// <param name="renderSystem">The render system used to create effect instances.</param>
        public StandardEffectLibrary(IRenderSystem renderSystem)
        {
            if (renderSystem == null)
            {
                throw new ArgumentNullException(nameof(renderSystem));
            }

            _effects = new Dictionary<string, Entry>();
            _providers = new Dictionary<string, IEffectByteCodeProvider>();
            _sync = new object();
            _renderSystem = renderSystem;
        }

        /// <summary>
        /// Gets the rendersystem associated with the effect library.
        /// </summary>
        public IRenderSystem RenderSystem => _renderSystem;

        /// <summary>
        /// Loads the effect byte code provider. The provider merely loads the byte code while
        /// the effect library manages the created effects.
        /// </summary>
        /// <param name="provider">The provider to be loaded.</param>
        /// <returns>True if the provider was added, false if it was already present.</returns>
        public bool LoadProvider(IEffectByteCodeProvider provider)
        {
            if (provider == null)
            {
                return false;
            }

            String folderPath = ContentHelper.NormalizePath(provider.FolderPath);

            lock (_sync)
            {
                if (folderPath == null || _providers.ContainsKey(folderPath))
                {
                    return false;
                }

                _providers.Add(folderPath, provider);
                return true;
            }
        }

        /// <summary>
        /// Queries if the provider is already loaded in the standard effect library.
        /// </summary>
        /// <param name="provider">Provider that may be loaded or not.</param>
        /// <returns>True if the provider was loaded, false if it is not present.</returns>
        public bool IsProviderLoaded(IEffectByteCodeProvider provider)
        {
            if (provider == null)
            {
                return false;
            }

            lock (_sync)
            {
                foreach (KeyValuePair<String, IEffectByteCodeProvider> kv in _providers)
                {
                    if (kv.Value == provider)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Queries if the folder path is registered with a provider in the standard effect library.
        /// </summary>
        /// <param name="folderPath">Folder path</param>
        /// <returns>True if the folder path is registered with a provider, false if a provider is not present.</returns>
        public bool IsProviderLoaded(string folderPath)
        {
            if (folderPath == null)
            {
                return false;
            }

            string normalizedFolderPath = ContentHelper.NormalizePath(folderPath);

            lock (_sync)
            {
                if (!_providers.ContainsKey(normalizedFolderPath))
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Unloads the effect byte code provider using the specified folder path.
        /// </summary>
        /// <param name="folderPath">The folder path of the provider to be unloaded.</param>
        /// <returns>True if the provider was removed, false if it wasn't present.</returns>
        public bool UnloadProvider(string folderPath)
        {
            if (folderPath == null)
            {
                return false;
            }

            folderPath = ContentHelper.NormalizePath(folderPath);

            lock (_sync)
            {
                if (!_providers.TryGetValue(folderPath, out IEffectByteCodeProvider provider))
                {
                    return false;
                }

                _providers.Remove(folderPath);

                if (_tempEffects == null)
                {
                    _tempEffects = new Dictionary<string, Entry>(_effects.Count);
                }

                // Go through cache and dipose of effects and add ones from other providers
                // to the temp dictionary
                foreach (KeyValuePair<string, Entry> kv in _effects)
                {
                    if (kv.Value.Provider == provider)
                    {
                        kv.Value.FirstEffectInstance.Dispose();
                    }
                    else
                    {
                        _tempEffects.Add(kv.Key, kv.Value);
                    }
                }

                _effects.Clear();
                SwapDictionaries();

                // If provider is disposable, make sure we call that too
                if (provider is IDisposable)
                {
                    (provider as IDisposable).Dispose();
                }

                return true;
            }
        }

        /// <summary>
        /// Creates a new standard effect corresponding to the full name (without extension). The folder path in the name corresponds
        /// to the provider the effect byte code resides in.
        /// </summary>
        /// <param name="fullName">The full name, folder path + name of the effect instance to return.</param>
        /// <returns>Effect instance, or null if not found.</returns>
        public Effect CreateEffect(string fullName)
        {
            if (string.IsNullOrEmpty(fullName))
            {
                return null;
            }

            // If there is an extension, this will strip it out
            fullName = Path.ChangeExtension(fullName, null);

            lock (_sync)
            {
                // Get the effect, if not there query for its byte code and create the first instance,
                // otherwise just clone
                if (!_effects.TryGetValue(fullName, out Entry entry))
                {
                    string folderPath = Path.GetDirectoryName(fullName);
                    string name = Path.GetFileName(fullName); // Will be without extension always
                    
                    if (!_providers.TryGetValue(folderPath, out IEffectByteCodeProvider provider))
                    {
                        return null;
                    }

                    byte[] byteCode = provider.GetEffectByteCode(name);

                    // Could not find the byte code
                    if (byteCode == null)
                    {
                        return null;
                    }

                    entry.ByteCode = byteCode;
                    entry.Provider = provider;
                    entry.FirstEffectInstance = new Effect(_renderSystem, EffectData.FromBytes(entry.ByteCode), fullName);
                    entry.FirstEffectInstance.Name = fullName;

                    _effects.Add(fullName, entry);

                    return entry.FirstEffectInstance; // Always return first instance
                }

                return entry.FirstEffectInstance.Clone(); // Always clone subsequent instances
            }
        }

        /// <summary>
        /// Disposes the object instance
        /// </summary>
        /// <param name="isDisposing">True if called from dispose, false if called from the finalizer</param>
        protected override void Dispose(bool isDisposing)
        {
            if (IsDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                // Dispose of all effects
                foreach (KeyValuePair<string, Entry> kv in _effects)
                {
                    kv.Value.FirstEffectInstance.Dispose();
                }

                _effects.Clear();

                // Remove and dispose of all providers
                foreach (KeyValuePair<string, IEffectByteCodeProvider> kv in _providers)
                {
                    if (kv.Value is IDisposable)
                    {
                        (kv.Value as IDisposable).Dispose();
                    }
                }

                _providers.Clear();
            }

            base.Dispose(isDisposing);
        }
        
        /// <summary>
        /// Swaps the effect dictionaries
        /// </summary>
        private void SwapDictionaries()
        {
            Dictionary<string, Entry> swap = _effects;
            _effects = _tempEffects;
            _tempEffects = swap;
        }

        /// <summary>
        /// Effect cache entry
        /// </summary>
        private struct Entry
        {
            public Effect FirstEffectInstance;
            public byte[] ByteCode;
            public IEffectByteCodeProvider Provider;
        }

        /// <summary>
        /// Base implementation for a <see cref="IEffectByteCodeProvider"/>. Effect byte code buffers get preloaded so no locking is necessary.
        /// </summary>
        public abstract class BaseProvider : IEffectByteCodeProvider
        {
            private readonly string m_folderPath;
            private readonly Dictionary<string, byte[]> m_effectByteCodes;

            /// <summary>
            /// Gets the folder path where the effect shader files of this provider reside in. The Engine's default effect shader files are in the topmost folder (e.g.
            /// <see cref="string.Empty" /> would be the value).
            /// </summary>
            public string FolderPath => m_folderPath;

            /// <summary>
            /// Constructs a new instance of the <see cref="BaseProvider"/> class.
            /// </summary>
            /// <param name="folderPath">The folder path that identifies the location of the effect shader files.</param>
            protected BaseProvider(string folderPath)
            {
                if (folderPath == null)
                {
                    throw new ArgumentNullException(nameof(folderPath));
                }

                m_folderPath = folderPath;
                m_effectByteCodes = new Dictionary<string, byte[]>();

                Preload(m_effectByteCodes);
            }

            /// <summary>
            /// Gets the effect byte code specified by the name.
            /// </summary>
            /// <param name="name">The name of the effect file to get byte code for. Should not include the .tefx extension.</param>
            /// <returns>Effect byte code, or null if the name does not correspond to an effect file.</returns>
            public byte[] GetEffectByteCode(string name)
            {
                if (string.IsNullOrEmpty(name))
                {
                    return null;
                }
                
                if (!m_effectByteCodes.TryGetValue(name, out byte[] byteCode))
                {
                    byteCode = null;
                }

                return byteCode;
            }

            /// <summary>
            /// Called to preload all the effect byte code buffers.
            /// </summary>
            /// <param name="effectByteCodes">Cache that will hold the effect byte code buffers.</param>
            protected abstract void Preload(Dictionary<string, byte[]> effectByteCodes);
        }
    }
}
