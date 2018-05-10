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

        public NearestNeighborAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public IGraph Execute(INode _StartNode)
        {

            var hStopwatch = new Stopwatch();
            Console.WriteLine("Start Nearest Neigbor Algorithmus");
            hStopwatch.Start();

            var hNodeInGraphCount = FUsedGraph.GetNodeIndices().Count;
            var hTourCosts = 0.0;
            var hVisitedNodes = new HashSet<INode>();

            var hNearestNeighborGraph = new Graph();
            var hCurrentNode = _StartNode;  // Auswahl Startknoten.        
            hVisitedNodes.Add(hCurrentNode);
            hNearestNeighborGraph.CreateNewNode(hCurrentNode.Id);
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

                // Knoten und Kante in neuen Graph einfügen
                hNearestNeighborGraph.CreateNewNode(hCurrentNode.Id);
                hNearestNeighborGraph.CreateUndirectedEdge(hMinimumNodeEdge.Edge.GetPossibleEnpoints()[0],
                    hMinimumNodeEdge.Edge.GetPossibleEnpoints()[1],
                    new CostWeighted(hMinimumEdgeWeight));

                Console.WriteLine("Zu Knoten-ID: " + hCurrentNode?.Id + "\tKosten: " + hMinimumEdgeWeight);
            }

            // Kreis schließen
            foreach (var hEdgeToNeighbor in hCurrentNode.NeighboursEdges)
            {
                if (hEdgeToNeighbor.Node == _StartNode)
                {
                    hTourCosts += hEdgeToNeighbor.Edge.GetWeightValue();
                    hNearestNeighborGraph.CreateUndirectedEdge(hEdgeToNeighbor.Edge.GetPossibleEnpoints()[0],
                        hEdgeToNeighbor.Edge.GetPossibleEnpoints()[1],
                        new CostWeighted(hEdgeToNeighbor.Edge.GetWeightValue()));
                    Console.WriteLine("Tour beenden zu ID: " + _StartNode?.Id + "\tKosten: " + hEdgeToNeighbor.Edge.GetWeightValue());
                    break;
                }
            }

            hStopwatch.Stop();
            Console.WriteLine("Gesamtkosten:\t" + hTourCosts);
            Console.WriteLine("Dauer:\t" + hStopwatch.ElapsedMilliseconds + " ms");

            return hNearestNeighborGraph;
        }
    }
}
