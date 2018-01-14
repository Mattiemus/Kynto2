namespace Spark.Graphics
{
    using System;

    /// <summary>
    /// Defines general shader compile flags. Some options may not be available on all platforms.
    /// </summary>
    [Flags]
    public enum ShaderCompileFlags
    {
        /// <summary>
        /// No flags set. Compile with moderate optimization.
        /// </summary>
        None = 0,

        /// <summary>
        /// Output debug information with shader code.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Skip validation of shader code.
        /// </summary>
        SkipValidation = 2,

        /// <summary>
        /// Skip optimization of shader code.
        /// </summary>
        SkipOptimization = 4,

        /// <summary>
        /// Pack matrices in row major order.
        /// </summary>
        PackMatrixRowMajor = 8,

        /// <summary>
        /// Pack matrices in column major order.
        /// </summary>
        PackMatrixColumnMajor = 16,

        /// <summary>
        /// Forces all computations in the shader to occur at partial precision.
        /// </summary>
        PartialPrecision = 32,

        /// <summary>
        /// Disables preshaders. The compiler will not pull out static expressions that are evaluated on the host CPU.
        /// </summary>
        NoPreShader = 256,

        /// <summary>
        /// Hint to compiler to avoid flow control instructions.
        /// </summary>
        AvoidFlowControl = 512,

        /// <summary>
        /// Hint to compiler to not avoid flow control instructions.
        /// </summary>
        PreferFlowControl = 1024,

        /// <summary>
        /// Lowest optimization level, produces slower code but quicker.
        /// </summary>
        OptimizationLevel0 = 16384,

        /// <summary>
        /// Second highest optimization level.
        /// </summary>
        OptimizationLevel3 = 32768,

        /// <summary>
        /// Highest optimization level, produces faster code but slower.
        /// </summary>
        OptimizationLevel2 = 49152,

        /// <summary>
        /// Treat all warnings as errors.
        /// </summary>
        WarningsAreErrors = 262144
    }
}
