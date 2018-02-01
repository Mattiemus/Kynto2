namespace Spark.Content.Importers
{
    using System;
    using System.IO;
    using System.Xml;

    using Graphics;
    using Math;

    public sealed class BitmapFontImporter : ResourceImporter<SpriteFont>
    {
        public BitmapFontImporter() 
            : base(".fnt")
        {
        }
        
        public override SpriteFont Load(IResourceFile resourceFile, ContentManager contentManager, ImporterParameters parameters)
        {
            var document = new XmlDocument();
            document.Load(resourceFile.OpenRead());

            XmlNode fontNode = ParseFontNode(document);
            ParseFontData(fontNode, out string[] texturePaths, out int lineHeight, out XmlNode charsNode, out XmlNode kerningsNode);

            var textures = new Texture2D[texturePaths.Length];
            for (int i = 0; i < textures.Length; i++)
            {
                textures[i] = contentManager.LoadRelativeTo<Texture2D>(texturePaths[i], resourceFile, parameters);
            }

            var font = new SpriteFont(lineHeight, textures);

            AddCharacters(font, charsNode);
            AddKernings(font, kerningsNode);

            return font;
        }

        public override SpriteFont Load(Stream input, ContentManager contentManager, ImporterParameters parameters)
        {
            // Multi-file content may not be supported
            throw new NotImplementedException();
        }

        private XmlNode ParseFontNode(XmlDocument document)
        {
            if (!document.HasChildNodes)
            {
                throw new InvalidDataException("File does not contain font data.");
            }

            foreach (XmlNode node in document.ChildNodes)
            {
                if (node.Name.Equals("font"))
                {
                    return node;
                }
            }

            throw new InvalidDataException("File does not contain font data.");
        }

        private void ParseFontData(XmlNode fontNode, out string[] texturesPaths, out int lineHeight, out XmlNode charsNode, out XmlNode kerningsNode)
        {
            if (!fontNode.HasChildNodes)
            {
                throw new InvalidDataException("File does not contain font data.");
            }

            XmlNode commonNode = null;
            XmlNode pagesNode = null;
            charsNode = null;
            kerningsNode = null;
            lineHeight = 0;
            foreach (XmlNode node in fontNode.ChildNodes)
            {
                switch (node.Name)
                {
                    case "common":
                        commonNode = node;
                        break;
                    case "pages":
                        pagesNode = node;
                        break;
                    case "chars":
                        charsNode = node;
                        break;
                    case "kernings":
                        kerningsNode = node;
                        break;
                }
            }

            if (commonNode == null || commonNode.Attributes.Count == 0)
            {
                throw new InvalidDataException("Missing common font data.");
            }

            if (charsNode == null || !charsNode.HasChildNodes)
            {
                throw new InvalidDataException("Missing character font data.");
            }

            XmlNode lineHeightAttr = commonNode.Attributes.GetNamedItem("lineHeight");

            if (lineHeightAttr != null && !int.TryParse(lineHeightAttr.Value, out lineHeight))
            {
                lineHeight = 0;
            }

            if (pagesNode == null || pagesNode.ChildNodes.Count == 0)
            {
                throw new InvalidDataException("Missing page font data.");
            }

            texturesPaths = new string[pagesNode.ChildNodes.Count];
            for (int i = 0; i < pagesNode.ChildNodes.Count; i++)
            {
                XmlNode page = pagesNode.ChildNodes[i];
                if (page != null)
                {
                    XmlNode pathAttr = page.Attributes.GetNamedItem("file");
                    if (pathAttr != null)
                    {
                        texturesPaths[i] = pathAttr.Value;
                    }
                }
            }
        }

        private void ParseGlyph(XmlNode charNode, out Glyph glyph)
        {
            if (charNode.Attributes.Count < 10)
            {
                throw new InvalidDataException("Missing character information.");
            }

            XmlAttributeCollection attributes = charNode.Attributes;
            XmlNode idNode = attributes.GetNamedItem("id");
            XmlNode xNode = attributes.GetNamedItem("x");
            XmlNode yNode = attributes.GetNamedItem("y");
            XmlNode widthNode = attributes.GetNamedItem("width");
            XmlNode heightNode = attributes.GetNamedItem("height");
            XmlNode xOffsetNode = attributes.GetNamedItem("xoffset");
            XmlNode yOffsetNode = attributes.GetNamedItem("yoffset");
            XmlNode xAdvanceNode = attributes.GetNamedItem("xadvance");
            XmlNode pageIdNode = attributes.GetNamedItem("page");

            if (idNode == null || 
                xNode == null ||
                yNode == null || 
                widthNode == null || 
                heightNode == null ||
                xOffsetNode == null || 
                yOffsetNode == null ||
                xAdvanceNode == null || 
                pageIdNode == null)
            {
                throw new InvalidDataException("Missing character information.");
            }

            int id = int.Parse(idNode.Value);

            // Default (invalid char) found, put it in the proper range.
            if (id == -1)
            {
                id = 0;
            }

            int x = int.Parse(xNode.Value);
            int y = int.Parse(yNode.Value);
            int width = int.Parse(widthNode.Value);
            int height = int.Parse(heightNode.Value);
            int xOffset = int.Parse(xOffsetNode.Value);
            int yOffset = int.Parse(yOffsetNode.Value);
            int xAdvance = int.Parse(xAdvanceNode.Value);
            int pageId = int.Parse(pageIdNode.Value);

            glyph = new Glyph((char)id, new Rectangle(x, y, width, height), pageId, new Vector2(xOffset, yOffset), xAdvance);
        }

        private void AddCharacters(SpriteFont font, XmlNode charsNode)
        {
            foreach (XmlNode node in charsNode.ChildNodes)
            {
                ParseGlyph(node, out Glyph glyph);

                if (glyph.Literal == 0)
                {
                    font.SetDefaultCharacter(glyph);
                }
                else
                {
                    font.AddGlyph(glyph);
                }
            }
        }

        private void AddKernings(SpriteFont font, XmlNode kernings)
        {
            if (kernings == null)
            {
                return;
            }

            foreach (XmlNode node in kernings.ChildNodes)
            {
                XmlAttributeCollection attributes = node.Attributes;
                XmlNode first = attributes.GetNamedItem("first");
                XmlNode second = attributes.GetNamedItem("second");
                XmlNode amount = attributes.GetNamedItem("amount");

                if (first == null || second == null || amount == null)
                {
                    throw new InvalidDataException("Missing kerning data.");
                }

                font.AddKerning((char)int.Parse(second.Value), (char)int.Parse(first.Value), int.Parse(amount.Value));
            }
        }
    }
}
