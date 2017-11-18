namespace Spark
{
    using System;
    
    /// <summary>
    /// Extension methods for the data buffer family of interfaces.
    /// </summary>
    public static class DataBufferExtensions
    {
        /// <summary>
        /// Converts the <see cref="IReadOnlyDataBuffer"/> source into a writable data buffer source. Even if the
        /// source buffer is writable, this will make a new writable buffer filled with a copy of the original data.
        /// </summary>
        /// <param name="source">Source data buffer.</param>
        /// <returns>Writable data buffer copy.</returns>
        public static IDataBuffer AsWriteable(this IReadOnlyDataBuffer source)
        {
            if (source == null)
            {
                return null;
            }

            Type elemType = source.ElementType;
            Type newType = typeof(DataBuffer<>).MakeGenericType(elemType);
            IDataBuffer destination = Activator.CreateInstance(newType, source.Length) as IDataBuffer;

            if (destination != null)
            {
                int oldPos = source.Position;

                CopyTo(source, destination);

                source.Position = oldPos;
                destination.Position = 0;
            }

            return destination;
        }

        /// <summary>
        /// Converts the <see cref="IReadOnlyDataBuffer"/> source into a writable data buffer source. Even if the
        /// source buffer is writable, this will make a new writable buffer filled with a copy of the original data.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="source">Source data buffer.</param>
        /// <returns>Writable data buffer copy.</returns>
        public static IDataBuffer<T> AsWriteable<T>(this IReadOnlyDataBuffer<T> source) where T : struct
        {
            if (source == null)
            {
                return null;
            }

            DataBuffer<T> destination = new DataBuffer<T>(source.Length);
            int oldPos = source.Position;
            source.Position = 0;

            CopyTo(source, destination);

            source.Position = oldPos;
            destination.Position = 0;

            return destination;
        }

        /// <summary>
        /// Converts the <see cref="IDataBuffer"/> source into a read-only view of the original data buffer source.
        /// </summary>
        /// <param name="source">Source data buffer.</param>
        /// <returns>Read-only view of the data buffer.</returns>
        public static IReadOnlyDataBuffer AsReadOnly(this IDataBuffer source)
        {
            // TODO: At some point wrap this up
            return source;
        }

        /// <summary>
        /// Converts the <see cref="IDataBuffer"/> source into a read-only view of the original data buffer source.
        /// </summary>
        /// <typeparam name="T">Data type</typeparam>
        /// <param name="source">Source data buffer.</param>
        /// <returns>Read-only view of the data buffer.</returns>
        public static IReadOnlyDataBuffer<T> AsReadOnly<T>(this IDataBuffer<T> source) where T : struct
        {
            // TODO: At some point wrap this up
            return source;
        }

        /// <summary>
        /// Copies data from the source data buffer and writes it to another data buffer. Copying begins at the current position
        /// of the source databuffer and does not reset the position of the destination databuffer after the operation is complete.
        /// </summary>
        /// <param name="source">Source data buffer.</param>
        /// <param name="destination">Destination data buffer.</param>
        public static void CopyTo(this IReadOnlyDataBuffer source, IDataBuffer destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Data buffer is null or empty");
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination), "Data buffer is null or empty");
            }

            int sourceElemSizeInBytes = source.ElementSizeInBytes;
            int destinationElemSizeInBytes = destination.ElementSizeInBytes;
            int numBytesToCopy = source.RemainingLength * sourceElemSizeInBytes;
            int numBytesCanCopy = destination.RemainingLength * destinationElemSizeInBytes;

            if (numBytesToCopy > numBytesCanCopy)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "Write buffer overflow");
            }

            using (MappedDataBuffer sourcePtr = source.Map())
            {
                using (MappedDataBuffer destPtr = destination.Map())
                {
                    MemoryHelper.CopyMemory(destPtr + (destination.Position * destinationElemSizeInBytes), sourcePtr + (source.Position * sourceElemSizeInBytes), numBytesToCopy);
                }
            }

            destination.Position += numBytesToCopy / destinationElemSizeInBytes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="offsetInSourceInBytes"></param>
        /// <param name="offsetInDestinationInBytes"></param>
        /// <param name="numBytesToCopy"></param>
        public static void CopyTo(this IReadOnlyDataBuffer source, IDataBuffer destination, int offsetInSourceInBytes, int offsetInDestinationInBytes, int numBytesToCopy)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source), "Data buffer is null or empty");
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination), "Data buffer is null or empty");
            }

            int sourceElemSizeInBytes = source.ElementSizeInBytes;
            int destinationElemSizeInBytes = destination.ElementSizeInBytes;
            int numBytesCanCopy = destination.RemainingLength * destinationElemSizeInBytes;

            if (numBytesToCopy > numBytesCanCopy)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "Write buffer overflow");
            }

            if (offsetInDestinationInBytes + numBytesToCopy > numBytesCanCopy)
            {
                throw new ArgumentOutOfRangeException(nameof(destination), "Index and count out of range");
            }

            if (offsetInSourceInBytes + numBytesToCopy > numBytesCanCopy)
            {
                throw new ArgumentOutOfRangeException(nameof(source), "Index and count out of range");
            }

            using (MappedDataBuffer sourcePtr = source.Map())
            {
                using (MappedDataBuffer destPtr = destination.Map())
                {
                    MemoryHelper.CopyMemory(destPtr + offsetInDestinationInBytes, sourcePtr + offsetInSourceInBytes, numBytesToCopy);
                }
            }

            source.Position = (offsetInSourceInBytes + numBytesToCopy) / sourceElemSizeInBytes;
            destination.Position = (offsetInDestinationInBytes + numBytesToCopy) / destinationElemSizeInBytes;
        }
    }
}
