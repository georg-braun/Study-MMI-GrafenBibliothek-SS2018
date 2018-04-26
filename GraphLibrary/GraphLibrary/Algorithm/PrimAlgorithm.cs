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
            var hNodeEdge = new NodeEdge(hStartNode, new UndirectedEdge(hStartNode, hStartNode, new CostWeighted(0.0)));
            
            // Startknoten hinzufügen
            hSmallestEdgePq.Enqueue(hNodeEdge, 0.0);
            hNodeCosts[hStartNodeIndex] = 0.0;

            while (hNodesAdded < hNodeIndex.Count)
            {
                var hNewNodeEdge = hSmallestEdgePq.Dequeue();
                var hNewNodeInId = hNewNodeEdge.Node.Id;

                if (hInMst[hNewNodeInId]) continue;  // Der Zielknoten der kleinsten Kante wurde bereits besucht. Also nächste Iteration durchlaufen (=nächste Kante nehmen)
                hInMst[hNewNodeInId] = true;
                hNodesAdded++;
                hCosts += hNewNodeEdge.Edge.GetWeightValue();

                foreach (var hNeighbourNodeEdges in hNewNodeEdge.Node.NeighboursEdges)
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


            FStopwatch.Stop();
            Console.WriteLine("Kruskal-Zeit:\t" + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine("Kruskal-Kosten:\t " + hCosts.ToString());

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
