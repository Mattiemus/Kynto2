namespace Spark.Graphics.Renderer
{
    using System;

    using Math;

    /// <summary>
    /// Helper methods for the renderer
    /// </summary>
    public static class RenderHelper
    {
        /// <summary>
        /// Determines the distance between the renderable and the camera
        /// </summary>
        /// <param name="renderable">Renderable</param>
        /// <param name="cam">Camera</param>
        /// <returns>Dinstance between the renderable and the camera</returns>
        public static float DistanceToCamera(IRenderable renderable, Camera cam)
        {
            Vector3 camPos = cam.Position;
            Vector3 camDir = cam.Direction;

            Vector3 worldPos = renderable.WorldTransform.Translation;

            Vector3.Subtract(ref worldPos, ref camPos, out Vector3 distVector);
            Vector3.Dot(ref distVector, ref camDir, out float retVal);
            Vector3.Dot(ref camDir, ref camDir, out float temp);

            retVal = Math.Abs(retVal / temp);
            Vector3.Multiply(ref camDir, retVal, out distVector);

            return distVector.Length();
        }
    }
}
