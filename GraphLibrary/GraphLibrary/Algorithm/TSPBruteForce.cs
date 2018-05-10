using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class TSPBruteForce
    {
        private IGraph FUsedGraph;

        public TSPBruteForce(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        private int FNodeCount;

        private INode FStartNode;

        private List<List<INode>> FAllTours;

        private List<INode> FSmallestTour;

        private double FSmallestTourCost = Double.PositiveInfinity;

        private Stopwatch FStopwatch;

        /// <summary>
        /// Probiert alle Routen aus (auch Duplikate). Also (n-1)! mögliche
        /// </summary>
        public void Execute()
        {
            Console.WriteLine("Brute Force TSP");
            FStopwatch.Start();
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            FNodeCount = hNodeDictionary.Count;

            FStartNode = hNodeDictionary[0];
            var hUnvisitedNodes = hNodeDictionary.Values.ToList();

            FAllTours = new List<List<INode>>();

            FindBestRouteRecursive(FStartNode, hUnvisitedNodes, new List<INode>(), 0.0);

            FStopwatch.Stop();
            Console.WriteLine("Dauer der Berechnung: " + FStopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("Tour Kosten: " + FSmallestTourCost);
            Console.WriteLine("Tour Verlauf: ");
            foreach (var hNode in FSmallestTour)
            {
                Console.WriteLine(hNode.Id);
            } 

        }

        private void FindBestRouteRecursive(INode _CurrentNode, List<INode> _UnvisitedNodes, List<INode> _Tour, double _TourCost)
        {
            _Tour.Add(_CurrentNode);
            _UnvisitedNodes.Remove(_CurrentNode);

            if (_Tour.Count == FNodeCount)  
            {
                // Jeder Knoten wurde besucht. Jetzt noch die Tour zum Startknoten schließen
                _Tour.Add(FStartNode);
                FAllTours.Add(_Tour);

                var hClosingEdge = FUsedGraph.GetEdge(_CurrentNode.Id, FStartNode.Id, true);
                var hTotalTourCost = _TourCost + hClosingEdge.GetWeightValue();
                if (hTotalTourCost <= FSmallestTourCost)
                {
                    FSmallestTourCost = hTotalTourCost;
                    FSmallestTour = _Tour;
                }
            }
            else
            {
                foreach (var hNeighborEdge in _CurrentNode.NeighbourEdges)
                {
                    if (_UnvisitedNodes.Contains(hNeighborEdge.Node))
                    {
                        var hTourCost = _TourCost + hNeighborEdge.Edge.GetWeightValue();
                        FindBestRouteRecursive(hNeighborEdge.Node, new List<INode>(_UnvisitedNodes), new List<INode>(_Tour), hTourCost);
                    }
                }
            }
        }


    }
}
