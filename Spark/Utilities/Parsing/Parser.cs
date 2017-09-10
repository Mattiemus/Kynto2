namespace Spark.Utilities.Parsing
{
    using System;
    
    /// <summary>
    /// Parser takes the result from a <see cref="Scanner{TokenT}"/> and converts it into a <see cref="ParseTree{TokenT, NodeT}"/>
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    /// <typeparam name="NodeT">Node type</typeparam>
    public abstract class Parser<TokenT, NodeT> where TokenT : struct
                                                where NodeT : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Parser{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="scanner">Scanner instance</param>
        protected Parser(Scanner<TokenT> scanner)
        {
            Scanner = scanner;
        }

        /// <summary>
        /// Gets the scanner
        /// </summary>
        public Scanner<TokenT> Scanner { get; }

        /// <summary>
        /// Gets the node type of the start node
        /// </summary>
        public abstract NodeT StartNode { get; }

        /// <summary>
        /// Parses an input string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <returns>Resulting parse tree</returns>
        public ParseTree<TokenT, NodeT> Parse(string input)
        {
            return Parse(input, new ParseTree<TokenT, NodeT>(input, StartNode));
        }

        /// <summary>
        /// Parses an input string
        /// </summary>
        /// <param name="input">Input string</param>
        /// <param name="tree">Parse tree to extend from</param>
        /// <returns>Resulting parse tree</returns>
        public ParseTree<TokenT, NodeT> Parse(string input, ParseTree<TokenT, NodeT> tree)
        {
            Scanner.Initialize(input);
            ParseStart(tree);
            return tree;
        }

        /// <summary>
        /// Entrypoint for the parser
        /// </summary>
        /// <param name="node">Parent node</param>
        protected abstract void ParseStart(ParseNode<TokenT, NodeT> node);

        /// <summary>
        /// Expects the next token to be of a specific type
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="tokenType">Expected token type</param>
        protected void ExpectToken(ParseNode<TokenT, NodeT> node, TokenT tokenType)
        {
            Token<TokenT> token = Scanner.Scan(tokenType);
            node.CreateNode(token);
            if (!token.TokenType.Equals(tokenType))
            {
                throw new SparkParseException($"Unexpected token '{token.Text.Replace("\n", "")}' found. Expected '{tokenType.ToString()}'");
            }
        }

        /// <summary>
        /// Calls a parser that creates a given node type
        /// </summary>
        /// <param name="node">Parent node</param>
        /// <param name="nodeType">Type that the node parses</param>
        /// <param name="parseAction">Action that parses from the current location into the newly created node</param>
        protected void ParseWith(ParseNode<TokenT, NodeT> node, NodeT nodeType, Action<ParseNode<TokenT, NodeT>> parseAction)
        {
            ParseNode<TokenT, NodeT> nextNode = node.CreateNode(nodeType);
            parseAction(nextNode);
        }
    }
}
