using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    /// <summary>
    /// Führt auf dem Graphen eine Tiefensuche durch. 
    /// </summary>
    class DepthFirstSearch : IGraphTraverseAlgorithm
    {
        private INode FStartNode;

        private IGraph FDfsGraph;

        public DepthFirstSearch()
        {
        }

        
        private HashSet<int> FVisitedBfsNodes = new HashSet<int>();

        /// <summary>
        /// Ausführung der Tiefensuche
        /// </summary>
        /// <param name="_StartNode"></param>
        /// <returns>Ein neues Graph Objekt welches die Tiefensuche repräsentiert. Die Knoten und Kanten Referenzen zeigen auf den übergebenen Graphen.
        /// Es handelt sich um eine Art Overlay über den alten Graphen</returns>
        public IGraph Execute(INode _StartNode)
        {
            FStartNode = _StartNode;
            FDfsGraph = new Graph();
            FDfsGraph.TryToAddNode(FStartNode);

            FVisitedBfsNodes.Clear();
            DfsRecursive(FStartNode);
            return FDfsGraph;
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
                    FDfsGraph.TryToAddNode(hNeighbourEdge.Node);
                    FDfsGraph.AddEdge(hNeighbourEdge.Edge);
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
