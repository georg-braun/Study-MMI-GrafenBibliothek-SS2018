using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class NearestNeighborAlgorithm
    {

        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;

        public NearestNeighborAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public IGraph Execute(INode _StartNode)
        {
            var hNodeInGraphCount = FUsedGraph.GetNodeIndices().Count;
            var hTourCosts = 0.0;
            var hVisitedNodes = new HashSet<INode>();


            var hCurrentNode = _StartNode;  // Auswahl Startknoten.        
            hVisitedNodes.Add(hCurrentNode);
            Console.WriteLine("Start bei Knoten ID: " + hCurrentNode.Id);

            // Solange es noch unbesuchte Knoten gibt
            while (hVisitedNodes.Count < hNodeInGraphCount)
            {
                double hMinimumEdgeWeight = Double.MaxValue;
                NodeEdge hMinimumNodeEdge = null;

                // Guck dir alle Nachbarn vom aktuellen Nachbarn an und wähle die niedrigste Kante zu einem unbesuchten Knoten
                foreach (var hEdgeToNeighbor in hCurrentNode.NeighboursEdges)
                {
                    var hEdgeToNeighborWeight = hEdgeToNeighbor.Edge.GetWeightValue();
                    if (!hVisitedNodes.Contains(hEdgeToNeighbor.Node) && hEdgeToNeighborWeight < hMinimumEdgeWeight)
                    {
                        hMinimumEdgeWeight = hEdgeToNeighborWeight;
                        hMinimumNodeEdge = hEdgeToNeighbor;
                    }
                }

                hTourCosts += hMinimumEdgeWeight;
                hCurrentNode = hMinimumNodeEdge?.Node;
                hVisitedNodes.Add(hCurrentNode);
                Console.WriteLine("Zu Knoten-ID: " + hCurrentNode?.Id + "\tKosten: " + hMinimumEdgeWeight);
            }

            // Kreis schließen
            foreach (var hEdgeToNeighbor in hCurrentNode.NeighboursEdges)
            {
                if (hEdgeToNeighbor.Node == _StartNode)
                {
                    hTourCosts += hEdgeToNeighbor.Edge.GetWeightValue();
                    Console.WriteLine("Tour beenden zu ID: " + _StartNode?.Id + "\tKosten: " + hEdgeToNeighbor.Edge.GetWeightValue());
                    break;
                }
            }

            Console.WriteLine("Gesamtkosten: " + hTourCosts);


            return null;
        }
    }
}
