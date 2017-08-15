namespace Spark.Core.Interop
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Helper class for dealing with memory, in particular unmanaged memory.
    /// </summary>
    public static class MemoryHelper
    {
        private static Dictionary<object, GCHandle> _pinnedObjects = new Dictionary<object, GCHandle>();

        /// <summary>
        /// Pins an object in memory, which allows a pointer to it to be returned. While the object remains pinned the runtime
        /// cannot move the object around in memory, which may degrade performance.
        /// </summary>
        /// <param name="obj">Object to pin.</param>
        /// <returns>Pointer to pinned object's memory location.</returns>
        public static IntPtr PinObject(object obj)
        {
            lock (_pinnedObjects)
            {
                GCHandle handle;
                if (!_pinnedObjects.TryGetValue(obj, out handle))
                {
                    handle = GCHandle.Alloc(obj, GCHandleType.Pinned);
                    _pinnedObjects.Add(obj, handle);
                }

                return handle.AddrOfPinnedObject();
            }
        }

        /// <summary>
        /// Unpins an object in memory, allowing it to once again freely be moved around by the runtime.
        /// </summary>
        /// <param name="obj">Object to unpin.</param>
        public static void UnpinObject(object obj)
        {
            lock (_pinnedObjects)
            {
                GCHandle handle;
                if (!_pinnedObjects.TryGetValue(obj, out handle))
                {
                    handle.Free();
                    _pinnedObjects.Remove(obj);
                }
            }
        }

        /// <summary>
        /// Allocates unmanaged memory. This memory should only be freed by this helper.
        /// </summary>
        /// <param name="sizeInBytes">Size to allocate</param>
        /// <param name="alignment">Alignment of the memory, by default aligned along 16-byte boundary.</param>
        /// <returns>Pointer to the allocated unmanaged memory.</returns>
        public static unsafe IntPtr AllocateMemory(int sizeInBytes, int alignment = 16)
        {
            int mask = alignment - 1;
            IntPtr rawPtr = Marshal.AllocHGlobal(sizeInBytes + mask + IntPtr.Size);
            long ptr = (long)((byte*)rawPtr + sizeof(void*) + mask) & ~mask;
            ((IntPtr*)ptr)[-1] = rawPtr;

            return new IntPtr(ptr);
        }

        /// <summary>
        /// Allocates unmanaged memory that is cleared to a certain value. This memory should only be freed by this helper.
        /// </summary>
        /// <param name="sizeInBytes">Size to allocate</param>
        /// <param name="clearValue">Value the memory will be cleared to, by default zero.</param>
        /// <param name="alignment">Alignment of the memory, by default aligned along 16-byte boundary.</param>
        /// <returns>Pointer to the allocated unmanaged memory.</returns>
        public static IntPtr AllocateClearedMemory(int sizeInBytes, byte clearValue = 0, int alignment = 16)
        {
            IntPtr ptr = AllocateMemory(sizeInBytes, alignment);
            ClearMemory(ptr, sizeInBytes, clearValue);
            return ptr;
        }

        /// <summary>
        /// Frees unmanaged memory that was allocated by this helper.
        /// </summary>
        /// <param name="memoryPtr">Pointer to unmanaged memory to free.</param>
        public static unsafe void FreeMemory(IntPtr memoryPtr)
        {
            if (memoryPtr == IntPtr.Zero)
            {
                return;
            }

            Marshal.FreeHGlobal(((IntPtr*)memoryPtr)[-1]);
        }

        /// <summary>
        /// Checks if the memory is aligned to the specified alignment.
        /// </summary>
        /// <param name="memoryPtr">Pointer to the memory</param>
        /// <param name="alignment">Alignment value, by defauly 16-byte</param>
        /// <returns>True if is aligned, false otherwise.</returns>
        public static bool IsMemoryAligned(IntPtr memoryPtr, int alignment = 16)
        {
            int mask = alignment - 1;
            return (memoryPtr.ToInt64() & mask) == 0;
        }
        
        /// <summary>
        /// Copies memory from one location to another
        /// </summary>
        /// <param name="pDest">Destination pointer</param>
        /// <param name="pSrc">Source pointer</param>
        /// <param name="count">Number of bytes</param>
        public static void CopyMemory(IntPtr pDest, IntPtr pSrc, int count)
        {
            NativeMethods.CopyMemory(pDest, pSrc, new IntPtr(count));
        }

        /// <summary>
        /// Fills a block of memory with a single value
        /// </summary>
        /// <param name="pDest">Destination pointer</param>
        /// <param name="length">Number of bytes</param>
        /// <param name="value">Value to set</param>
        public static void ClearMemory(IntPtr pDest, int count, byte value)
        {
            NativeMethods.ClearMemory(pDest, new IntPtr(count), value);
        }

        /// <summary>
        /// Computes the size of the struct type.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <returns>Size of the struct in bytes.</returns>
        public static int SizeOf<T>() where T : struct
        {
            return Marshal.SizeOf<T>();
        }

        /// <summary>
        /// Computes the size of the struct array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="array">Array of structs</param>
        /// <returns>Total size, in bytes, of the array's contents.</returns>
        public static int SizeOf<T>(T[] array) where T : struct
        {
            if (array == null)
            {
                return 0;
            }

            return array.Length * SizeOf<T>();
        }
        
        /// <summary>
        /// Reads a single element from the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <returns>The read value</returns>
        public static T Read<T>(IntPtr pSrc) where T : struct
        {
            return Marshal.PtrToStructure<T>(pSrc);
        }

        /// <summary>
        /// Reads a single element from the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <param name="value">The read value.</param>
        public static void Read<T>(IntPtr pSrc, out T value) where T : struct
        {
            value = Read<T>(pSrc);
        }

        /// <summary>
        /// Reads data from the memory location into the array.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pSrc">Pointer to memory location</param>
        /// <param name="data">Array to store the copied data</param>
        /// <param name="startIndexInArray">Zero-based element index to start writing data to in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Read<T>(IntPtr pSrc, T[] data, int startIndexInArray, int count) where T : struct
        {
            int size = SizeOf<T>();
            for (int i = startIndexInArray; i < startIndexInArray + count; i++)
            {
                Read<T>(pSrc, out data[i]);
                pSrc += size;
            }
        }

        /// <summary>
        /// Writes a single element to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">The value to write</param>
        public static void Write<T>(IntPtr pDest, T data) where T : struct
        {
            Marshal.StructureToPtr<T>(data, pDest, false);
        }

        /// <summary>
        /// Writes a single element to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">The value to write</param>
        public static void Write<T>(IntPtr pDest, ref T data) where T : struct
        {
            Marshal.StructureToPtr<T>(data, pDest, false);
        }

        /// <summary>
        /// Writes data from the array to the memory location.
        /// </summary>
        /// <typeparam name="T">Struct type</typeparam>
        /// <param name="pDest">Pointer to memory location</param>
        /// <param name="data">Array containing data to write</param>
        /// <param name="startIndexInArray">Zero-based element index to start reading data from in the element array.</param>
        /// <param name="count">Number of elements to copy</param>
        public static unsafe void Write<T>(IntPtr pDest, T[] data, int startIndexInArray, int count) where T : struct
        {
            int size = SizeOf<T>();
            for (int i = startIndexInArray; i < startIndexInArray + count; i++)
            {
                Write<T>(pDest, data[i]);
                pDest += size;
            }
        }
    }
}
