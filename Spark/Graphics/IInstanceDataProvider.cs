namespace Spark.Graphics
{
    using Core;
    using Content;
    using Graphics.Renderer;

    /// <summary>
    /// A provider for per-instance data. A concrete example is the world transform of every instance. A single data provider can supply all
    /// the necessary data, or multiple ones can be used (and then mix-and-matched at will, all independent of each other).
    /// </summary>
    public interface IInstanceDataProvider : ISavable
    {
        /// <summary>
        /// Gets the name of the provider. Each provider should have a unique name to be identified by.
        /// </summary>
        string InstanceDataName { get; }

        /// <summary>
        /// Gets the data layout of the data that will be written in the <see cref="SetData"/> method.
        /// </summary>
        VertexElement[] DataLayout { get; }

        /// <summary>
        /// Gets the size of the data in bytes.
        /// </summary>
        int DataSizeInBytes { get; }

        /// <summary>
        /// Queries data from an instance's render properties and sets the data to a mapped data pointer. The data provider is responsible for
        /// incrementing the data pointer.
        /// </summary>
        /// <param name="instanceDefinition">Instance definition that is calling the data provider.</param>
        /// <param name="renderProperties">Render properties of the instance.</param>
        /// <param name="dataBuffer">Mapped data buffer pointer at which to write data to.</param>
        void SetData(InstanceDefinition instanceDefinition, RenderPropertyCollection renderProperties, ref MappedDataBuffer dataBuffer);
    }
}
