namespace Spark.Graphics.Materials
{
    using System;

    /// <summary>
    /// Parameter in a <see cref="Material"/>
    /// </summary>
    public sealed class MaterialParameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialParameter"/> class.
        /// </summary>
        /// <param name="name">Name of parameter to bind.</param>
        /// <param name="parameter">Effect parameter instance.</param>
        public MaterialParameter(string name, IEffectParameter parameter)
        {
            Name = name;
            Parameter = parameter;
            DataType = null;

            Validate();
        }

        /// <summary>
        /// Gets the parameter name that is being bound.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the effect parameter instance.
        /// </summary>
        public IEffectParameter Parameter { get; }

        /// <summary>
        /// Gets if the binding is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets the bound data type. This might be different than the default type of the effect parameter.
        /// </summary>
        public Type DataType { get; internal set; }

        /// <summary>
        /// Validates the parameter
        /// </summary>
        private void Validate()
        {
            IsValid = Parameter != null && !string.IsNullOrEmpty(Name) && Parameter.Name.Equals(Name);

            if (IsValid)
            {
                DataType = Parameter.DefaultNetType;
            }
        }
    }
}
