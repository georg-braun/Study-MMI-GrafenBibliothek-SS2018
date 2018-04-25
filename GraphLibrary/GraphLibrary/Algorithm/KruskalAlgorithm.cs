using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class KruskalAlgorithm
    {
        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;


        public KruskalAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        public IGraph Execute()
        {
            FStopwatch.Start();

            var hMinimalSpanningTree = new Graph();

            // Use a copy of the Edge List. Don't wan't to affect the datastructure
            var hEdgesAsc = new List<Edge>(FUsedGraph.GetEdgeIndices());
            var hNodeDictionary = FUsedGraph.GetNodeIndices();
            
            var hEdgeWeightComparerAsc = new EdgeWeightComparerAsc();
            hEdgesAsc.Sort(hEdgeWeightComparerAsc);

            var hNodePathList = new Dictionary<int,List<int>>();
            foreach (var hNodeValue in hNodeDictionary)
            {
                // Jeder Eintrag in der Path List hat eine Liste mit den in dem Path enthaltenen Knoten
                var hNodesInPath = new List<int> { hNodeValue.Value.Id };
                hNodePathList.Add(hNodeValue.Value.Id, hNodesInPath);
            }


            double hCosts = 0;
            var hNodesAdded = 1;    // Initial ein Knoten schon hinzugefügt
            var hCurrentEdgeId = 0;
            var hEdgesCount = hEdgesAsc.Count;
            var hNodesInDictionaryCount = hNodeDictionary.Count;

            while (hCurrentEdgeId < hEdgesCount && hNodesAdded < hNodesInDictionaryCount)
            {
                var hEdgeEndPoints = hEdgesAsc[hCurrentEdgeId].GetPossibleEnpoints();
                var hNodeAId = hEdgeEndPoints[0].Id;
                var hNodeBId = hEdgeEndPoints[1].Id;

                if ( hNodePathList[hNodeAId] != hNodePathList[hNodeBId] )
                {
                    var hNewNodeA = new Node(hNodeAId);
                    var hNewNodeB = new Node(hNodeBId);
                    hMinimalSpanningTree.AddNode(hNewNodeA);
                    hMinimalSpanningTree.AddNode(hNewNodeB);
                    hMinimalSpanningTree.CreateUndirectedEdge(hNewNodeA, hNewNodeB, new CostWeighted(hEdgesAsc[hCurrentEdgeId].GetWeightValue()));


                    // Knoten A und B verweisen auf verschiedene Pfad-Listen. Also sind die nicht im selben Pfad und würden keinen Kreis bilden
                    // Noch prüfen welcher Pfad der größere ist. Es soll nämlich der kleinere Pfad in den größeren Eingefügt werden
                    List<int> hBiggerPath;
                    List<int> hSmallerPath;
                    if (hNodePathList[hNodeAId].Count >= hNodePathList[hNodeBId].Count)
                    {
                        // B in A
                        hBiggerPath = hNodePathList[hNodeAId];
                        hSmallerPath = hNodePathList[hNodeBId];
                    }
                    else
                    {
                        // A in B
                        hBiggerPath = hNodePathList[hNodeBId];
                        hSmallerPath = hNodePathList[hNodeAId];
                    }

                    // Die Pfad-Liste von dem kleineren in den größeren angehangen wird nun in die Pfadliste von A angehangen.
                    hBiggerPath.AddRange(hSmallerPath);
                    // Alle Knoten die im kleineren Pfad waren, sollen nun im Dictionary auch auf den zusammengeführten Pfad zeigen
                    foreach (var hNodeInSmaller in hSmallerPath)
                    {
                        hNodePathList[hNodeInSmaller] = hBiggerPath;
                    }

                    // Kante wurde hinzugefügt. Also gewicht aufaddieren
                    hCosts += hEdgesAsc[hCurrentEdgeId].GetWeightValue();
                    hNodesAdded++;
                }

                hCurrentEdgeId++;
            } 

            FStopwatch.Stop();
            Console.WriteLine("Kruskal-Zeit:\t" + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine("Kruskal-Kosten:\t " + hCosts.ToString());

            return hMinimalSpanningTree;
        }


        public void PrintInfosToConsole()
        {
            throw new NotImplementedException();
        }
    }

    class EdgeWeightComparerAsc : IComparer<Edge>
    {
        public int Compare(Edge x, Edge y)
        {
            var hXWeightValue = x.GetWeightValue();
            var hYWeightValue = y.GetWeightValue();
            
            if (Equals(hXWeightValue, hYWeightValue))
            {
                return 0;
            }
            else if (hXWeightValue > hYWeightValue)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
    }
}
