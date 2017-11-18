namespace Spark.Graphics
{
    /// <summary>
    /// Provider of effect bytecode, used for the <see cref="StandardEffectLibrary"/>.
    /// </summary>
    public interface IEffectByteCodeProvider
    {
        /// <summary>
        /// Gets the folder path where the effect shader files of this provider reside in.
        /// </summary>
        string FolderPath { get; }

        /// <summary>
        /// Gets the effect byte code specified by the name.
        /// </summary>
        /// <param name="name">The name of the effect file to get byte code for. Should not include the .tefx extension.</param>
        /// <returns>Effect byte code, or null if the name does not correspond to an effect file.</returns>
        byte[] GetEffectByteCode(string name);
    }
}
