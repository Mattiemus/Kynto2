namespace Spark.Math
{
    using Core.Interop;

    /// <summary>
    /// 
    /// </summary>
    public struct Ellipse
    {
        /// <summary>
        /// Center of the ellipse.
        /// </summary>
        public Vector3 Center;

        /// <summary>
        /// 0 degree axis.
        /// </summary>
        public Vector3 Axis0;

        /// <summary>
        /// 90 degree axis.
        /// </summary>
        public Vector3 Axis90;

        /// <summary>
        /// Radius along the 0 degree axis.
        /// </summary>
        public float Radius0;

        /// <summary>
        /// Radius along the 90 degree axis.
        /// </summary>
        public float Radius90;

        /// <summary>
        /// Start angle of arc.
        /// </summary>
        public Angle StartAngle;

        /// <summary>
        /// Sweep angle of arc.
        /// </summary>
        public Angle SweepAngle;

        /// <summary>
        /// Initializes a new instance of the <see cref="Ellipse"/> struct.
        /// </summary>
        /// <param name="center">Center of the ellipse</param>
        /// <param name="axis0">0 degree axis</param>
        /// <param name="axis90">90 degree axis</param>
        /// <param name="radius0">Radius along the 0 degree axis</param>
        /// <param name="radius90">Radius along the 90 degree axis</param>
        /// <param name="startAngle">Start angle of arc</param>
        /// <param name="sweepAngle">Sweep angle of arc</param>
        public Ellipse(Vector3 center, Vector3 axis0, Vector3 axis90, float radius0, float radius90, Angle startAngle, Angle sweepAngle)
        {
            Center = center;
            Axis0 = axis0;
            Axis90 = axis90;
            Radius0 = radius0;
            Radius90 = radius90;
            StartAngle = startAngle;
            SweepAngle = sweepAngle;
        }

        /// <summary>
        /// Gets the size of the <see cref="Ellipse"/> type in bytes.
        /// </summary>
        public static int SizeInBytes => MemoryHelper.SizeOf<Ellipse>();

        /// <summary>
        /// Gets the unit circle where both radii are one and the sweep makes a complete circle.
        /// </summary>
        public static Ellipse UnitCircle => new Ellipse(Vector3.Zero, Vector3.UnitX, Vector3.UnitY, 1.0f, 1.0f, Angle.Zero, Angle.TwoPi);

        /// <summary>
        /// Gets the normal of the ellipse.
        /// </summary>
        public Vector3 Normal
        {
            get
            {
                Vector3.NormalizedCross(ref Axis0, ref Axis90, out Vector3 normal);
                return normal;
            }
        }
    }
}
