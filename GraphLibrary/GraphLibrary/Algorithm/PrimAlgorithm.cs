using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class PrimAlgorithm
    {

        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;
    

        public PrimAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        public void Execute()
        {
            FStopwatch.Start();

            // Annahme: Der übergebene Graf ist zusammenhängend 

            var hMinimalSpanningTree = new Graph();
            var hNodeDictionary = FUsedGraph.GetNodeIndices();
            
            var hPossibleEdgeToNodeIdIndex = new Dictionary<Edge,int>(FUsedGraph.GetEdgeIndices().Count);
            var hReachableNodeIdWithEdgeIndex = new Dictionary<int, Edge>(FUsedGraph.GetNodeIndices().Count);
            var hEdgeWeightAscComparer = new EdgeWeightComparerAsc();
            

              var hVisitedNodes = new HashSet<int>();           

            // Angabe eines Startknoten. Ein Knoten der mit der ersten Kante aus der Globalen Kantenliste erreicht wird.
            // Eine Fake-Edge für den Einstieg in den Algorithmus
            var hFakeEdge = new UndirectedEdge(new Node(-1), new Node(-1),new CostWeighted(0.0));
            hPossibleEdgeToNodeIdIndex.Add(hFakeEdge, 0);
            hReachableNodeIdWithEdgeIndex.Add(0, hFakeEdge);
            var hUsableEdgesAsc = new List<Edge>(hReachableNodeIdWithEdgeIndex.Values);
            

            double hWeightValue = 0.0;
            int hTotalNodeCount = hNodeDictionary.Count;

            while (hVisitedNodes.Count < hTotalNodeCount)
            {
                hUsableEdgesAsc.Sort(hEdgeWeightAscComparer);

                // Finde niedrigste Kante die zu einem unbesuchten Knoten führt
                var hSmallestPossibleEdge = hUsableEdgesAsc.First();
                var hReachableNodeIdWithSmallestEdge = hPossibleEdgeToNodeIdIndex[hSmallestPossibleEdge];
                
                // Knoten besucht
                hVisitedNodes.Add(hReachableNodeIdWithSmallestEdge);
                // Indizes anpassen
                hPossibleEdgeToNodeIdIndex.Remove(hSmallestPossibleEdge);
                hReachableNodeIdWithEdgeIndex.Remove(hReachableNodeIdWithSmallestEdge);
                hUsableEdgesAsc.Remove(hSmallestPossibleEdge);
                
                hWeightValue += hSmallestPossibleEdge.GetWeightValue();

                // Durch den gerade besuchten Knoten gibt es evtl. neue potenziell erreichbare Knoten oder bereits erreichbare können noch "günstiger" erreicht werden.
                // Diese Infos werden nun in die Erreichbaren Liste ergänzt.
                foreach (var hNewEdgeInfos in hNodeDictionary[hReachableNodeIdWithSmallestEdge].NeighboursEdges)
                {
                    var hNeighbourNodeId = hNewEdgeInfos.Node.Id;
                    var hEdgeToNeighbour = hNewEdgeInfos.Edge;
                    if (!hVisitedNodes.Contains(hNeighbourNodeId))
                    {
                        // Knoten wurde noch nicht besucht
                        if (hReachableNodeIdWithEdgeIndex.ContainsKey(hNeighbourNodeId))
                        {
                            // Es gibt schon einen Weg zu diesem Knoten
                            if (hEdgeToNeighbour.GetWeightValue() < hReachableNodeIdWithEdgeIndex[hNeighbourNodeId].GetWeightValue())
                            {
                                // Mit der neuen Kante kommt man aber günstiger zu dem Knoten
                                hUsableEdgesAsc.Remove(hReachableNodeIdWithEdgeIndex[hNeighbourNodeId]); // bisherige Kante in der sortierten Kanten-Liste entfernen
                                hUsableEdgesAsc.Add(hEdgeToNeighbour);
                                hPossibleEdgeToNodeIdIndex.Add(hEdgeToNeighbour, hNeighbourNodeId);
                                hReachableNodeIdWithEdgeIndex[hNeighbourNodeId] = hEdgeToNeighbour;
                            }
                        }
                        else
                        {
                            // Es gab bisher noch keinen Weg zu diesem Knoten. Also wird er nun aufgenommen
                            hUsableEdgesAsc.Add(hEdgeToNeighbour);
                            hPossibleEdgeToNodeIdIndex.Add(hEdgeToNeighbour, hNeighbourNodeId);
                            hReachableNodeIdWithEdgeIndex.Add(hNeighbourNodeId, hEdgeToNeighbour);
                        }
                    }
                }
            }

            FStopwatch.Stop();
            Console.WriteLine("Prim-Zeit:\t" + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine("Prim-Kosten:\t " + hWeightValue.ToString());
        }

        


        private void AddPossibleEdgeToNodeInfo(Dictionary<int, List<Edge>> _Dict, int _NodeId, Edge _Edge)
        {
            // Merken: Von welchen Kanten kann der Knoten x momentan erreicht werden
            if (!_Dict.ContainsKey(_NodeId))
            {
                _Dict.Add(_NodeId, new List<Edge>());
            }
            _Dict[_NodeId].Add(_Edge);
        }

        private void DeleteEdgesToNode(HashSet<Edge> _PossibleEdges, List<Edge> _EdgesToDelete)
        {
            foreach (var hEdgeToDelete in _EdgesToDelete)
            {
                _PossibleEdges.Remove(hEdgeToDelete);
            } 
        }
    }
}
