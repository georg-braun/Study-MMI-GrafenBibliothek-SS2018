using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class BreadthFirstSearch : IGraphTraverseAlgorithm
    {
        private INode FStartNode;
        
        

        public BreadthFirstSearch()
        {
            
        }




        public IGraph Execute(INode _StartNode)
        {
            FStartNode = _StartNode;
            return BreadthFirstSearchAlgorithm(FStartNode);
        }

        private IGraph BreadthFirstSearchAlgorithm(INode _Node)
        {
            var hSubGraph = new Graph();
            hSubGraph.AddNode(_Node);

            var FVisitedBfsNodes = new HashSet<int>();
            var hBfsQueue = new Queue<INode>();
            hBfsQueue.Enqueue(_Node);
            FVisitedBfsNodes.Add(_Node.Id);
            
            while (hBfsQueue.Count > 0)
            {
                var hCurrentNode = hBfsQueue.Dequeue();
                var hNeighbourEdges = hCurrentNode.NeighbourEdges;

                foreach (var hNeighbourEdge in hNeighbourEdges)
                {
                    // Besuche nur weitere Knoten wenn diese noch nicht besucht wurden
                    if (!FVisitedBfsNodes.Contains(hNeighbourEdge.Node.Id))
                    {
                        FVisitedBfsNodes.Add(hNeighbourEdge.Node.Id);
                        hBfsQueue.Enqueue(hNeighbourEdge.Node);
                        hSubGraph.AddNode(hNeighbourEdge.Node);
                        hSubGraph.AddEdge(hNeighbourEdge.Edge);
                    }
                }
            }
            return hSubGraph;
        }



        public void PrintInfosToConsole()
        {
            throw new System.NotImplementedException();
        }
    }
}
