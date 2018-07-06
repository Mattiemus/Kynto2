namespace Spark.Utilities
{
    using System;

    public static class ArrayHelper
    {
        public static void ArrayFill<T>(T[] arrayToFill, T fillValue)
        {
            ArrayFill<T>(arrayToFill, new T[] { fillValue });
        }

        public static void ArrayFill<T>(T[] arrayToFill, T[] fillValue)
        {
            if (fillValue.Length >= arrayToFill.Length)
            {
                throw new ArgumentException("fillValue array length must be smaller than length of arrayToFill", nameof(fillValue));
            }

            Array.Copy(fillValue, arrayToFill, fillValue.Length);
            var arrayToFillHalfLength = arrayToFill.Length / 2;
            for (var i = fillValue.Length; i < arrayToFill.Length; i *= 2)
            {
                var copyLength = i;
                if (i > arrayToFillHalfLength)
                {
                    copyLength = arrayToFill.Length - i;
                }

                Array.Copy(arrayToFill, 0, arrayToFill, i, copyLength);
            }
        }
    }
}
