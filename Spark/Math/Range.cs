namespace Spark.Math
{
    using System;
    using System.Runtime.InteropServices;
    
    [Serializable]
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct Range
    {
        public int Start;
        public int End;

        public Range(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Length => End - Start;

        public void Expand(int count)
        {
            End += count;
        }
    }
}
