using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    interface IGraphTraverseAlgorithm
    {
        IGraph Execute(INode _StartNode);
    }
}