namespace Spark.Graphics
{
    /// <summary>
    /// Represents an effect constant buffer. A constant buffer contains only value type effect parameters
    /// </summary>
    public interface IEffectConstantBuffer : IEffectPart
    {
        /// <summary>
        /// Gets the buffer's total size in bytes.
        /// </summary>
        int SizeInBytes { get; }

        /// <summary>
        /// Gets a collection of parameters that is contained by this buffer.
        /// </summary>
        EffectParameterCollection Parameters { get; }

        /// <summary>
        /// Gets the contents of the buffer as a struct, typically one that represents the entire buffer. The struct type should be padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <returns>Contents of the buffer returned as a struct</returns>
        T Get<T>() where T : struct;

        /// <summary>
        /// Gets the contents of the buffer as a struct starting at the specified offset, typically one that represents an element in the buffer. The struct type should be 
        /// padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="offsetInBytes">Start offset to read from in the buffer.</param>
        /// <returns>Contents of the buffer returned as a struct</returns>
        T Get<T>(int offsetInBytes) where T : struct;

        /// <summary>
        /// Gets the contents of the buffer as an array of structs. The struct type should be padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="count">Number of values to get.</param>
        /// <returns>Array of values from the buffer.</returns>
        T[] GetRange<T>(int count) where T : struct;

        /// <summary>
        /// Gets the contents of the buffer as an array of structs, starting at the specified offset. The struct type should be padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="offsetInBytes">Start offset to read from in the buffer.</param>
        /// <param name="count">Number of values to read.</param>
        /// <returns>Array of values from the buffer.</returns>
        T[] GetRange<T>(int offsetInBytes, int count) where T : struct;

        /// <summary>
        /// Sets the value to the buffer, typically one that represents the entire buffer. The struct type should be padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="value">Value to set.</param>
        void Set<T>(T value) where T : struct;

        /// <summary>
        /// Sets the value to the buffer, typically one that represents the entire buffer. The struct type should be padded and aligned correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="value">Value to set.</param>
        void Set<T>(ref T value) where T : struct;

        /// <summary>
        /// Sets the value to the buffer starting at the specified offset, typically one that represents an element in the buffer. The struct type should be padded and aligned 
        /// correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="offsetInBytes">Start offset to write to the buffer at.</param>
        /// <param name="value">Value to set.</param>
        void Set<T>(int offsetInBytes, T value) where T : struct;

        /// <summary>
        /// Sets the value to the buffer starting at the specified offset, typically one that represents an element in the buffer. The struct type should be padded and aligned 
        /// correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="offsetInBytes">Start offset to write to the buffer at.</param>
        /// <param name="value">Value to set.</param>
        void Set<T>(int offsetInBytes, ref T value) where T : struct;

        /// <summary>
        /// Sets the array of values to the buffer. The struct type should be padded and aligned 
        /// correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="values">Array of values to write to the buffer.</param>
        void Set<T>(params T[] values) where T : struct;

        /// <summary>
        /// Sets the array of values to the buffer starting at the specified offset. The struct type should be padded and aligned 
        /// correctly.
        /// </summary>
        /// <typeparam name="T">Type of the struct.</typeparam>
        /// <param name="offsetInBytes">Start offset to write to the buffer at.</param>
        /// <param name="values">Array of values to write to the buffer.</param>
        void Set<T>(int offsetInBytes, params T[] values) where T : struct;
    }
}
