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
            var hVisitedBfsNodes = new Dictionary<int,bool>();
            FSubGraphs.Clear();
            var hSubGraphId = 0;

            foreach (var hNode in hGraphNodes)
            {
                hVisitedBfsNodes.Add(hNode.Key,false);
            }

            while (hVisitedBfsNodes.Values.Contains(false))
            {
                
                var hBfsQueue = new Queue<int>();
                var hUnvisitedNodeId = FindUnvisitedNode(hVisitedBfsNodes);
                hBfsQueue.Enqueue(hUnvisitedNodeId);
                hVisitedBfsNodes[hUnvisitedNodeId] = true;
                AddNodeIdToSubgraph(hSubGraphId, hUnvisitedNodeId);

                while (hBfsQueue.Count > 0)
                {
                    var hCurrentNodeId = hBfsQueue.Dequeue();
                    

                    var hNeighbourNodesIds = hGraphNodes[hCurrentNodeId].GetNeighbourIds();
                    
                    foreach (var hNeighbourNodesId in hNeighbourNodesIds)
                    {
                        // Besuche nur weitere Knoten wenn diese noch nicht besucht wurden
                        if (!hVisitedBfsNodes[hNeighbourNodesId])
                        {
                            hVisitedBfsNodes[hNeighbourNodesId] =true;
                            AddNodeIdToSubgraph(hSubGraphId, hNeighbourNodesId);
                            hBfsQueue.Enqueue(hNeighbourNodesId);
                        }
                    }
                }
                hSubGraphId++;
            }
        }

        private int FindUnvisitedNode(Dictionary<int, bool> dict)
        {
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                if (dict[i] == false)
                {
                    return i;
                }
            } //for (int i = 0; i < UPPER; i++)

            return -1;
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
