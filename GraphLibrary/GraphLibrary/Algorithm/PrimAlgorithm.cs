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
            var hEdges = new HashSet<Edge>();
            //var hPossibleEdges = new HashSet<Edge>();
            var hEdgesToNodeDict = new Dictionary<int,List<Edge>>();
            //var hEdgeWeightComparerAsc = new EdgeWeightComparerAsc();

            var hNodeDictionary = FUsedGraph.GetNodeIndices();
            var hVisitedNodes = new HashSet<int>();


            var hNewNodeId = hNodeDictionary[0].Id;
            hVisitedNodes.Add(hNewNodeId);
            double hWeightValue = 0.0;

            while (hVisitedNodes.Count < hNodeDictionary.Count)
            {
                // Ergänze die Kanten des aktuellen Knotens in die Liste der verfügbaren Kanten
                foreach (var hPossibleEdge in hNodeDictionary[hNewNodeId].NeighboursEdges)
                {
                    // Wenn der erreichbare Knoten noch nicht besucht wurde, füge die Kante als mögliche hinzu
                    if (!hVisitedNodes.Contains(hPossibleEdge.Node.Id))
                    {
                        hEdges.Add(hPossibleEdge.Edge);
                        AddPossibleEdgeToNodeInfo(hEdgesToNodeDict, hPossibleEdge.Node.Id, hPossibleEdge.Edge);
                    }
                }

                var hSmallestEdge = hEdges.First();
                // Suche die niedrigste Kante
                foreach (var hEdge in hEdges)
                {
                    if (hEdge.GetWeightValue() < hSmallestEdge.GetWeightValue())
                    {
                        hSmallestEdge = hEdge;
                    }
                } 
                
                hWeightValue += hSmallestEdge.GetWeightValue();

                // Rausfinden welcher der beiden Knoten in der Kante der neu erreichte Knoten ist.
                var hPossibleEndponts = hSmallestEdge.GetPossibleEnpoints();
                if (hVisitedNodes.Contains(hPossibleEndponts[0].Id))
                {
                    hNewNodeId = hPossibleEndponts[1].Id;
                }
                else
                {
                    hNewNodeId = hPossibleEndponts[0].Id;
                }

                hVisitedNodes.Add(hNewNodeId);

                // Kanten in der Liste die auch zu diesem neuen Knoten gehen werden nicht mehr benötigt.
                DeleteEdgesToNode(hEdges,hEdgesToNodeDict[hNewNodeId]);
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
