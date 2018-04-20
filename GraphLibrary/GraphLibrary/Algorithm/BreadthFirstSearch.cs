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

        private Dictionary<int, bool> FVisitedBfsNodes;

        private Dictionary<int, INode> FGraphNodes;

        int FSubGraphId = 0;

        public BreadthFirstSearch(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
            FSubGraphs = new Dictionary<int, List<int>>();
            FVisitedBfsNodes = new Dictionary<int, bool>();
        }

        private void AddNodeIdToSubgraph(int _NodeId)
        {
            FSubGraphs[FSubGraphId].Add(_NodeId);
        }


        public void Execute()
        {
            FGraphNodes = UsedGraph.GetNodeIndices();
            FVisitedBfsNodes.Clear();
            FSubGraphs.Clear();
            FSubGraphId = 0;

            foreach (var hNode in FGraphNodes)
            {
                FVisitedBfsNodes.Add(hNode.Key,false);
            }

            while (FVisitedBfsNodes.Values.Contains(false))
            {
                FSubGraphs.Add(FSubGraphId, new List<int>());
                var hUnvisitedNode = FGraphNodes[FindUnvisitedNode(FVisitedBfsNodes)];
                BreadthFirstSearchAlgorithm(hUnvisitedNode);
                FSubGraphId++;
            }
        }

        private IGraph BreadthFirstSearchAlgorithm(INode _Node)
        {
            var hBfsQueue = new Queue<int>();
            hBfsQueue.Enqueue(_Node.Id);
            FVisitedBfsNodes[_Node.Id] = true;
            AddNodeIdToSubgraph(_Node.Id);

            while (hBfsQueue.Count > 0)
            {
                var hCurrentNodeId = hBfsQueue.Dequeue();


                var hNeighbourNodesIds = FGraphNodes[hCurrentNodeId].GetNeighbourIds();

                foreach (var hNeighbourNodesId in hNeighbourNodesIds)
                {
                    // Besuche nur weitere Knoten wenn diese noch nicht besucht wurden
                    if (!FVisitedBfsNodes[hNeighbourNodesId])
                    {
                        FVisitedBfsNodes[hNeighbourNodesId] = true;
                        AddNodeIdToSubgraph(hNeighbourNodesId);
                        hBfsQueue.Enqueue(hNeighbourNodesId);
                    }
                }
            }

            return null;
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
