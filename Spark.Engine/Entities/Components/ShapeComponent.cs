namespace Spark.Entities
{
    public abstract class ShapeComponent : GeometryComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeComponent"/> class.
        /// </summary>
        protected ShapeComponent()
            : this("ShapeComponent")
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeComponent"/> class.
        /// </summary>
        /// <param name="name">Name of the component</param>
        protected ShapeComponent(string name)
            : base(name)
        {
        }
    }
}
