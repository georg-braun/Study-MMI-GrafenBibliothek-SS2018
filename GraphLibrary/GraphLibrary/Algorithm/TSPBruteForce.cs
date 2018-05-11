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

        //private List<List<INode>> FAllTours;

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
            var hUnvisitedNodes = new HashSet<INode>(hNodeDictionary.Values);

            
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

        private void FindBestRouteRecursive(INode _CurrentNode, HashSet<INode> _UnvisitedNodes, List<INode> _Tour, double _TourCost)
        {
            _Tour.Add(_CurrentNode);
            _UnvisitedNodes.Remove(_CurrentNode);

            if (_Tour.Count == FNodeCount)  
            {
                // Jeder Knoten wurde besucht. Jetzt noch die Tour zum Startknoten schließen
                _Tour.Add(FStartNode);
                //FAllTours.Add(_Tour);

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
                        var hUnvisitedNodesClone = new HashSet<INode>(_UnvisitedNodes);
                        var hTourClone = new List<INode>(_Tour);
                        FindBestRouteRecursive(hNeighborEdge.Node, hUnvisitedNodesClone, hTourClone, hTourCost);
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
