using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

using Priority_Queue;

namespace GraphLibrary.Algorithm
{
    class Dijkstra
    {
        private IGraph FUsedGraph;

        public Dijkstra(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }


        public void Execute(int _SourceNodeId, int _TargetNodeId)
        {
            Console.WriteLine("Start Dijkstra");
            try
            {
                foreach (var hEdge in FUsedGraph.GetEdgeIndices())
                {
                    if (hEdge.GetWeightValue() < 0.0)
                    {
                        throw new DijkastraException("Negative Kantengewichte gefunden. Der Dijkstra-Algorithmus kann nicht mit mit negativen Werten umgehen.");
                    }
                }

                var hNodeDictionary = FUsedGraph.GetNodeDictionary();
                var hStartNode = hNodeDictionary[_SourceNodeId];
                var hTargetNode = hNodeDictionary[_TargetNodeId];
                bool hTargetFound = false;

                var hCostDictionary = new Dictionary<INode, double>();
                var hParentNodeEdge = new Dictionary<INode, Edge>(); // Speichert mit welcher Kante ein Knoten erreicht wurde
                var hNodePriorityQueue = new SimplePriorityQueue<INode, double>();
                var hVisitedNodes = new HashSet<INode>();

                // Initialisierung
                foreach (var hNode in hNodeDictionary.Values)
                {
                    hParentNodeEdge.Add(hNode, null);
                    hCostDictionary.Add(hNode, Double.PositiveInfinity);
                }

                hNodePriorityQueue.Enqueue(hStartNode, 0.0);
                hCostDictionary[hStartNode] = 0.0;


                while (!hTargetFound)
                {
                    var hCurrentNode = hNodePriorityQueue.Dequeue();
                    hVisitedNodes.Add(hCurrentNode);

                    if (hCurrentNode == hTargetNode)
                    {
                        hTargetFound = true;
                        // break
                    }

                    foreach (var hNeighborEdge in hCurrentNode.NeighbourEdges)
                    {
                        if (!hVisitedNodes.Contains(hNeighborEdge.Node))
                        {
                            // Nachbar ist noch nicht besucht worden
                            var hTourCostToNeighbor = hCostDictionary[hCurrentNode] + hNeighborEdge.Edge.GetWeightValue();

                            // Ist der Nachbar schon in der Priority Queue drin und hab einen besseren Weg gefunden?
                            if (hNodePriorityQueue.Contains(hNeighborEdge.Node) && (hTourCostToNeighbor < hNodePriorityQueue.GetPriority(hNeighborEdge.Node)))
                            {
                                hNodePriorityQueue.UpdatePriority(hNeighborEdge.Node, hTourCostToNeighbor);
                                hParentNodeEdge[hNeighborEdge.Node] = hNeighborEdge.Edge;
                                hCostDictionary[hNeighborEdge.Node] = hTourCostToNeighbor;
                            }
                            else if (!hNodePriorityQueue.Contains(hNeighborEdge.Node))
                            {
                                // Nachbarknoten wurde noch garnicht bemerkt. Also mit den gerade gefunden Kosten in die Priority Queue
                                hNodePriorityQueue.Enqueue(hNeighborEdge.Node, hTourCostToNeighbor);
                                hParentNodeEdge[hNeighborEdge.Node] = hNeighborEdge.Edge;
                                hCostDictionary[hNeighborEdge.Node] = hTourCostToNeighbor;
                            }
                        }
                    }
                }

                // Jetzt vom Zielknonten zurück zum Startknoten ;)
                var hTmpNode = hTargetNode;
                var hShortestPathStack = new Stack<int>();
                var hCosts = 0.0;
                while (hTmpNode != hStartNode)
                {
                    hCosts += hParentNodeEdge[hTmpNode].GetWeightValue();
                    hShortestPathStack.Push(hTmpNode.Id);

                    // "Knoten davor"
                    if (hParentNodeEdge[hTmpNode] is DirectedEdge)
                    {
                        DirectedEdge hEdge = (DirectedEdge)hParentNodeEdge[hTmpNode];
                        hTmpNode = hEdge.GetEdgeSource();
                    }
                    else if (hParentNodeEdge[hTmpNode] is UndirectedEdge)
                    {
                        UndirectedEdge hEdge = (UndirectedEdge)hParentNodeEdge[hTmpNode];
                        hTmpNode = hParentNodeEdge[hTmpNode].GetOtherEndpoint(hTmpNode);
                    }
                }

                hShortestPathStack.Push(hStartNode.Id);

                // Ausgabe
                Console.WriteLine("Kürzeste Route:\t" + string.Join(",", hShortestPathStack));
                Console.WriteLine("Kosten:\t" + hCosts);
            }
            catch (DijkastraException ex)
            {
                Console.WriteLine(ex.Message);
            }

        }  // Execute
    }
    internal class DijkastraException : Exception
    {
        public DijkastraException(string _Message)
            : base(_Message)
        {
        }
    }


}
