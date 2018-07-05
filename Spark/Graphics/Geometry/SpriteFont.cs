namespace Spark.Graphics
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Text;
    
    using Math;
    using Content;

    /// <summary>
    /// Represents a font that is used to render text as a series of sprites on the screen. Each sprite is a single character represented by a sub-image from
    /// a single or multiple glyph texture atlasses and includes spacing data such as kernings to properly render text to the screen.
    /// </summary>
    public sealed class SpriteFont : ISavable, INamable
    {
        private const short Version = 1;
        
        private Texture2D[] _textures;
        private ReadOnlyList<Texture2D> _readOnlyTextures;
        private char? _defaultChar;
        private readonly Dictionary<char, Glyph> _glyphs;
        private readonly Dictionary<char, Dictionary<char, int>> _kerningMap;

        /// <summary>
        /// Initializes a default instance of the <see cref="SpriteFont"/> class from being created.
        /// </summary>
        private SpriteFont() 
            : this(0, 0, null, new Texture2D[0])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="lineHeight">Vertical height between consecutive lines, includes blank space and height of the characters on the line.</param>
        /// <param name="textures">Array of textures that glyphs in the font map to.</param>
        public SpriteFont(int lineHeight, params Texture2D[] textures) 
            : this(lineHeight, 0, null, textures)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="lineHeight">Vertical height between consecutive lines, includes blank space and height of the characters on the line.</param>
        /// <param name="defaultChar">Default character, if any.</param>
        /// <param name="textures">Array of textures that glyphs in the font map to.</param>
        public SpriteFont(int lineHeight, Glyph? defaultChar, params Texture2D[] textures) 
            : this(lineHeight, 0, defaultChar, textures)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="lineHeight">Vertical height between consecutive lines, includes blank space and height of the characters on the line.</param>
        /// <param name="horizontalPadding">Horizontal padding between consecutive characters on the same line.</param>
        /// <param name="textures">Array of textures that glyphs in the font map to.</param>
        public SpriteFont(int lineHeight, int horizontalPadding, params Texture2D[] textures) 
            : this(lineHeight, horizontalPadding, null, textures)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SpriteFont"/> class.
        /// </summary>
        /// <param name="lineHeight">Vertical height between consecutive lines, includes blank space and height of the characters on the line.</param>
        /// <param name="horizontalPadding">Horizontal padding between consecutive characters on the same line.</param>
        /// <param name="defaultChar">Default character, if any.</param>
        /// <param name="textures">Array of textures that glyphs in the font map to.</param>
        public SpriteFont(int lineHeight, int horizontalPadding, Glyph? defaultChar, params Texture2D[] textures)
        {
            Name = string.Empty;
            _glyphs = new Dictionary<char, Glyph>();
            _kerningMap = new Dictionary<char, Dictionary<char, int>>();
            LineHeight = lineHeight;
            HorizontalPadding = horizontalPadding;
            _defaultChar = null;
            _textures = textures ?? new Texture2D[0];

            if (defaultChar.HasValue)
            {
                _defaultChar = defaultChar.Value.Literal;
                _glyphs.Add(defaultChar.Value.Literal, defaultChar.Value);
            }
        }

        /// <summary>
        /// Gets or sets the name of the object.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the default character that is rendered if a character in text being rendered is missing. May be null.
        /// </summary>
        public char? DefaultCharacter => _defaultChar;

        /// <summary>
        /// Gets the vertical distance between consecutive lines of text, which includes the blank space between the lines and the height of the characters on the line, in pixels.
        /// </summary>
        public int LineHeight { get; set; }

        /// <summary>
        /// Gets the horizontal padding between characters on the same line, in pixels.
        /// </summary>
        public int HorizontalPadding { get; set; }

        /// <summary>
        /// Gets the textures that hold the textural glyph information used during rendering.
        /// </summary>
        public IReadOnlyList<Texture2D> Textures
        {
            get
            {
                if (_readOnlyTextures == null)
                {
                    _readOnlyTextures = new ReadOnlyList<Texture2D>(_textures);
                }

                return _readOnlyTextures;
            }
        }

        /// <summary>
        /// Gets the characters and their glyph data represented in this sprite font.
        /// </summary>
        public IReadOnlyDictionary<char, Glyph> Characters => _glyphs;

        /// <summary>
        /// Sets the default character of the sprite font, this is used if a character to be rendered
        /// is not present in the font.
        /// </summary>
        /// <param name="glyph">Glyph representing the default character.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool SetDefaultCharacter(Glyph glyph)
        {
            if (!_defaultChar.HasValue)
            {
                AddGlyph(glyph);
                _defaultChar = glyph.Literal;
                return true;
            }

            if (_defaultChar.Value == glyph.Literal)
            {
                return false;
            }

            RemoveGlyph(_defaultChar.Value);
            return AddGlyph(glyph);
        }

        /// <summary>
        /// Adds a glyph to the sprite font.
        /// </summary>
        /// <param name="glyph">Glyph to add</param>
        public bool AddGlyph(Glyph glyph)
        {
            if (_glyphs.ContainsKey(glyph.Literal))
            {
                return false;
            }

            _glyphs.Add(glyph.Literal, glyph);
            return true;
        }

        /// <summary>
        /// Removes a glyph from the sprite font.
        /// </summary>
        /// <param name="literal">Character to remove</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool RemoveGlyph(char literal)
        {
            bool removed = _glyphs.Remove(literal);
            if (removed)
            {
                _kerningMap.Remove(literal);
            }

            return removed;
        }

        /// <summary>
        /// Adds a kerning pair to the sprite font. Kerning is the process of adjusting a character's horizontal space
        /// with the preceeding character.
        /// </summary>
        /// <param name="second">Current character.</param>
        /// <param name="first">Preceeding character.</param>
        /// <param name="amount">Amount to move the current character horizontally.</param>
        /// <returns>True if the operation was successful, false otherwise.</returns>
        public bool AddKerning(char second, char first, int amount)
        {
            if (!_kerningMap.TryGetValue(second, out Dictionary<char, int> kernings))
            {
                kernings = new Dictionary<char, int>();
                _kerningMap.Add(second, kernings);
            }

            if (kernings.ContainsKey(first))
            {
                return false;
            }

            kernings.Add(first, amount);
            return true;
        }

        /// <summary>
        /// Removes a kerning pair from the sprite font.
        /// </summary>
        /// <param name="second">Current character.</param>
        /// <param name="first">Preceeding character.</param>
        /// <returns>True if the operation was succesful, false otherwise.</returns>
        public bool RemoveKerning(char second, char first)
        {
            if (!_kerningMap.TryGetValue(second, out Dictionary<char, int> kernings))
            {
                return true;
            }

            return kernings.Remove(first);
        }

        /// <summary>
        /// Queries if a kerning pair is contained in the sprite font.
        /// </summary>
        /// <param name="second">Current character.</param>
        /// <param name="first">Preceeding character.</param>
        /// <returns>True if the kerning pair is present in the sprite font, false otherwise.</returns>
        public bool ContainsKerning(char second, char first)
        {
            if (!_kerningMap.TryGetValue(second, out Dictionary<char, int>  kernings))
            {
                return false;
            }

            return kernings.ContainsKey(first);
        }

        /// <summary>
        /// Gets the kerning amount for the kerning pair.
        /// </summary>
        /// <param name="second">Current character.</param>
        /// <param name="first">Preceeding character.</param>
        /// <returns>Kerning amount between the pair of characters.</returns>
        public int GetKerning(char second, char first)
        {
            if (_kerningMap.TryGetValue(second, out Dictionary<char, int> kernings))
            {
                if (kernings.TryGetValue(first, out int amount))
                {
                    return amount;
                }
            }

            return 0;
        }

        /// <summary>
        /// Gets all the kerning pairs corresponding to the character literal.
        /// </summary>
        /// <param name="second">The character literal, which is the "second" character in the kerning pair.</param>
        /// <returns>Array of kerning pairs.</returns>
        public KerningPair[] GetKernings(char second)
        {
            if (_kerningMap.TryGetValue(second, out Dictionary<char, int> kernings))
            {
                KerningPair[] pairs = new KerningPair[kernings.Count];
                int index = 0;
                foreach (KeyValuePair<char, int> kv in kernings)
                {
                    pairs[index] = new KerningPair(kv.Key, second, kv.Value);
                    index++;
                }
            }

            return new KerningPair[0];
        }

        /// <summary>
        /// Measures the string, returning the maximum width and height (stored in X and Y respectively)
        /// of the string as it would appear when rendered.
        /// </summary>
        /// <param name="text">Text to measure</param>
        /// <returns>Maximum width and height stored in X and Y coordinates.</returns>
        public Vector2 MeasureString(string text)
        {
            MeasureStringInternal(new StringReference(text), out Vector2 result);
            return result;
        }

        /// <summary>
        /// Measures the string, returning the maximum width and height (stored in X and Y respectively)
        /// of the string as it would appear when rendered.
        /// </summary>
        /// <param name="text">Text to measure</param>
        /// <returns>Maximum width and height stored in X and Y coordinates.</returns>
        public Vector2 MeasureString(StringBuilder text)
        {
            MeasureStringInternal(new StringReference(text), out Vector2 result);
            return result;
        }

        /// <summary>
        /// Draws a string
        /// </summary>
        /// <param name="batch">Spritebatch to draw the string with</param>
        /// <param name="text">Text to draw</param>
        /// <param name="position">Position to draw the string at</param>
        /// <param name="scale">Scale to draw the string at</param>
        /// <param name="tintColor">String color</param>
        /// <param name="angle">Angle to draw the string at</param>
        /// <param name="origin">Origin within the string</param>
        /// <param name="flipEffect">String flip effect</param>
        /// <param name="depth">Depth to draw the string at</param>
        internal void DrawString(SpriteBatch batch, StringReference text, Vector2 position, Vector2 scale, Color tintColor, Angle angle, Vector2 origin, SpriteFlipEffect flipEffect, float depth)
        {
            //TODO - Angle, Scale aren't quite right. And need to implement flip effect
            
            Matrix4x4.FromRotationZ(angle, out Matrix4x4 transform);
            Matrix4x4.FromTranslation(-origin.X * scale.X, -origin.Y * scale.Y, 0.0f, out Matrix4x4 translation);
            Matrix4x4.Multiply(ref translation, ref transform, out transform);

            float lx = 0.0f;
            float ly = 0.0f;

            char prevChar = '\0';
            for (int i = 0; i < text.Length; i++)
            {
                char literal = text[i];
                switch (literal)
                {
                    case '\r':
                        break;
                    case '\n':
                        lx = 0.0f;
                        ly += LineHeight;
                        break;
                    default:
                        bool hasIt = true;
                        if (!_glyphs.TryGetValue(literal, out Glyph glyph))
                        {
                            if (_defaultChar.HasValue)
                            {
                                glyph = _glyphs[_defaultChar.Value];
                            }
                            else
                            {
                                hasIt = false;
                            }
                        }

                        if (hasIt)
                        {
                            Vector2 pos = new Vector2(lx + glyph.Offset.X, ly + glyph.Offset.Y);
                            Vector2.Transform(ref pos, ref transform, out pos);

                            pos.X += position.X;
                            pos.Y += position.Y;

                            batch.Draw(_textures[glyph.TextureIndex], pos, scale, glyph.Cropping, tintColor, angle, Vector2.Zero, flipEffect, depth);

                            lx += ComputeAdvance(ref glyph, ref prevChar);
                        }

                        prevChar = literal;
                        break;
                }
            }
        }

        /// <summary>
        /// Measures a string
        /// </summary>
        /// <param name="stringRef">String to measure</param>
        /// <param name="result">Size of the string</param>
        private void MeasureStringInternal(StringReference stringRef, out Vector2 result)
        {
            result = Vector2.Zero;

            if (stringRef.Length == 0)
            {
                return;
            }

            float maxLineWidth = 0.0f;
            int newLineCount = 1;
            char prevChar = '\0';

            for (int i = 0; i < stringRef.Length; i++)
            {
                char literal = stringRef[i];
                switch (literal)
                {
                    case '\r':
                        break;
                    case '\n':
                        maxLineWidth = Math.Max(result.X, maxLineWidth);

                        result.X = 0.0f;

                        newLineCount++;
                        break;
                    default:
                        bool hasIt = true;
                        if (!_glyphs.TryGetValue(literal, out Glyph glyph))
                        {
                            if (_defaultChar.HasValue)
                            {
                                glyph = _glyphs[_defaultChar.Value];
                            }
                            else
                            {
                                hasIt = false;
                            }
                        }

                        if (hasIt)
                        {
                            result.X += ComputeAdvance(ref glyph, ref prevChar);
                        }

                        break;
                }

                prevChar = literal;
            }

            result.X = Math.Max(result.X, maxLineWidth);
            result.Y = newLineCount * LineHeight;
        }

        /// <summary>
        /// Computes how far to advance the rendering by for a given glyph
        /// </summary>
        /// <param name="g">Next glyph</param>
        /// <param name="prevChar">Previous drawn character</param>
        /// <returns>Amount to advance by</returns>
        private float ComputeAdvance(ref Glyph g, ref char prevChar)
        {
            return g.XAdvance + GetKerning(g.Literal, prevChar) + HorizontalPadding;
        }

        /// <summary>
        /// Reads the object data from the input.
        /// </summary>
        /// <param name="input">Savable reader</param>
        public void Read(ISavableReader input)
        {
            input.ReadInt16();
            Name = input.ReadString();
            int textureCount = input.ReadInt32();
            _textures = (textureCount > 0) ? new Texture2D[textureCount] : new Texture2D[0];
            _readOnlyTextures = null;

            for (int i = 0; i < textureCount; i++)
            {
                _textures[i] = input.ReadSharedSavable<Texture2D>();
            }

            _defaultChar = null;

            if (input.ReadBoolean())
            {
                _defaultChar = input.ReadChar();
            }

            LineHeight = input.ReadInt32();
            HorizontalPadding = input.ReadInt32();

            int glyphCount = input.ReadInt32();
            _glyphs.Clear();
            _kerningMap.Clear();

            for (int i = 0; i < glyphCount; i++)
            {
                Glyph g = input.Read<Glyph>();
                _glyphs.Add(g.Literal, g);

                int kerningCount = input.ReadInt32();

                if (kerningCount > 0)
                {
                    Dictionary<char, int> kernings = new Dictionary<char, int>();
                    _kerningMap.Add(g.Literal, kernings);

                    for (int j = 0; j < kerningCount; j++)
                    {
                        char firstChar = input.ReadChar();
                        int kerningAmount = input.ReadInt32();

                        kernings.Add(firstChar, kerningAmount);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the object data to the output.
        /// </summary>
        /// <param name="output">Savable writer</param>
        public void Write(ISavableWriter output)
        {
            output.Write("Version", Version);
            output.Write("Name", Name);
            output.Write("TextureCount", _textures.Length);
            for (int i = 0; i < _textures.Length; i++)
            {
                output.WriteSharedSavable("Texture", _textures[i]);
            }

            output.Write("HasDefaultCharacter", _defaultChar.HasValue);

            if (_defaultChar.HasValue)
            {
                output.Write("DefaultCharacter", _defaultChar.Value);
            }

            output.Write("VerticalSpacing", LineHeight);
            output.Write("HorizontalSpacing", HorizontalPadding);

            output.Write("GlyphCount", _glyphs.Count);

            foreach (Glyph glyph in _glyphs.Values)
            {
                output.Write("Glyph", glyph);
                _kerningMap.TryGetValue(glyph.Literal, out Dictionary<char, int> kernings);

                if (kernings != null)
                {
                    output.Write("KerningCount", kernings.Count);
                    foreach (KeyValuePair<char, int> kv in kernings)
                    {
                        output.Write("FirstCharacter", kv.Key);
                        output.Write("KerningAmount", kv.Value);
                    }
                }
                else
                {
                    output.Write("KerningCount", 0);
                }
            }
        }
    }

    /// <summary>
    /// Represents a kerning amount between two characters. The kerning is the amount of 
    /// space on the X axis between the "second" character and the preceeding "first" character.
    /// </summary>
    public struct KerningPair : IPrimitiveValue
    {
        /// <summary>
        /// First character in the pair.
        /// </summary>
        public char FirstCharacter;

        /// <summary>
        /// Second character in the pair.
        /// </summary>
        public char SecondCharacter;

        /// <summary>
        /// Kerning amount between the second and first characters.
        /// </summary>
        public int KerningAmount;

        /// <summary>
        /// Initializes a new instance of the <see cref="KerningPair"/> struct.
        /// </summary>
        /// <param name="firstChar">The first character.</param>
        /// <param name="secondChar">The second character.</param>
        /// <param name="amount">The kerning amount between the second character and the first.</param>
        public KerningPair(char firstChar, char secondChar, int amount)
        {
            FirstCharacter = firstChar;
            SecondCharacter = secondChar;
            KerningAmount = amount;
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            FirstCharacter = input.ReadChar();
            SecondCharacter = input.ReadChar();
            KerningAmount = input.ReadInt32();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("FirstCharacter", FirstCharacter);
            output.Write("SecondCharacter", SecondCharacter);
            output.Write("KerningAmount", KerningAmount);
        }
    }

    /// <summary>
    /// Represents a single glyph and its layout information found on a texture.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Glyph : IPrimitiveValue
    {
        /// <summary>
        /// Character the glyph corresponds to.
        /// </summary>
        public char Literal;

        /// <summary>
        /// Cropping rectangle in pixels, the sub-image representing the glyph from the texture.
        /// </summary>
        public Rectangle Cropping;

        /// <summary>
        /// The index in the sprite font representing which texture the glyph is found on.
        /// </summary>
        public int TextureIndex;

        /// <summary>
        /// Represents the top-left origin of the glyph quad in pixels.
        /// </summary>
        public Vector2 Offset;

        /// <summary>
        /// The amount of space to add from the origin of the glyph quad to the next glyph's top-left origin along the X axis in pixels.
        /// </summary>
        public float XAdvance;

        /// <summary>
        /// Initializes a new instance of the <see cref="Glyph"/> struct.
        /// </summary>
        /// <param name="literal">The character literal.</param>
        /// <param name="cropping">The cropping rectangle, in pixels.</param>
        /// <param name="textureIndex">The texture index the glyph is found on.</param>
        /// <param name="offset">The offset representing the top-left corner of the glyph.</param>
        /// <param name="xAdvance">The amount to advance to the next glyph along the X axis.</param>
        public Glyph(char literal, Rectangle cropping, int textureIndex, Vector2 offset, float xAdvance)
        {
            Literal = literal;
            Cropping = cropping;
            TextureIndex = textureIndex;
            Offset = offset;
            XAdvance = xAdvance;
        }

        /// <summary>
        /// Reads the primitive data from the input.
        /// </summary>
        /// <param name="input">Primitive reader</param>
        public void Read(IPrimitiveReader input)
        {
            Literal = input.ReadChar();
            Cropping = input.Read<Rectangle>();
            TextureIndex = input.ReadInt32();
            Offset = input.Read<Vector2>();
            XAdvance = input.ReadSingle();
        }

        /// <summary>
        /// Writes the primitive data to the output.
        /// </summary>
        /// <param name="output">Primitive writer</param>
        public void Write(IPrimitiveWriter output)
        {
            output.Write("Literal", Literal);
            output.Write("Cropping", ref Cropping);
            output.Write("TextureIndex", TextureIndex);
            output.Write("Offset", ref Offset);
            output.Write("XAdvance", XAdvance);
        }
    }

    /// <summary>
    /// Reference to a string
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct StringReference
    {
        private readonly string _string;
        private readonly StringBuilder _stringBuilder;

        /// <summary>
        /// Initializes a new instance of the <see cref="StringReference"/> class.
        /// </summary>
        /// <param name="str">String to reference</param>
        public StringReference(string str)
        {
            _string = str;
            _stringBuilder = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringReference"/> class.
        /// </summary>
        /// <param name="strBuilder">String builder instance to reference</param>
        public StringReference(StringBuilder strBuilder)
        {
            _string = null;
            _stringBuilder = strBuilder;
        }

        /// <summary>
        /// Gets the character at the given position in the string
        /// </summary>
        /// <param name="index">String index</param>
        /// <returns>Character at the given index</returns>
        public char this[int index]
        {
            get
            {
                if (_string != null)
                {
                    return _string[index];
                }

                if (_stringBuilder != null)
                {
                    return _stringBuilder[index];
                }

                return '\0';
            }
        }

        /// <summary>
        /// Gets the length of the string we are referencing
        /// </summary>
        public int Length
        {
            get
            {
                if (_string != null)
                {
                    return _string.Length;
                }

                if (_stringBuilder != null)
                {
                    return _stringBuilder.Length;
                }

                return 0;
            }
        }
    }
}

