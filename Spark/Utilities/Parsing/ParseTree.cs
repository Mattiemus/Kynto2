namespace Spark.Utilities.Parsing
{
    /// <summary>
    /// Representation of a parse tree
    /// </summary>
    /// <typeparam name="TokenT">Token type</typeparam>
    /// <typeparam name="NodeT">Node type</typeparam>
    public class ParseTree<TokenT, NodeT> : ParseNode<TokenT, NodeT> where NodeT : struct
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParseTree{TokenT, NodeT}"/> class
        /// </summary>
        /// <param name="source">Source input string</param>
        /// <param name="nodeType">Node type</param>
        internal ParseTree(string source, NodeT nodeType)
            : base(source, nodeType)
        {
        }
}
}
