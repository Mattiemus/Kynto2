namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    
    using Math;
    using Content;

    /// <summary>
    /// A texture atlas is a single texture that contains a number of sub-textures, all referenced by a name and a rectangle
    /// (in texel coordinates). It is commonly used to batch small textures together to reduce state changes in the render pipeline.
    /// </summary>
    public class TextureAtlas : Dictionary<string, Rectangle>, ISavable, INamable, IContentCastable
    {
        private string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAtlas"/> class.
        /// </summary>
        protected TextureAtlas()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TextureAtlas"/> class.
        /// </summary>
        /// <param name="texture">The texture.</param>
        public TextureAtlas(Texture2D texture)
        {
            if (texture == null)
            {
                throw new ArgumentNullException(nameof(texture));
            }

            Texture = texture;
            _name = string.Empty;
        }
        
        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name
        {
            get => _name;
            set
            {
                if (Texture != null)
                {
                    Texture.Name = value;
                }

                _name = value;
            }
        }

        /// <summary>
        /// Gets or sets the texture.
        /// </summary>
        public Texture2D Texture { get; set; }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            _name = input.ReadString();
            Texture = input.ReadSharedSavable<Texture2D>();

            int numSubImages = input.ReadInt32();

            for (int i = 0; i < numSubImages; i++)
            {
                string name = input.ReadString();
                input.Read(out Rectangle rect);

                Add(name, rect);
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Name", _name);

            output.WriteSharedSavable("Texture", Texture);

            output.Write("Count", Count);

            foreach (KeyValuePair<string, Rectangle> kv in this)
            {
                output.Write("Key", kv.Key);
                output.Write("Value", kv.Value);
            }
        }

        /// <summary>
        /// Attempts to cast the content item to another type.
        /// </summary>
        /// <param name="targetType">Type to cast to.</param>
        /// <param name="subresourceName">Optional subresource name.</param>
        /// <returns>Casted type or null if the type could not be converted.</returns>
        public object CastTo(Type targetType, string subresourceName)
        {
            if (targetType.IsAssignableFrom(typeof(Texture2D)))
            {
                return Texture;
            }

            return this;
        }
    }
}
