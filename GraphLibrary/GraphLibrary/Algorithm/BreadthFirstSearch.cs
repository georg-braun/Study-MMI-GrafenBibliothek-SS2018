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

        private Dictionary<int, NodeEdge> FParentNodeEdges; // Speichert den "Vaterknoten" und dessen Kante

        public BreadthFirstSearch()
        {
            
        }

        public IGraph Execute(INode _StartNode)
        {
            FStartNode = _StartNode;
            return BreadthFirstSearchAlgorithm<CostWeighted>(FStartNode);
        }

        public IGraph Execute<T>(INode _StartNode) where T : IWeight, new()
        {
            FStartNode = _StartNode;
            return BreadthFirstSearchAlgorithm<T>(FStartNode);
        }



        private IGraph BreadthFirstSearchAlgorithm<T>(INode _Node) where T : IWeight, new()
        {
            FParentNodeEdges = new Dictionary<int, NodeEdge>();
            var hSubGraph = new Graph();
            hSubGraph.CreateNewNode(_Node.Id);

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
                        var hNewNode = hSubGraph.CreateNewNode(hNeighbourEdge.Node.Id);
                        Edge hNewEdge;
                        if (hNeighbourEdge.Edge.IsWeighted())
                        {
                            var hNewWeight = new T();
                            hNewWeight.SetValue(hNeighbourEdge.Edge.GetWeightValue<T>());
                            hNewEdge = hSubGraph.CreateUndirectedEdge(hCurrentNode.Id, hNewNode.Id, hNewWeight);
                        }
                        else
                        {
                            hNewEdge = hSubGraph.CreateUndirectedEdge(hCurrentNode.Id, hNewNode.Id);
                        }
                        FParentNodeEdges.Add(hNewNode.Id, new NodeEdge(hCurrentNode, hNewEdge));
                    }
                }
            }

            hSubGraph.UpdateNeighbourInfoInNodes();
            return hSubGraph;
        }

        public Dictionary<int, NodeEdge> GetParentNodeEdge()
        {
            return FParentNodeEdges;
        }


        public void PrintInfosToConsole()
        {
            throw new System.NotImplementedException();
        }
    }
}
