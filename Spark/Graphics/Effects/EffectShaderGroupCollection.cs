namespace Spark.Graphics
{
    using System.Collections.Generic;
        
    /// <summary>
    /// Represents a collection of shader groups.
    /// </summary>
    public sealed class EffectShaderGroupCollection : ReadOnlyNamedListFast<IEffectShaderGroup>
    {
        /// <summary>
        /// Initialies a new instance of the <see cref="EffectShaderGroupCollection"/> class.
        /// </summary>
        /// <param name="shaderGroups">Effect shader groups.</param>
        public EffectShaderGroupCollection(params IEffectShaderGroup[] shaderGroups) 
            : base(shaderGroups)
        {
        }

        /// <summary>
        /// Initialies a new instance of the <see cref="EffectShaderGroupCollection"/> class.
        /// </summary>
        /// <param name="shaderGroups">Effect shader groups.</param>
        public EffectShaderGroupCollection(IEnumerable<IEffectShaderGroup> shaderGroups) 
            : base(shaderGroups)
        {
        }

        /// <summary>
        /// Initialies a new instance of the <see cref="EffectShaderGroupCollection"/> class.
        /// </summary>
        private EffectShaderGroupCollection()
        {
        }

        /// <summary>
        /// Empty shader group collection.
        /// </summary>
        public static EffectShaderGroupCollection EmptyCollection => new EffectShaderGroupCollection();
    }
}
