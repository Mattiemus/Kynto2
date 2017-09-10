namespace Spark.Utilities.Parsing
{
    /// <summary>
    /// Representation of a token
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    public sealed class Token<TokenT>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Token{TokenT}"/> class
        /// </summary>
        /// <param name="tokenType">Token type</param>
        /// <param name="fullInput">Full input the token was extracted from</param>
        /// <param name="startPos">Token start position</param>
        /// <param name="endPos">Token end position</param>
        public Token(TokenT tokenType, string fullInput, int startPos, int endPos)
        {
            TokenType = tokenType;
            FullInput = fullInput;
            StartPosition = startPos;
            EndPosition = endPos;
        }

        /// <summary>
        /// Gets the token type
        /// </summary>
        public TokenT TokenType { get; }

        /// <summary>
        /// Gets the full text which the token was extracted from
        /// </summary>
        public string FullInput { get; }

        /// <summary>
        /// Gets the text that represents the token
        /// </summary>
        public string Text => FullInput.Substring(StartPosition, Length);

        /// <summary>
        /// Gets the start position of the token
        /// </summary>
        public int StartPosition { get; }

        /// <summary>
        /// Gets the end position of the token
        /// </summary>
        public int EndPosition { get; }

        /// <summary>
        /// Gets the length of the token text
        /// </summary>
        public int Length => EndPosition - StartPosition;

        /// <summary>
        /// Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="string" /> that represents this instance.</returns>
        public override string ToString()
        {
            return Text;
        }
    }
}
