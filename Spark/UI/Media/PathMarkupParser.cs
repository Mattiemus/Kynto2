namespace Spark.UI.Media
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using Math;

    public class PathMarkupParser
    {
        private static readonly Dictionary<char, Command> Commands = new Dictionary<char, Command>
        {
            { 'F', Command.FillRule },
            { 'f', Command.FillRule },
            { 'M', Command.Move },
            { 'm', Command.MoveRelative },
            { 'L', Command.Line },
            { 'l', Command.LineRelative },
            { 'H', Command.HorizontalLine },
            { 'h', Command.HorizontalLineRelative },
            { 'V', Command.VerticalLine },
            { 'v', Command.VerticalLineRelative },
            { 'C', Command.CubicBezierCurve },
            { 'c', Command.CubicBezierCurveRelative },
            { 'Z', Command.Close },
            { 'z', Command.Close },
        };

        private readonly StreamGeometry _geometry;
        private readonly StreamGeometryContext _context;

        public PathMarkupParser(StreamGeometry geometry, StreamGeometryContext context)
        {
            _geometry = geometry;
            _context = context;
        }

        public void Parse(string s)
        {
            using (StringReader reader = new StringReader(s))
            {
                Command lastCommand = Command.None;
                Command command;
                Vector2 startPoint = new Vector2();
                Vector2 point = new Vector2();

                while ((command = ReadCommand(reader, lastCommand)) != Command.Eof)
                {
                    switch (command)
                    {
                        case Command.FillRule:
                            // TODO: Implement.
                            reader.Read();
                            break;

                        case Command.Move:
                        case Command.MoveRelative:
                            point = startPoint = ReadPoint(reader);
                            _context.BeginFigure(point, true, false);
                            break;

                        case Command.Line:
                            point = ReadPoint(reader);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.LineRelative:
                            point = ReadRelativePoint(reader, point);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.HorizontalLine:
                            point.X = ReadFloat(reader);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.HorizontalLineRelative:
                            point.X += ReadFloat(reader);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.VerticalLine:
                            point.Y = ReadFloat(reader);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.VerticalLineRelative:
                            point.Y += ReadFloat(reader);
                            _context.LineTo(point, true, false);
                            break;

                        case Command.CubicBezierCurve:
                            {
                                Vector2 point1 = ReadPoint(reader);
                                Vector2 point2 = ReadPoint(reader);
                                point = ReadPoint(reader);
                                _context.BezierTo(point1, point2, point, true, false);
                                break;
                            }

                        case Command.Close:
                            _context.LineTo(startPoint, true, false);
                            break;

                        default:
                            throw new NotSupportedException("Unsupported command");
                    }

                    lastCommand = command;
                }
            }
        }

        private static Command ReadCommand(StringReader reader, Command lastCommand)
        {
            ReadWhitespace(reader);

            int i = reader.Peek();

            if (i == -1)
            {
                return Command.Eof;
            }

            char c = (char)i;
            Command command = Command.None;
            bool canMove = lastCommand == Command.None || lastCommand == Command.FillRule;

            if (!Commands.TryGetValue(c, out command))
            {
                if ((char.IsDigit(c) || c == '.' || c == '+' || c == '-') && (lastCommand != Command.None))
                {
                    return lastCommand;
                }

                throw new InvalidDataException("Unexpected path command '" + c + "'.");
            }

            if (!canMove && command <= Command.MoveRelative)
            {
                command += 2;
            }

            reader.Read();
            return command;
        }

        private static float ReadFloat(TextReader reader)
        {
            // TODO: Handle Infinity, NaN and scientific notation.
            StringBuilder b = new StringBuilder();
            bool readSign = false;
            bool readPoint = false;
            bool readExponent = false;
            int i;

            while ((i = reader.Peek()) != -1)
            {
                char c = char.ToUpperInvariant((char)i);

                if (((c == '+' || c == '-') && !readSign) ||
                    (c == '.' && !readPoint) ||
                    (c == 'E' && !readExponent) ||
                    char.IsDigit(c))
                {
                    b.Append(c);
                    reader.Read();
                    readSign = c == '+' || c == '-';
                    readPoint = c == '.';

                    if (c == 'E')
                    {
                        readSign = false;
                        readExponent = c == 'E';
                    }
                }
                else
                {
                    break;
                }
            }

            return float.Parse(b.ToString());
        }

        private static Vector2 ReadPoint(StringReader reader)
        {
            ReadWhitespace(reader);
            float x = ReadFloat(reader);
            ReadSeparator(reader);
            float y = ReadFloat(reader);
            return new Vector2(x, y);
        }

        private static Vector2 ReadRelativePoint(StringReader reader, Vector2 lastPoint)
        {
            ReadWhitespace(reader);
            float x = ReadFloat(reader);
            ReadSeparator(reader);
            float y = ReadFloat(reader);
            return new Vector2(lastPoint.X + x, lastPoint.Y + y);
        }

        private static void ReadSeparator(StringReader reader)
        {
            int i;
            bool readComma = false;

            while ((i = reader.Peek()) != -1)
            {
                char c = (char)i;

                if (char.IsWhiteSpace(c))
                {
                    reader.Read();
                }
                else if (c == ',')
                {
                    if (readComma)
                    {
                        throw new InvalidDataException("Unexpected ','.");
                    }

                    readComma = true;
                    reader.Read();
                }
                else
                {
                    break;
                }
            }
        }

        private static void ReadWhitespace(StringReader reader)
        {
            int i;
            while ((i = reader.Peek()) != -1)
            {
                char c = (char)i;

                if (char.IsWhiteSpace(c))
                {
                    reader.Read();
                }
                else
                {
                    break;
                }
            }
        }

        private enum Command
        {
            None,
            FillRule,
            Move,
            MoveRelative,
            Line,
            LineRelative,
            HorizontalLine,
            HorizontalLineRelative,
            VerticalLine,
            VerticalLineRelative,
            CubicBezierCurve,
            CubicBezierCurveRelative,
            Close,
            Eof,
        }
    }
}
