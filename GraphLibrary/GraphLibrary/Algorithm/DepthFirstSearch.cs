using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class DepthFirstSearch : IGraphAlgorithm
    {
        public IGraph UsedGraph { get; }

        private Dictionary<int, List<int>> FSubGraphs;

        public DepthFirstSearch(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
            FSubGraphs = new Dictionary<int, List<int>>();
        }

        private int FSubgraphId = 0;
        private void AddNodeIdToSubgraph( int _NodeId)
        {
            if (!FSubGraphs.ContainsKey(FSubgraphId))
            {
                FSubGraphs.Add(FSubgraphId, new List<int>());
            }
            FSubGraphs[FSubgraphId].Add(_NodeId);
        }


        private HashSet<int> FVisitedBfsNodes = new HashSet<int>();
        private HashSet<int> FUnvisitedGraphNodes = new HashSet<int>();

        public void Execute()
        {
            var hGraphNodes = UsedGraph.GetNodeIndices();
            FVisitedBfsNodes.Clear();
            FUnvisitedGraphNodes.Clear();
            FSubGraphs.Clear();

            FSubgraphId = 0;

            foreach (var hNode in hGraphNodes)
            {
                FUnvisitedGraphNodes.Add(hNode.Key);
            }

            while (FUnvisitedGraphNodes.Count > 0)
            {
                var hUnvisitedGraphNodeId = FUnvisitedGraphNodes.First();
                var hUnvisitedGraphNode = hGraphNodes[hUnvisitedGraphNodeId];
                DfsRecursive(hUnvisitedGraphNode);
                FSubgraphId++;
            }
        }

        public void DfsRecursive(INode _Node)
        {
            FVisitedBfsNodes.Add(_Node.Id);
            FUnvisitedGraphNodes.Remove(_Node.Id);
            AddNodeIdToSubgraph(_Node.Id);
            var hNodeNeighboursConnection = _Node.NeighboursEdges;
            
            foreach (var hCurrentConnection in hNodeNeighboursConnection)
            {
                var hNeighbourNode = hCurrentConnection.Node;
                if (!FVisitedBfsNodes.Contains(hNeighbourNode.Id))
                {
                    DfsRecursive(hNeighbourNode);
                }
            }
        }

        public void PrintInfosToConsole()
        {
            Console.WriteLine("--- Tiefensuche ---");
            Console.WriteLine("Anzahl der Knoten: " + UsedGraph.GetNodeIndices().Count.ToString());
            Console.WriteLine("Anzahl der Kanten: " + UsedGraph.GetEdgeIndices().Count.ToString());
            Console.WriteLine("Anzahl der Teilgrafen: " + FSubGraphs.Keys.Count.ToString());
            foreach (var hSubGraphList in FSubGraphs)
            {
                Console.WriteLine("\tTeilgraf " + hSubGraphList.Key.ToString()
                                                + ": " + hSubGraphList.Value.Count);
            }
        }
    }
}
