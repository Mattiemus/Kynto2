namespace Spark.Direct3D11.Graphics
{
    using D3D = SharpDX.D3DCompiler;

    public sealed class D3DShaderParameter
    {
        internal D3DShaderParameter(D3D.ShaderParameterDescription shaderParameter)
        {
            SemanticName = shaderParameter.SemanticName;
            SemanticIndex = shaderParameter.SemanticIndex;
            StreamIndex = shaderParameter.Stream;
            Register = shaderParameter.Register;

            // For some reason there's some gobbly gook in the usage mask value
            UsageMask = (D3DComponentMaskFlags)(shaderParameter.UsageMask & D3D.RegisterComponentMaskFlags.All);
            ReadWriteMask = (D3DComponentMaskFlags)shaderParameter.ReadWriteMask;
            ComponentType = (D3DComponentType)shaderParameter.ComponentType;
            SystemType = (D3DSystemValueType)shaderParameter.SystemValueType;
        }

        /// <summary>
        /// Gets the per-parameter string that identifies how the data will be used.
        /// </summary>
        public string SemanticName { get; }

        /// <summary>
        /// Gets the semantic index the modifies the semantic name, used to differentiate different parameters that
        /// use the same semantic.
        /// </summary>
        public int SemanticIndex { get; }

        /// <summary>
        /// Gets the index of which stream the geometry shader is using for the signature parameter.
        /// </summary>
        public int StreamIndex { get; }

        /// <summary>
        /// Gets the register that will contain the variable's data.
        /// </summary>
        public int Register { get; }

        /// <summary>
        /// Gets the mask that indicates which components of a register are used.
        /// </summary>
        public D3DComponentMaskFlags UsageMask { get; }

        /// <summary>
        /// Gets the mask that indicates whether a given component is never written (if the signature is an output)
        /// or always read (if the signature is an input).
        /// </summary>
        public D3DComponentMaskFlags ReadWriteMask { get; }

        /// <summary>
        /// Gets the per-component data type that is stored in the register. Each register can store up to four
        /// components of data.
        /// </summary>
        public D3DComponentType ComponentType { get; }

        /// <summary>
        /// Gets the predefined string that determines the functionality of certain pipeline stages.
        /// </summary>
        public D3DSystemValueType SystemType { get; }

        internal bool AreSame(D3DShaderParameter parameter)
        {
            if (!SemanticName.Equals(parameter.SemanticName) ||
                SemanticIndex != parameter.SemanticIndex || 
                StreamIndex != parameter.StreamIndex || 
                Register != parameter.Register || 
                UsageMask != parameter.UsageMask || 
                ReadWriteMask != parameter.ReadWriteMask ||
                ComponentType != parameter.ComponentType ||
                SystemType != parameter.SystemType)
            {
                return false;
            }

            return true;
        }
    }
}
