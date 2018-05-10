using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class DepthFirstSearch : IGraphTraverseAlgorithm
    {
        private INode FStartNode;

        private IGraph FSubGraph;

        public DepthFirstSearch()
        {
        }

        
        private HashSet<int> FVisitedBfsNodes = new HashSet<int>();

        public IGraph Execute(INode _StartNode)
        {
            FStartNode = _StartNode;
            FSubGraph = new Graph();
            FSubGraph.AddNode(FStartNode);

            FVisitedBfsNodes.Clear();
            DfsRecursive(FStartNode);
            return FSubGraph;
        }

        public void DfsRecursive(INode _Node)
        {
            FVisitedBfsNodes.Add(_Node.Id);
            var hNodeNeighboursConnection = _Node.NeighbourEdges;
            
            foreach (var hNeighbourEdge in hNodeNeighboursConnection)
            {
                var hNeighbourNode = hNeighbourEdge.Node;
                if (!FVisitedBfsNodes.Contains(hNeighbourNode.Id))
                {
                    FSubGraph.AddNode(hNeighbourEdge.Node);
                    FSubGraph.AddEdge(hNeighbourEdge.Edge);
                    DfsRecursive(hNeighbourNode);
                }
            }
        }

        public void PrintInfosToConsole()
        {
            throw new System.NotImplementedException();
        }
    }
}
