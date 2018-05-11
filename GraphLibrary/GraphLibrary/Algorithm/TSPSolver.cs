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
    class TSPSolver
    {
        private IGraph FUsedGraph;

        private int FNodeCount;

        private INode FStartNode;

        private Dictionary<string, Edge> FEdgeDictionary;

        private HashSet<int> FSmallestTour;

        private double FSmallestTourCost = Double.PositiveInfinity;

        private bool FUseBranchAndBound = true;

        private Stopwatch FStopwatch;

        public TSPSolver(IGraph _UsedGraph, bool _UseBranchAndBound = true)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
            FUseBranchAndBound = _UseBranchAndBound;
        }

        /// <summary>
        /// Findet eine exakte Möglichkeit. Zwei Modi:
        /// 1. BruteForce: Alle n-1! Möglichkeiten (also auch Duplikate) werden geprüft
        /// 2. Branch-And-Bound: 
        /// </summary>
        public void Execute()
        {
            Console.WriteLine("TSP");
            Console.WriteLine("Nutzung von Branch-And-Bound: " + FUseBranchAndBound.ToString());
            FStopwatch.Start();
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hUnvisitedNodes = new HashSet<INode>(hNodeDictionary.Values);
            FNodeCount = hNodeDictionary.Count;
            FEdgeDictionary = FUsedGraph.GetEdgeDictionary();

            FStartNode = hNodeDictionary[0];
            
            FindBestRouteRecursive(FStartNode, new HashSet<int>(), 0.0);

            FStopwatch.Stop();
            Console.WriteLine("Dauer der Berechnung: " + FStopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("Tour Kosten: " + FSmallestTourCost);
            Console.WriteLine("Tour Verlauf: ");
            foreach (var hNode in FSmallestTour)
            {
                Console.WriteLine(hNode);
            } 

        }

        private void FindBestRouteRecursive(INode _CurrentNode, HashSet<int> _Tour, double _TourCost)
        {
            if (FUseBranchAndBound && _TourCost > FSmallestTourCost) return; // Bound: Bisherige Tour ist schlechter als die bisher beste

            _Tour.Add(_CurrentNode.Id);

            if (_Tour.Count == FNodeCount)  
            {
                // Jeder Knoten wurde besucht. Jetzt noch die Tour zum Startknoten schließen
                var hClosingEdge = FEdgeDictionary[_CurrentNode.Id + "-" + FStartNode.Id];
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
                    
                    if (!_Tour.Contains(hNeighborEdge.Node.Id))
                    {
                        var hTourCost = _TourCost + hNeighborEdge.Edge.GetWeightValue();
                        var hTourClone = new HashSet<int>(_Tour);
                        FindBestRouteRecursive(hNeighborEdge.Node, hTourClone, hTourCost);
                    }
                }
            }
        }


        public void TSPBruteIterativ()
        {
            FStopwatch.Start();
            var hDfsStack = new Stack<INode>();
            var hTourStack = new Stack<string>();
            var hCostStack = new Stack<double>();
            var hUnvisitedNodesStack = new Stack<HashSet<INode>>();
            var hSmallestTourCost = Double.PositiveInfinity;
            string hSmallestTour = "";

            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hStartNode = hNodeDictionary[0];
            var hUnsivitedNodes = new HashSet<INode>(hNodeDictionary.Values);
            //hUnsivitedNodes.Remove(hStartNode);


            hDfsStack.Push(hStartNode);
            hTourStack.Push(hStartNode.Id.ToString());
            hCostStack.Push(0.0);
            hUnvisitedNodesStack.Push(hUnsivitedNodes);

            while (hDfsStack.Count > 0)
            {
                var hCurrentNode = hDfsStack.Pop();
                var hCurrentPath = hTourStack.Pop();
                var hCurrentCost = hCostStack.Pop();
                var hCurrentUnvisitedNodes = hUnvisitedNodesStack.Pop();
                hCurrentUnvisitedNodes.Remove(hCurrentNode);

                if (hCurrentUnvisitedNodes.Count == 0)
                {
                    // Am Ende angelangt
                    var hNodeIdsInTour = hCurrentPath.Split(',');
                    var hLastNodeInTour = Convert.ToInt32(hNodeIdsInTour.Last());
                    var hClosingEdge = FUsedGraph.GetEdge(hLastNodeInTour, hStartNode.Id, true);
                    var hTotalTourCost = hCurrentCost + hClosingEdge.GetWeightValue();
                    if (hTotalTourCost <= hSmallestTourCost)
                    {
                        hSmallestTourCost = hTotalTourCost;
                        hSmallestTour = hCurrentPath;
                    }
                }
                else
                {
                    foreach (var hNeighborEdge in hCurrentNode.NeighbourEdges)
                    {
                        if (hCurrentUnvisitedNodes.Contains(hNeighborEdge.Node))
                        {
                            hDfsStack.Push(hNeighborEdge.Node);
                            hTourStack.Push(hCurrentPath + "," + hNeighborEdge.Node.Id.ToString());
                            hCostStack.Push(hCurrentCost + hNeighborEdge.Edge.GetWeightValue());
                            hUnvisitedNodesStack.Push(new HashSet<INode>(hCurrentUnvisitedNodes));
                        }
                    }
                }
                
            }

            FStopwatch.Stop();
            Console.WriteLine("Dauer der Berechnung: " + FStopwatch.ElapsedMilliseconds + " ms");
            Console.WriteLine("Tour Kosten: " + hSmallestTourCost);
            Console.WriteLine("Tour Verlauf: " + hSmallestTour);
        }


    }
}
