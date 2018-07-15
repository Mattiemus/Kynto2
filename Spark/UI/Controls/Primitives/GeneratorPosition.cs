namespace Spark.UI.Controls.Primitives
{
    public struct GeneratorPosition
    {
        public GeneratorPosition(int index, int offset)
            : this()
        {
            Index = index;
            Offset = offset;
        }

        public int Index { get; set; }

        public int Offset { get; set; }

        public static bool operator ==(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            return gp1.Index == gp2.Index && gp1.Offset == gp2.Offset;
        }

        public static bool operator !=(GeneratorPosition gp1, GeneratorPosition gp2)
        {
            return !(gp1 == gp2);
        }

        public override bool Equals(object o)
        {
            return o is GeneratorPosition && this == ((GeneratorPosition)o);
        }

        public override int GetHashCode()
        {
            return Index + Offset;
        }

        public override string ToString()
        {
            return string.Format("GeneratorPosition ({0},{1})", Index, Offset);
        }
    }
}
