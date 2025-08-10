namespace Nemonuri.Trees;

public interface IChildrenProvider<TNode>
{ 
    IEnumerable<TNode> GetChildren(TNode source);
}
