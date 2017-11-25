namespace Spark.OpenGL.Graphics
{
    using System.Globalization;
    using System.Collections;
    using System.Collections.Generic;

    using Spark.Graphics;

    /// <summary>
    /// Effect bytecode provider for D3D11 effects.
    /// </summary>
    public sealed class OpenGLEffectByteCodeProvider : StandardEffectLibrary.BaseProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenGLEffectByteCodeProvider"/> class.
        /// </summary>
        public OpenGLEffectByteCodeProvider()
            : base(string.Empty)
        {
        }

        /// <summary>
        /// Called to preload all the effect byte code buffers.
        /// </summary>
        /// <param name="effectByteCodes">Cache that will hold the effect byte code buffers.</param>
        protected override void Preload(Dictionary<string, byte[]> effectByteCodes)
        {
            // Key should always be a string (name of the file) and value should always be a byte[]
            foreach (DictionaryEntry kv in StandardEffects.ResourceManager.GetResourceSet(CultureInfo.InvariantCulture, true, false))
            {
                effectByteCodes.Add((string)kv.Key, (byte[])kv.Value);
            }

            // TODO: remove
            effectByteCodes.Add("Sprite", EffectData.ToBytes(new EffectData
            {
                EffectName = "SpriteTexture",
                VertexShader =
@"
#version 330 core

uniform mat4 SpriteTransform;

out vec2 uv;

layout(location = 0) in vec3 vertexPosition_modelspace;
layout(location = 1) in vec2 textureCoordinate;

void main() 
{
    uv = textureCoordinate;
    gl_Position = SpriteTransform * vec4(vertexPosition_modelspace, 1.0);
}
",

                PixelShader =
@"
#version 330 core

uniform sampler2D SpriteMapSampler;

in vec2 uv;

out vec3 color;

void main() 
{
	color = texture(SpriteMapSampler, uv).rgb;
}
"
            }));
        }
    }
}
