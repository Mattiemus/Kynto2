namespace Spark.Utilities.Parsing
{
    using System.Collections.Generic;

    /// <summary>
    /// Representation of a node within a parse tree
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    /// <typeparam name="NodeT">Node type</typeparam>
    public class ParseNode<TokenT, NodeT>
    {
        private readonly List<ParseNode<TokenT, NodeT>> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode{TokenT, NodeT}"/> class
        /// </summary>
        protected ParseNode()
        {
            _children = new List<ParseNode<TokenT, NodeT>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="parent">Parent node</param>
        /// <param name="token">Child token</param>
        protected ParseNode(ParseNode<TokenT, NodeT> parent, Token<TokenT> token)
        {
            Parent = parent;
            Token = token;
            _children = new List<ParseNode<TokenT, NodeT>>();
        }

        /// <summary>
        /// Gets the parent node
        /// </summary>
        public ParseNode<TokenT, NodeT> Parent { get; }

        /// <summary>
        /// Gets the token
        /// </summary>
        public Token<TokenT> Token { get; }

        /// <summary>
        /// Gets the child nodes
        /// </summary>
        public IReadOnlyList<ParseNode<TokenT, NodeT>> Children => _children;

        /// <summary>
        /// Creates a child node
        /// </summary>
        /// <param name="token">Node token</param>
        /// <returns>Created child node</returns>
        public ParseNode<TokenT, NodeT> CreateNode(Token<TokenT> token)
        {
            ParseNode<TokenT, NodeT> node = new ParseNode<TokenT, NodeT>(this, token);
            _children.Add(node);
            return node;
        }
    }
}
