namespace Nemonuri.Study.CSharpSyntaxRewriter2;

internal static class SyntaxNodeTheory
{
    private static readonly Walker _walker = new();

    public static IEnumerable<SyntaxNode> GetDescendantNodes
    (
        this SyntaxNode node,
        SyntaxNodePredicate predicate,
        bool includeSelf = false
    )
    {
        return _walker.GetDescendantNodes(node, predicate, includeSelf);
    }

    public static IEnumerable<TSyntaxNode> GetDescendantNodes<TSyntaxNode>
    (
        this SyntaxNode node,
        bool includeSelf = false
    )
        where TSyntaxNode : SyntaxNode
    {
        return node.GetDescendantNodes(static n => n is TSyntaxNode, includeSelf).OfType<TSyntaxNode>();
    }

    private class Walker : CSharpSyntaxWalker
    {
        private SyntaxNode? _rootNode = null;
        private List<SyntaxNode>? _syntaxNodes = null;
        private SyntaxNodePredicate? _predicate = null;
        private bool _includeSelf = false;

        public Walker() : base(SyntaxWalkerDepth.Node)
        { }

        public IEnumerable<SyntaxNode> GetDescendantNodes
        (
            SyntaxNode rootNode,
            SyntaxNodePredicate predicate,
            bool includeSelf
        )
        {
            _rootNode = rootNode;
            _syntaxNodes = new();
            _predicate = predicate;
            _includeSelf = includeSelf;

            Visit(rootNode);

            var result = _syntaxNodes;
            _rootNode = null;
            _syntaxNodes = null;
            _predicate = null;

            return result;
        }

        public override void DefaultVisit(SyntaxNode node)
        {
            if (_predicate is not null && _syntaxNodes is not null)
            {
                if (_rootNode != node || _includeSelf)
                {
                    if (_predicate(node))
                    {
                        _syntaxNodes.Add(node);
                        return;
                    }
                }
            }

            base.DefaultVisit(node);
        }
    }
}

public delegate bool SyntaxNodePredicate(SyntaxNode node);
