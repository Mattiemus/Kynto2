namespace Spark.Direct3D11.Graphics
{
    using System.IO;

    /// <summary>
    /// Interface that specifies how #include files should be located.
    /// </summary>
    public interface IIncludeHandler
    {
        /// <summary>
        /// User implemented method for closing the #include file stream.
        /// </summary>
        /// <param name="stream">Stream that was opened</param>
        void Close(Stream stream);

        /// <summary>
        /// User implemented method for opening and reading the contents of a shader #include file.
        /// </summary>
        /// <param name="includeType">Determines the location of the #include file</param>
        /// <param name="fileName">Name of the #include file</param>
        /// <param name="parentStream">Stream that contains the shader contents that includes the #include file</param>
        Stream Open(IncludeType includeType, string fileName, Stream parentStream);

        /// <summary>
        /// Adds an include directory to search in.
        /// </summary>
        /// <param name="directory">Directory to search in.</param>
        /// <returns>True if successfully added to the search list, false otherwise.</returns>
        bool AddIncludeDirectory(string directory);

        /// <summary>
        /// Removes an include directory to search in.
        /// </summary>
        /// <param name="directory">Directory to remove.</param>
        /// <returns>True if successfully removed from the list, false otherwise.</returns>
        bool RemoveIncludeDirectory(string directory);
    }
}
