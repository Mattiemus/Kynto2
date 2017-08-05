namespace Spark.Content
{
    /// <summary>
    /// Interface that represents a custom primitive data type that can be written to an output and read from
    /// an input.
    /// </summary>
    public interface IPrimitiveValue
    {
        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        void Read(IPrimitiveReader input);

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        void Write(IPrimitiveWriter output);
    }
}
