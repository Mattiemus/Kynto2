namespace Spark.Content
{
    using Core;

    /// <summary>
    /// Extension methods for the serialization interfaces (readers/writers/etc)
    /// </summary>
    public static class ReaderWriterExtensions
    {
        /// <summary>
        /// Writes an array of values to the output starting from the current position of the buffer to the last element.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="name">Name of the value</param>
        /// <param name="values">Databuffer containing the values.</param>
        public static void Write<T>(this IPrimitiveWriter writer, string name, IDataBuffer<T> values) where T : struct
        {
            writer.Write(name, values, (values != null) ? values.RemainingLength : 0);
        }

        /// <summary>
        /// Reads an array of elements and returns the values in a data buffer. If the size exceeds 85,000 bytes then
        /// an unmanaged data buffer is returned, otherwise a managed data buffer is returned.
        /// </summary>
        /// <typeparam name="T">Struct type.</typeparam>
        /// <returns>Data buffer containing the array values.</returns>
        public static IDataBuffer<T> ReadArrayData<T>(this IPrimitiveReader reader) where T : struct
        {
            int count = reader.PeekArrayCount();

            IDataBuffer<T> values = null;
            if (count > 0)
            {
                values = new DataBuffer<T>(count);
            }

            reader.ReadArrayData(values);
            if (values != null)
            {
                values.Position = 0;
            }

            return values;
        }
    }
}
