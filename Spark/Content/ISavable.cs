namespace Spark.Content
{
    /// <summary>
    /// Interface for objects that can be written to an output and read from an input.
    /// </summary>
    public interface ISavable
    {
        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        void Read(ISavableReader input);

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        void Write(ISavableWriter output);
    }
}
