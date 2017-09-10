namespace Spark.Utilities.Parsing
{
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// Representation of a node within a parse tree
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    /// <typeparam name="NodeT">Node type</typeparam>
    public class ParseNode<TokenT, NodeT> where NodeT : struct
    {
        private readonly List<ParseNode<TokenT, NodeT>> _children;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="source">Source input string</param>
        /// <param name="nodeType">Node type</param>
        protected ParseNode(string source, NodeT nodeType)
        {
            _children = new List<ParseNode<TokenT, NodeT>>();

            FullInput = source;
            NodeType = nodeType;
            Token = null;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="source">Source input string</param>
        /// <param name="parent">Parent node</param>
        /// <param name="token">Child token</param>
        protected ParseNode(string source, ParseNode<TokenT, NodeT> parent, Token<TokenT> token)
        {
            _children = new List<ParseNode<TokenT, NodeT>>();

            Parent = parent;
            Token = token;
            FullInput = source;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParseNode{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="source">Source input string</param>
        /// <param name="parent">Parent node</param>
        /// <param name="nodeType">Node type</param>
        protected ParseNode(string source, ParseNode<TokenT, NodeT> parent, NodeT nodeType)
        {
            _children = new List<ParseNode<TokenT, NodeT>>();

            Parent = parent;
            NodeType = nodeType;
            FullInput = source;
        }

        /// <summary>
        /// Gets the parent node
        /// </summary>
        public ParseNode<TokenT, NodeT> Parent { get; }
        
        /// <summary>
        /// Gets the child nodes
        /// </summary>
        public IReadOnlyList<ParseNode<TokenT, NodeT>> Children => _children;

        /// <summary>
        /// Gets a value indicating whether the node is a leaf node
        /// </summary>
        public bool IsLeaf => Token != null;

        /// <summary>
        /// Gets the node type
        /// </summary>
        public NodeT NodeType { get; }

        /// <summary>
        /// Gets the token if this node is a leaf
        /// </summary>
        public Token<TokenT> Token { get; }

        /// <summary>
        /// Gets the full text which the node was extracted from
        /// </summary>
        public string FullInput { get; }

        /// <summary>
        /// Gets the text that represents the token
        /// </summary>
        public string Text => FullInput.Substring(StartPosition, Length);

        /// <summary>
        /// Gets the start position of the node
        /// </summary>
        public int StartPosition => Token == null ? Children.Min(c => c.StartPosition) : Token.StartPosition;

        /// <summary>
        /// Gets the end position of the node
        /// </summary>
        public int EndPosition => Token == null ? Children.Max(c => c.EndPosition) : Token.EndPosition;

        /// <summary>
        /// Gets the length of the node text
        /// </summary>
        public int Length => EndPosition - StartPosition;

        /// <summary>
        /// Creates a child node
        /// </summary>
        /// <param name="token">Node type</param>
        /// <returns>Created child node</returns>
        public ParseNode<TokenT, NodeT> CreateNode(NodeT nodeType)
        {
            ParseNode<TokenT, NodeT> node = new ParseNode<TokenT, NodeT>(FullInput, this, nodeType);
            _children.Add(node);
            return node;
        }

        /// <summary>
        /// Creates a child leaf node
        /// </summary>
        /// <param name="token">Node token</param>
        /// <returns>Created child node</returns>
        public ParseNode<TokenT, NodeT> CreateNode(Token<TokenT> token)
        {
            ParseNode<TokenT, NodeT> node = new ParseNode<TokenT, NodeT>(FullInput, this, token);
            _children.Add(node);
            return node;
        }

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
