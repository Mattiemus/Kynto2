namespace Spark.Content
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines a set of parameters that configure how an asset is loaded.
    /// </summary>
    public class ImporterParameters : ISavable
    {
        private string _subresourceName;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImporterParameters"/> class.
        /// </summary>
        public ImporterParameters()
        {
            ExtendedParameters = new Dictionary<String, String>();
        }

        /// <summary>
        /// Gets an empty set of loading parameters.
        /// </summary>
        public static ImporterParameters None => new ImporterParameters();

        /// <summary>
        /// Gets or sets an optional subresource name. When a resource file is imported it may contain multiple pieces of
        /// content of which only one needs to be imported, as identified by this name.
        /// </summary>
        public String SubresourceName
        {
            get
            {
                return _subresourceName;
            }
            set
            {
                _subresourceName = value;
                if (string.IsNullOrEmpty(_subresourceName))
                {
                    _subresourceName = string.Empty;
                }
            }
        }

        /// <summary>
        /// Gets the extended parameter key-value pair collection.
        /// </summary>
        public Dictionary<string, string> ExtendedParameters { get; }

        /// <summary>
        /// Copies the importer parameters from the specified instance.
        /// </summary>
        /// <param name="parameters">Importer parameter instance to copy from.</param>
        public virtual void Set(ImporterParameters parameters)
        {
            if (parameters == null)
            {
                return;
            }

            _subresourceName = parameters._subresourceName;
            ExtendedParameters.Clear();

            foreach (KeyValuePair<string, string> kv in parameters.ExtendedParameters)
            {
                ExtendedParameters.Add(kv.Key, kv.Value);
            }
        }

        /// <summary>
        /// Resets the importer parameters to default values.
        /// </summary>
        public virtual void Reset()
        {
            _subresourceName = string.Empty;
            ExtendedParameters.Clear();
        }

        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <returns>True if the parameters are valid.</returns>
        public bool Validate()
        {
            string reason;
            return Validate(out reason);
        }

        /// <summary>
        /// Validates parameters.
        /// </summary>
        /// <param name="reason">Reason why the parameters are not valid.</param>
        /// <returns>True if the parameters are valid.</returns>
        public virtual bool Validate(out string reason)
        {
            reason = string.Empty;
            return true;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public virtual void Read(ISavableReader input)
        {
            _subresourceName = input.ReadString();

            int count = input.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = input.ReadString();
                string value = input.ReadString();
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    ExtendedParameters.Add(key, value);
                }
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public virtual void Write(ISavableWriter output)
        {
            output.Write("SubresourceName", _subresourceName);
            output.Write("ExtendedParameterCount", ExtendedParameters.Count);

            foreach (KeyValuePair<string, string> kv in ExtendedParameters)
            {
                if (!string.IsNullOrEmpty(kv.Key) && !string.IsNullOrEmpty(kv.Value))
                {
                    output.Write("Key", kv.Key);
                    output.Write("Value", kv.Value);
                }
            }
        }
    }
}
