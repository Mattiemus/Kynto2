namespace Spark.Utilities.Parsing
{
    using System;
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// String tokenizer that creates a stream of tokens from an input text source. A token is typically
    /// separated by either a white space, a semi-colon, or a control character (such as a new line). Double slash (//) comments are allowed
    /// in the text, and are skipped over. Text is considered a comment at the start of two forward slash characters,
    /// and end at the following new line character.
    /// </summary>
    public class StringTokenizer
    {
        private const char EOF = (char)0;

        private StringBuilder _token;
        private int _pos;
        private string _peekToken;
        private int _line;
        private int _col;
        private readonly char[] _numDelims = { 'f' };

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTokenizer"/> class.
        /// </summary>
        public StringTokenizer() 
            : this(string.Empty)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StringTokenizer"/> class.
        /// </summary>
        /// <param name="text">Input text</param>
        public StringTokenizer(string text)
        {
            Initialize(text);
        }

        /// <summary>
        /// Gets the current token, this is the result of the last NextToken() call.
        /// </summary>
        public string CurrentToken { get; private set; }

        /// <summary>
        /// Gets or sets the current character position in the string that is being tokenized.
        /// </summary>
        public int CurrentPosition
        {
            get => _pos;
            set
            {
                _pos = Math.Max(0, value);
                GoToNextToken();
            }
        }

        /// <summary>
        /// Gets the line number of the current token.
        /// </summary>
        public int LineNumber { get; private set; }

        /// <summary>
        /// Gets the starting column value of the current token.
        /// </summary>
        public int Column { get; private set; }

        /// <summary>
        /// Gets the string data that is being parsed.
        /// </summary>
        public string Data { get; private set; }

        /// <summary>
        /// Initializes the token stream with new input text.
        /// </summary>
        /// <param name="text">Input text</param>
        public void Initialize(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException(nameof(text), "Text cannot be null");
            }

            _pos = 0;
            _line = 1;
            _col = 1;

            Data = text;
            CurrentToken = string.Empty;
            
            if (_token == null)
            {
                _token = new StringBuilder();
            }
            else
            {
                _token.Clear();
            }
        }

        /// <summary>
        /// Checks if we have another token left in the token stream.
        /// </summary>
        /// <returns>True if there is a next token, false otherwise.</returns>
        public bool HasNext()
        {
            // Takes care of first/last cases when we're starting out or after we're done, and will reveal
            // if we do have a token left
            if (string.IsNullOrEmpty(CurrentToken))
            {
                GoToNextToken();
            }

            if (GetCharacter(0) == EOF)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Peeks ahead to the next token and checks if it matches with the string sequence. 
        /// This does not advance the current position, and uses InvariantCultureIgnoreCase
        /// as the string comparison.
        /// </summary>
        /// <param name="token">String sequence to check</param>
        /// <returns>True if the next token matches, false otherwise.</returns>
        public bool HasNext(string token)
        {
            return HasNext(token, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Peeks ahead to the next token and checks if it matches with the string sequence. 
        /// This does not advance the current position.
        /// </summary>
        /// <param name="token">String sequence to check</param>
        /// <param name="comparison">Comparison to use</param>
        /// <returns>True if the next token matches, false otherwise.</returns>
        public bool HasNext(string token, StringComparison comparison)
        {
            // If called more than once and NextToken() isn't called, we save the look ahead
            // so we don't have to keep collecting it.
            if (!string.IsNullOrEmpty(_peekToken))
            {
                return _peekToken.Equals(token, comparison);
            }

            int lc = _line;
            int cc = _col;
            int pos = _pos;
            int slc = LineNumber;
            int scc = Column;
            string sstr = CurrentToken;

            string str = NextToken();
            bool result = false;

            if (!string.IsNullOrEmpty(str))
            {
                result = str.Equals(token, comparison);
                _peekToken = str;
            }

            _line = lc;
            _col = cc;
            _pos = pos;
            LineNumber = slc;
            Column = scc;
            CurrentToken = sstr;

            return result;
        }

        /// <summary>
        /// Peeks ahead to the next token and checks if it starts with the string sequence. 
        /// This does not advance the current position, and uses InvariantCultureIgnoreCase
        /// as the string comparison.
        /// </summary>
        /// <param name="token">String sequence to check</param>
        /// <returns>True if the next token matches, false otherwise.</returns>
        public bool HasNextStartsWith(string token)
        {
            return HasNextStartsWith(token, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Peeks ahead to the next token and checks if it starts with the string sequence. 
        /// This does not advance the current position.
        /// </summary>
        /// <param name="token">String sequence to check</param>
        /// <param name="comparison">Comparison to use</param>
        /// <returns>True if the next token matches</returns>
        public bool HasNextStartsWith(string token, StringComparison comparison)
        {
            //If called more than once and NextToken() isn't called, we save the look ahead
            //so we don't have to keep collecting it.
            if (!string.IsNullOrEmpty(_peekToken))
            {
                return _peekToken.StartsWith(token, comparison);
            }

            int lc = _line;
            int cc = _col;
            int pos = _pos;
            int slc = LineNumber;
            int scc = Column;
            string sstr = CurrentToken;

            string str = NextToken();
            bool result = false;

            if (!string.IsNullOrEmpty(str))
            {
                result = str.StartsWith(token, comparison);
                _peekToken = str;
            }

            _line = lc;
            _col = cc;
            _pos = pos;

            LineNumber = slc;
            Column = scc;
            CurrentToken = sstr;

            return result;
        }

        /// <summary>
        /// Parses the next token as a boolean.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool NextBool()
        {
            return bool.Parse(NextToken());
        }

        /// <summary>
        /// Parses the next token as an integer.
        /// </summary>
        /// <returns>Int</returns>
        public int NextInt()
        {
            string token = NextToken();
            if (token.StartsWith("0x", StringComparison.InvariantCulture))
            {
                return int.Parse(token.Substring(2), NumberStyles.HexNumber);
            }

            return int.Parse(token);
        }

        /// <summary>
        /// Parses the next token as a float.
        /// </summary>
        /// <returns>Float</returns>
        public float NextSingle()
        {
            string str = NextToken();
            return float.Parse(str.TrimEnd(_numDelims), CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Gets the next token from the text. If reached the end of the text,
        /// an empty string is returned. Whitespaces, separators, comments,
        /// and control characters are ignored.
        /// </summary>
        /// <returns>The token</returns>
        public string NextToken()
        {
            while (true)
            {
                var c = GetCharacter(0);
                switch (c)
                {
                    case EOF:
                        CurrentToken = string.Empty;
                        return CurrentToken;
                    case ' ':
                    case '\t':
                    case ';':
                    case '\r':
                    case '\n':
                        GoToNextToken();
                        break;
                    case '/':
                        if (!GoToEndOfComment())
                            return ReadToken();
                        else
                            GoToNextToken();

                        break;
                    default:
                        return ReadToken();
                }
            }
        }

        /// <summary>
        /// Reads a complete token, returning when we hit
        /// either the end of the text, a separator, a comment,
        /// or a control character.
        /// </summary>
        /// <returns>The token</returns>
        private string ReadToken()
        {
            StartReading();
            Consume();

            while (true)
            {
                char c = GetCharacter(0);

                switch (c)
                {
                    case EOF:
                    case ' ':
                    case '\t':
                    case '\r':
                    case '\n':
                    case ';':
                        GoToNextToken();
                        CurrentToken = _token.ToString();
                        return CurrentToken;
                    case '/':
                        if (GoToEndOfComment())
                        {
                            GoToNextToken();
                            CurrentToken = _token.ToString();
                            return CurrentToken;
                        }
                        Consume();
                        break;
                    default:
                        Consume();
                        break;
                }
            }
        }

        /// <summary>
        /// Gets the character at the current position, or
        /// subsequent characters. Character will be located at pos + count
        /// </summary>
        /// <param name="count">Number of characters to skip ahead</param>
        /// <returns>Character at the position</returns>
        private char GetCharacter(int count)
        {
            if (_pos + count >= Data.Length)
            {
                return EOF;
            }

            return Data[_pos + count];
        }

        /// <summary>
        /// Prepare to start reading a token.
        /// </summary>
        private void StartReading()
        {
            _token.Clear();
            _peekToken = null;
            Column = _col;
            LineNumber = _line;
        }

        /// <summary>
        /// Consumes a single valid character, and adds it to the current token.
        /// </summary>
        private void Consume()
        {
            _token.Append(Data[_pos]); ;
            _pos++;
            _col++;
        }

        /// <summary>
        /// Skips an invalid character
        /// </summary>
        private void Skip()
        {
            _pos++;
        }

        /// <summary>
        /// Skips a number of invalid characters
        /// </summary>
        /// <param name="count">Number of characters to skip</param>
        private void Skip(int count)
        {
            _pos += count;
        }

        /// <summary>
        /// Go to the beginning of the very next token, skipping over
        /// separators, comments, and control characters.
        /// </summary>
        private void GoToNextToken()
        {
            while (true)
            {
                var c = GetCharacter(0);
                switch (c)
                {
                    case EOF:
                        Skip();
                        return;
                    case ' ':
                    case '\t':
                    case ';':
                        Skip();
                        _col++;
                        break;
                    case '\r':
                        char peek = GetCharacter(1);
                        if (peek == '\n')
                        {
                            Skip();
                        }

                        Skip();
                        _line++;
                        _col = 1;
                        break;
                    case '\n':
                        Skip();
                        _line++;
                        _col = 1;
                        break;
                    case '/':
                        if (GoToEndOfComment())
                            GoToNextToken();

                        return;
                    default:
                        return;
                }
            }
        }

        /// <summary>
        /// Checks if we're at the start of a comment (//), and
        /// if we are skip all the characters until we reach a control character.
        /// </summary>
        /// <returns></returns>
        private bool GoToEndOfComment()
        {
            char dbSlash = GetCharacter(1);
            if (dbSlash == '/')
            {
                Skip(2);
                _col += 2;
            }
            else
            {
                return false;
            }

            while (true)
            {
                char c = GetCharacter(0);
                if (!(c == '\r' || c == '\n'))
                {
                    Skip();
                    _col++;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
