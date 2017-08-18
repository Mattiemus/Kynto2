namespace Spark.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// Smart activator that can instantiate new types using reflection. A type that is a candidate for instantiation is either
    /// a value type or a (non-abstract) class with a public or non-public parameterless constructor.
    /// </summary>
    public static class SmartActivator
    {
        private static Dictionary<string, Type> _nameToType = new Dictionary<string, Type>();
        private static Dictionary<Type, string> _typeToName = new Dictionary<Type, string>();
        private static Dictionary<Type, CreatorInfo> _typeToCreators = new Dictionary<Type, CreatorInfo>();

        /// <summary>
        /// Gets an assembly qualified full name of the type without culture/version/public key token information. This will return
        /// a string in the format of [type full name], [assembly name]"/>
        /// </summary>
        /// <param name="type">Type to get the qualified name of.</param>
        /// <returns>Qualified name.</returns>
        public static string GetAssemblyQualifiedName(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            string assemblyName = type.Assembly.GetName().Name;

            if (!type.IsGenericType)
            {
                return Assembly.CreateQualifiedName(assemblyName, type.FullName);
            }

            string fullName;
            lock (_typeToName)
            {
                if (_typeToName.TryGetValue(type, out fullName))
                {
                    return fullName;
                }
            }

            Type[] genericArgs = type.GenericTypeArguments;

            StringBuilder name = new StringBuilder();
            name.Append(type.Namespace);
            name.Append(".");

            // Nested types will have a declaring type
            if (type.DeclaringType != null)
            {
                name.Append(type.DeclaringType.Name);
                name.Append("+");
            }

            name.Append(type.Name);

            if (genericArgs != null && genericArgs.Length > 0)
            {
                name.Append("[");

                int count = genericArgs.Length;
                for (int i = 0; i < genericArgs.Length; i++)
                {
                    name.Append("[");

                    string paramName = GetAssemblyQualifiedName(genericArgs[i]);
                    name.Append(paramName);

                    name.Append("]");

                    if (count > 0 && i != count - 1)
                    {
                        name.Append(",");
                    }
                }

                name.Append("]");
            }

            fullName = Assembly.CreateQualifiedName(assemblyName, name.ToString());
            lock (_typeToName)
            {
                _nameToType.Add(fullName, type);
                _typeToName.Add(type, fullName);
            }

            return fullName;
        }

        /// <summary>
        /// Resolves a type from a type name. Unlike <see cref="Type.GetType"/>, this method will fallback to looking at all loaded assemblies
        /// for the type (if an assembly name is not provided). For future queries, the name-type relationship is cached.
        /// </summary>
        /// <param name="typeName">Full name of the type (with namespace), optionally with an assembly name. For performance, always pass an assembly name.</param>
        /// <returns>The resolved type.</returns>
        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            Type type = Type.GetType(typeName);
            if (type != null)
            {
                return type;
            }

            // Typename is incomplete, so try and search for it in all the loaded assemblies
            lock (_typeToName)
            {
                // First see if we cached it already
                if (_nameToType.TryGetValue(typeName, out type))
                {
                    return type;
                }
            }

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for (int i = 0; i < assemblies.Length; i++)
            {
                type = assemblies[i].GetType(typeName);
                if (type != null)
                {
                    break;
                }
            }

            if (type == null)
            {
                throw new ArgumentException($"Cannot resolve type {typeName}");
            }

            // Type name is incomplete, so extract a fully assembly qualified name without versioning/culture/etc
            lock (_typeToName)
            {
                // Future calls should return the type that we found so we don't have to look through all the assemblies...
                _nameToType[typeName] = type;

                // Since this typename was incomplete, parse out its full name and ensure its proper name gets used in the future
                if (!_typeToName.ContainsKey(type))
                {
                    GetAssemblyQualifiedName(type);
                }
            }

            return type;
        }

        /// <summary>
        /// Construct a new instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type to instantiate</typeparam>
        /// <returns>Newly created instance</returns>
        public static T CreateInstance<T>()
        {
            CreateInstance(out T instance);
            return instance;
        }

        /// <summary>
        /// Construct a new instance of the specified type.
        /// </summary>
        /// <typeparam name="T">Type to instantiate</typeparam>
        /// <param name="instance">Newly created instance</param>
        public static void CreateInstance<T>(out T instance)
        {
            instance = (T)CreateInstance(typeof(T));
        }

        /// <summary>
        /// Construct a new instance of the specified type.
        /// </summary>
        /// <param name="type">Type to instantiate</param>
        /// <returns>Newly constructed instance</returns>
        public static object CreateInstance(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            CreatorInfo creatorInfo;
            if (_typeToCreators.TryGetValue(type, out creatorInfo))
            {
                // Handles classes + structs
                if (creatorInfo.HasPublicCtor)
                {
                    return Activator.CreateInstance(type);
                }
                else
                {
                    return creatorInfo.ConstructorInfo.Invoke(null);
                }
            }

            // Check if the type can be instantiated
            if (type.IsAbstract || type.IsInterface)
            {
                throw new InvalidOperationException("Cannot instantiate object from an abstract class or interface");
            }

            creatorInfo.IsStruct = type.IsValueType;

            // If a struct, just add and return default instance
            if (creatorInfo.IsStruct)
            {
                creatorInfo.ConstructorInfo = null;
                creatorInfo.HasPublicCtor = true;

                _typeToCreators.Add(type, creatorInfo);

                return Activator.CreateInstance(type);
            }

            // Otherwise, inspect parameterless constructor - public or non public
            creatorInfo.ConstructorInfo = type.GetConstructor(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);

            //If we didn't find one, we cannot proceed
            if (creatorInfo.ConstructorInfo == null)
            {
                throw new InvalidOperationException("Cannot instantiate object with no parameterless constructor");
            }

            // If its a public ctor, we can use the faster Activator create instance
            creatorInfo.HasPublicCtor = creatorInfo.ConstructorInfo.IsPublic;
            _typeToCreators.Add(type, creatorInfo);
            if (creatorInfo.HasPublicCtor)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return creatorInfo.ConstructorInfo.Invoke(null);
            }
        }

        /// <summary>
        /// Creator info for caching constructor info and other associated information
        /// </summary>
        private struct CreatorInfo
        {
            public bool HasPublicCtor;
            public bool IsStruct;
            public ConstructorInfo ConstructorInfo;
        }
    }
}
