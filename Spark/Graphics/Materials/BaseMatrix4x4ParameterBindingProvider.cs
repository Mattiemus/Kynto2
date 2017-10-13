namespace Spark.Graphics.Materials
{
    using Math;

    /// <summary>
    /// Abstract base class for a <see cref="Matrix4x4"/> parameter binding provider
    /// </summary>
    public abstract class BaseMatrix4x4ParameterBindingProvider : BaseParameterBindingProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMatrix4x4ParameterBindingProvider"/> class.
        /// </summary>
        /// <param name="name">Named of the parameter</param>
        protected BaseMatrix4x4ParameterBindingProvider(string name) 
            : base(name)
        {
        }

        /// <summary>
        /// Validates the specified effect parameter against the provider logic. If the effect parameter is not of the
        /// same format or type as the provider expects then it is not valid.
        /// </summary>
        /// <param name="parameter">Effect parameter instance to validate.</param>
        /// <returns>True if the effect parameter is valid, false otherwise.</returns>
        public override bool ValidateParameter(IEffectParameter parameter)
        {
            if (parameter == null)
            {
                return false;
            }

            // TODO: sort this
            return true;

            return (parameter.ParameterClass == EffectParameterClass.MatrixColumns || parameter.ParameterClass == EffectParameterClass.MatrixRows) && 
                   parameter.RowCount == 4 && 
                   parameter.ColumnCount == 4 && 
                   parameter.SizeInBytes == Matrix4x4.SizeInBytes;
        }
    }
}
