namespace Spark.Graphics
{
    /// <summary>
    /// Defines blending factors for source and destination colors.
    /// </summary>
    public enum Blend
    {
        /// <summary>
        /// Each color component is multiplied by (0, 0, 0, 0).
        /// </summary>
        Zero = 0,

        /// <summary>
        /// Each color component is multiplied by (1, 1, 1, 1).
        /// </summary>
        One = 1,

        /// <summary>
        /// Each color component is multiplied by the source color: (Rs, Gs, Bs, As).
        /// </summary>
        SourceColor = 2,

        /// <summary>
        /// Each color component is multiplied by the inverse source color: (1 - Rs, 1 - Gs, 1 - Bs, 1 - As).
        /// </summary>
        InverseSourceColor = 3,

        /// <summary>
        /// Each color component is multiplied by the alpha of the source color: (As, As, As, As).
        /// </summary>
        SourceAlpha = 4,

        /// <summary>
        /// Each color component is multiplied by the inverse alpha of the source color: (1 - As, 1 - As, 1 - As, 1 - As).
        /// </summary>
        InverseSourceAlpha = 5,

        /// <summary>
        /// Each color component is multiplied by the destination color: (Rd, Gd, Bd, Ad).
        /// </summary>
        DestinationColor = 6,

        /// <summary>
        /// Each color component is multiplied by the inverse destination color: (1 - Rd,1 - Gd,1 - Bd,1 - Ad).
        /// </summary>
        InverseDestinationColor = 7,

        /// <summary>
        /// Each color component is multiplied by the alpha of the destination color: (Ad, Ad, Ad, Ad).
        /// </summary>
        DestinationAlpha = 8,

        /// <summary>
        /// Each color component is multiplied by the inverse alpha of the destination color: (1 - Ad,1 - Ad,1 - Ad,1 - Ad).
        /// </summary>
        InverseDestinationAlpha = 9,

        /// <summary>
        /// Each color component is multiplied by a constant blend factor.
        /// </summary>
        BlendFactor = 10,

        /// <summary>
        /// Each color component is muliplied by the inverse of a constant blend factor.
        /// </summary>
        InverseBlendFactor = 11,

        /// <summary>
        /// Each color component is multiplied by either the alpha of the source color
        /// or the inverse of the alpha of the source color, whichever is greater: (f, f, f, 1) where
        /// f = min(A, 1 - Ad)
        /// </summary>
        SourceAlphaSaturation = 12
    }
}
