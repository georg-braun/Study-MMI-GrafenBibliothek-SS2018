using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class BreadthFirstSearch : IGraphAlgorithm
    {
        public IGraph UsedGraph { get; }

        private Dictionary<int, List<int>> FSubGraphs;

        public BreadthFirstSearch(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
            FSubGraphs = new Dictionary<int, List<int>>();
        }

        private void AddNodeIdToSubgraph(int _SubgraphId, int _NodeId)
        {
            if (!FSubGraphs.ContainsKey(_SubgraphId)) //ToDo
            {
                FSubGraphs.Add(_SubgraphId,new List<int>());
            }
            FSubGraphs[_SubgraphId].Add(_NodeId);
        }


        public void Execute()
        {
            var hGraphNodes = UsedGraph.GetNodeIndices();
            var hVisitedBfsNodes = new HashSet<int>();
            var hUnvisitedGraphNodes = new LinkedList<int>(); // ToDo: Vielleicht besser HashSet. Remove O(1)

            FSubGraphs.Clear();
            var hSubGraphId = 0;

            foreach (var hNode in hGraphNodes)
            {
                hUnvisitedGraphNodes.AddLast(hNode.Key);
            }

            while (hUnvisitedGraphNodes.Count > 0)
            {
                
                var hBfsQueue = new Queue<int>();
                hBfsQueue.Enqueue(hUnvisitedGraphNodes.First.Value);

                while (hBfsQueue.Count > 0)
                {
                    var hCurrentNodeId = hBfsQueue.Dequeue();

                    hVisitedBfsNodes.Add(hCurrentNodeId);       // ToDo: Visited bei der NachbarKnoten Prüfung
                    hUnvisitedGraphNodes.Remove(hCurrentNodeId);
                    AddNodeIdToSubgraph(hSubGraphId, hCurrentNodeId);

                    var hNeighbourNodesIds = hGraphNodes[hCurrentNodeId].GetNeighbourIds();
                    
                    foreach (var hNeighbourNodesId in hNeighbourNodesIds)
                    {
                        // Besuche nur weitere Knoten wenn diese noch nicht besucht wurden noch noch nicht in der Queue sind
                        if (!hVisitedBfsNodes.Contains(hNeighbourNodesId) && (!hBfsQueue.Contains(hNeighbourNodesId)))
                        {
                            hBfsQueue.Enqueue(hNeighbourNodesId);
                        }
                    }
                }
                hSubGraphId++;
            }
        }

        public void PrintInfosToConsole()
        {
            Console.WriteLine("--- Breitensuche ---");
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
