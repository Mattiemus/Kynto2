namespace Spark.Utilities.Parsing
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using Core;

    /// <summary>
    /// Scanner for extracting a stream of tokens from an input block of text
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    public abstract class Scanner<TokenT> where TokenT : struct
    {
        private readonly Dictionary<TokenT, Regex> _patterns;
        private readonly List<TokenT> _tokens;
        private readonly List<TokenT> _skipList;
        private string _input;
        private int _startPos;
        private int _endPos;
        private Token<TokenT> _lookahead;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scanner{TokenT}"/> class
        /// </summary>
        protected Scanner()
        {
            _patterns = new Dictionary<TokenT, Regex>();
            _tokens = new List<TokenT>();
            _skipList = new List<TokenT>();
        }

        /// <summary>
        /// Initializes the scanner
        /// </summary>
        /// <param name="input">Input to initialize with</param>
        public void Initialize(string input)
        {
            _input = input;
            _startPos = 0;
            _endPos = 0;
            _lookahead = null;
        }

        /// <summary>
        /// Advances to the next token
        /// </summary>
        /// <returns>Next token in the input stream</returns>
        public Token<TokenT> Scan()
        {
            ThrowIfNotInitialized();
            return Scan(_tokens.ToArray());
        }

        /// <summary>
        /// Advances to the next token
        /// </summary>
        /// <param name="expectedTokens">List of expected tokens</param>
        /// <returns>Next token in the input stream</returns>
        public Token<TokenT> Scan(params TokenT[] expectedTokens)
        {
            ThrowIfNotInitialized();

            Token<TokenT> tok = LookAhead(expectedTokens);
            _lookahead = null;
            _startPos = tok.EndPosition;
            _endPos = tok.EndPosition;
            return tok;
        }

        /// <summary>
        /// Returns the next token without advancing the scanner
        /// </summary>
        /// <returns>Next token in the input stream</returns>
        public Token<TokenT> LookAhead()
        {
            ThrowIfNotInitialized();
            return LookAhead(_tokens.ToArray());
        }

        /// <summary>
        /// Returns the next token without advancing the scanner
        /// </summary>
        /// <param name="expectedTokens">Next token to search for</param>
        /// <returns>Next token in the input stream</returns>
        public Token<TokenT> LookAhead(params TokenT[] expectedTokens)
        {
            ThrowIfNotInitialized();

            if (expectedTokens == null || expectedTokens.Length == 0)
            {
                throw new ArgumentNullException(nameof(expectedTokens));
            }

            if (_lookahead != null)
            {
                return _lookahead;
            }

            TokenT[] searchingTokens = expectedTokens.Concat(_skipList)
                                                     .Distinct()
                                                     .ToArray();
            
            int startPos = _startPos;
            int endPos = startPos;
            TokenT? nextTokenType = null;
            do
            {
                int len = -1;
                foreach (TokenT tokenType in searchingTokens)
                {
                    Regex r = _patterns[tokenType];
                    Match m = r.Match(_input, startPos);
                    if (m.Success && m.Index == startPos && m.Length >= len)
                    {
                        len = m.Length;
                        nextTokenType = tokenType;
                    }
                }

                if (nextTokenType.HasValue && len >= 0)
                {
                    endPos = startPos + len;
                }

                if (nextTokenType.HasValue && _skipList.Contains(nextTokenType.Value))
                {
                    startPos = endPos;
                }
            }
            while (nextTokenType.HasValue && _skipList.Contains(nextTokenType.Value));

            if (nextTokenType.HasValue)
            {
                _lookahead = new Token<TokenT>(nextTokenType.Value, _input, startPos, endPos);
            }
            else
            {
                _lookahead = null;
            }

            return _lookahead;
        }
        
        /// <summary>
        /// Adds a token to the skip list
        /// </summary>
        /// <param name="tokenType">Type of token to skip</param>
        protected void RegisterSkipToken(TokenT tokenType)
        {
            _skipList.Add(tokenType);
        }

        /// <summary>
        /// Registers a token and regex pair
        /// </summary>
        /// <param name="tokenType">Type of token</param>
        /// <param name="tokenRegex">Token identification regex</param>
        protected void RegisterToken(TokenT tokenType, string tokenRegex)
        {
            Regex regex = new Regex(tokenRegex, RegexOptions.Compiled);
            _patterns.Add(tokenType, regex);
            _tokens.Add(tokenType);
        }

        /// <summary>
        /// Throws an exception if the scacnner has not been initialized with input
        /// </summary>
        private void ThrowIfNotInitialized()
        {
            if (_input == null)
            {
                throw new SparkException("Scanner has not been initialized");
            }
        }
    }
}
