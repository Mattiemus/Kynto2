namespace Spark.Direct3D11.Graphics
{
    using System.IO;
    using System.Collections.Generic;

    public sealed class DefaultIncludeHandler : IIncludeHandler
    {
        private readonly Dictionary<string, string> _includeDirs;
        private readonly List<string> _tempIncludeDirs;
        private readonly Stack<string> _currDirStack;
        
        public DefaultIncludeHandler() 
            : this(null)
        {
        }

        public DefaultIncludeHandler(params string[] includeDirectories)
        {
            _includeDirs = new Dictionary<string, string>();

            if (includeDirectories != null)
            {
                foreach (string includeDir in includeDirectories)
                {
                    AddIncludeDirectory(includeDir);
                }
            }

            _currDirStack = new Stack<string>();
            _tempIncludeDirs = new List<string>(_includeDirs.Count);
        }

        public IEnumerable<string> IncludeDirectories => _includeDirs.Values;

        public bool AddIncludeDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return false;
            }

            if (!Directory.Exists(directory))
            {
                return false;
            }

            _includeDirs[directory] = directory;
            return true;
        }

        public bool RemoveIncludeDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                return false;
            }

            return _includeDirs.Remove(directory);
        }

        public void Close(Stream stream)
        {
            stream.Close();
            _currDirStack.Pop();
        }

        public Stream Open(IncludeType includeType, string fileName, Stream parentStream)
        {
            string path = fileName;
            if (!Path.IsPathRooted(path))
            {
                // Add the current path on the stack so that gets searched first, most likely include files are grouped spatially near each other
                string currPath = (_currDirStack.Count > 0) ? _currDirStack.Peek() : null;
                if (currPath != null)
                {
                    _tempIncludeDirs.Add(currPath);
                }

                _tempIncludeDirs.AddRange(_includeDirs.Values);

                // Search in each directory
                foreach (string includeDir in _tempIncludeDirs)
                {
                    string testFilePath = Path.Combine(includeDir, fileName);
                    if (File.Exists(testFilePath))
                    {
                        path = testFilePath;
                        break;
                    }
                }
            }

            if (string.IsNullOrEmpty(path) || !File.Exists(path))
            {
                return null;
            }

            _currDirStack.Push(Path.GetDirectoryName(path));
            return new FileStream(path, FileMode.Open, FileAccess.Read);
        }
    }
}
