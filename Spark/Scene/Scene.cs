namespace Spark.Scene
{
    /// <summary>
    /// Representation of the root of a scene graph
    /// </summary>
    public class Scene : Node
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        public Scene()
            : base("Scene")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        /// <param name="sceneName">Scene name</param>
        public Scene(string sceneName)
            : base(sceneName)
        {
        }
    }
}
