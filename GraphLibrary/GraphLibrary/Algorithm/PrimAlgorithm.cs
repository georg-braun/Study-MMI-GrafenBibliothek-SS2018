using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

using Priority_Queue;

namespace GraphLibrary.Algorithm
{
    /// <summary>
    /// Idee des Algorithmus:
    /// Es wird ein Startknoten ausgewählt. Solange der minimale Spannbaum noch nicht alle Knoten 
    /// aus dem Ursprungsgraf enthält wird folgendes wiederholt:
    /// (a) Die billigste Kante die von einem besuchten Knoten (im MST) zu einem nicht besuchten Knoten wird ausgewählt.
    /// (b) Die Kante wird in den MST hinzugefügt.
    /// (c) Prüfe die Nachbarn des neuen Knoten welche (neuen) Knoten erreicht werden können oder ob bisher unbesuchte registrierte Knoten günstiger erreicht werden.   
    /// </summary>
    class PrimAlgorithm
    {

        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;
    

        public PrimAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        /// <summary>
        /// Ermittelt den Minimalen Spannbaum mit dem Prim Algorithmus
        /// </summary>
        /// <returns>Neuer Graf der den MST repräsentiert</returns>

        public IGraph Execute()
        {
            // Umsetzung des Algorithmus:
            // hInMst: Liste über alle KnotenIds und speichert ob die jeweilige KnotenId schon im MST drin ist.
            // hNodeCosts : Liste über KnotenIds und speichert die minimalsten Kosten zu einem Knoten (dient als schneller Vergleich ob es eine günstigere Kante gibt)
            // hSmallestEdgePq : PriorityQueue, wobei die Priority die Kantenkosten sind und als Value ein NodeEdge Objekt dient

            FStopwatch.Start();

            var hMinimalSpanningTree = new Graph();
            double hCosts = 0.0;

            var hNodeIndex = FUsedGraph.GetNodeIndices();
            var hEdgeIndex = FUsedGraph.GetEdgeIndices();
            var hNodesAdded = 0;

            var hInMst = new List<bool>();
            var hNodeCosts = new List<double>();    // Sichert die minimalen Kosten mit denen ein Knoten erreicht werden kann
            var hSmallestEdgePq = new SimplePriorityQueue<NodeEdge, double>();

            for (var i = 0; i < hNodeIndex.Count; i++)
            {
                hInMst.Add(false);
                hNodeCosts.Add(double.MaxValue);
            }

            var hStartNodeIndex = 0;
            var hStartNode = hNodeIndex[hStartNodeIndex];
            var hStartFakeEdge = new UndirectedEdge(hStartNode, hStartNode, new CostWeighted(0.0));
            var hStartNodeEdge = new NodeEdge(hStartNode, hStartFakeEdge); // Fake-Edge für den Startknoten.
            
            // Startknoten hinzufügen
            hSmallestEdgePq.Enqueue(hStartNodeEdge, 0.0);
            hNodeCosts[hStartNodeIndex] = 0.0;

            while (hNodesAdded < hNodeIndex.Count)
            {
                var hSmallestNodeEdge = hSmallestEdgePq.Dequeue();
                var hSmallestEdgesNodeId = hSmallestNodeEdge.Node.Id;

                if (hInMst[hSmallestEdgesNodeId]) continue;  // Der Zielknoten der kleinsten Kante wurde bereits besucht. Also nächste Iteration durchlaufen (=nächste Kante nehmen)
                hInMst[hSmallestEdgesNodeId] = true;
                hNodesAdded++;
                // Create MSG Graph
                var hNewNode = new Node(hSmallestEdgesNodeId);
                hMinimalSpanningTree.CreateNewNode(hSmallestEdgesNodeId);
                var hFromNodeId = hSmallestNodeEdge.Edge.GetOtherEndpoint(hSmallestNodeEdge.Node).Id;
                var hFromNode = hMinimalSpanningTree.GetNodeById(hFromNodeId);
                hMinimalSpanningTree.CreateUndirectedEdge(hNewNode, hFromNode, new CostWeighted(hSmallestNodeEdge.Edge.GetWeightValue()));

                hCosts += hSmallestNodeEdge.Edge.GetWeightValue();

                foreach (var hNeighbourNodeEdges in hSmallestNodeEdge.Node.NeighboursEdges)
                {
                    var hNeighbourId = hNeighbourNodeEdges.Node.Id;
                    var hNeighbourWeight = hNeighbourNodeEdges.Edge.GetWeightValue();

                    if (hInMst[hNeighbourId] == false && hNodeCosts[hNeighbourId] > hNeighbourWeight)
                    {
                        hNodeCosts[hNeighbourId] = hNeighbourWeight;
                        hSmallestEdgePq.Enqueue(hNeighbourNodeEdges, hNeighbourWeight);
                    }
                }

            }

            // Die Fake-Edge des Startknotens wieder aus dem MST-Graf rausnehmen. (Es ist die erste Kante des Startknotens)
            hMinimalSpanningTree.RemoveEdge( hMinimalSpanningTree.GetNodeIndices()[hStartNodeIndex].NeighboursEdges[0].Edge );

            FStopwatch.Stop();
            Console.WriteLine("--- Prim ---");
            Console.WriteLine("Prim-Zeit:\t" + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine("Prim-Kosten:\t " + hCosts.ToString());

            return hMinimalSpanningTree;
        }


        private string GenerateEdgeHashValue(Edge _Edge)
        {
            var hEndpoints = _Edge.GetPossibleEnpoints();
            return hEndpoints[0].Id.ToString() + "-" + hEndpoints[1].Id.ToString();
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
