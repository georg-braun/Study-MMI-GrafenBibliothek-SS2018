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
            var hGraphNodes = UsedGraph.GetNodeIndices();
            FVisitedBfsNodes.Clear();
            FSubGraphs.Clear();
            FSubGraphId = 0;

            foreach (var hNode in hGraphNodes)
            {
                FVisitedBfsNodes.Add(hNode.Key,false);
            }

            while (FVisitedBfsNodes.Values.Contains(false))
            {
                FSubGraphs.Add(FSubGraphId, new List<int>());
                var hUnvisitedNode = hGraphNodes[FindUnvisitedNode(FVisitedBfsNodes)];
                BreadthFirstSearchAlgorithm(hUnvisitedNode);
                FSubGraphId++;
            }
        }

        private IGraph BreadthFirstSearchAlgorithm(INode _Node)
        {
            // ToDo Erstelle ein neues Graphen Objekt
            // ToDo Füge den übergebenen Knoten hinzu (als Referenz auf den Original Knoten)

            var hBfsQueue = new Queue<INode>();
            hBfsQueue.Enqueue(_Node);
            FVisitedBfsNodes[_Node.Id] = true;
            AddNodeIdToSubgraph(_Node.Id);

            while (hBfsQueue.Count > 0)
            {
                var hCurrentNode = hBfsQueue.Dequeue();
                // ToDo Umbauen dass auch die Kanten Objekte geholt werden.
                var hNeighbourEdges = hCurrentNode.NeighboursEdges;

                foreach (var hNeighbourEdge in hNeighbourEdges)
                {
                    // Besuche nur weitere Knoten wenn diese noch nicht besucht wurden
                    if (!FVisitedBfsNodes[hNeighbourEdge.Node.Id])
                    {
                        FVisitedBfsNodes[hNeighbourEdge.Node.Id] = true;
                        AddNodeIdToSubgraph(hNeighbourEdge.Node.Id);
                        hBfsQueue.Enqueue(hNeighbourEdge.Node);

                        // ToDo Es wurde ein neuer Knoten entdeckt. Speicher auch dessen Referenz
                        // ToDo Speicher ebenfalls die Kante der Verbindung
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
