namespace Spark.Graphics.Materials
{
    using Core;

    /// <summary>
    /// Parameter binding for a <see cref="Material"/>
    /// </summary>
    public sealed class MaterialParameterBinding : INamed
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialParameterBinding"/> class.
        /// </summary>
        /// <param name="name">Name of parameter to bind.</param>
        /// <param name="provider">Computed parameter provider.</param>
        /// <param name="parameter">Effect parameter instance.</param>
        public MaterialParameterBinding(string name, IParameterBindingProvider provider, IEffectParameter parameter)
        {
            Name = name;
            Provider = provider;
            Parameter = parameter;

            Validate();
        }

        /// <summary>
        /// Gets the parameter name that is being bound.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the computed parameter provider.
        /// </summary>
        public IParameterBindingProvider Provider { get; }

        /// <summary>
        /// Gets the effect parameter instance.
        /// </summary>
        public IEffectParameter Parameter { get; }

        /// <summary>
        /// Gets if the binding is valid.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Gets or sets the local state object used to pass information to the provider.
        /// </summary>
        public object LocalState { get; set; }

        /// <summary>
        /// Validates the parameter
        /// </summary>
        private void Validate()
        {
            IsValid = Parameter != null &&
                      Provider != null &&
                      Provider.ValidateParameter(Parameter) && 
                      !string.IsNullOrEmpty(Name) && 
                      Parameter.Name.Equals(Name);
        }
    }
}
