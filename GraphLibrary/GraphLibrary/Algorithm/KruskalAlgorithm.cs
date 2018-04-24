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

        private Stopwatch FindCircleStopwatch;
        

        public KruskalAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
            FindCircleStopwatch = new Stopwatch();
            
        }

        public IGraph Execute()
        {
            // ToDo: Ggf. sinnvoll aus dem übergebenen Grafen die Subgrafen zu ermitteln und dann zu wissen welche Knoten darin vorhanden sind
            // Der Algorithmus könnte sonst Probleme bekommen wenn es mehr als eine Zusammenhangskomponente gibt
            FStopwatch.Start();

            var hMinimalSpanningTree = new Graph();

            // Use a copy of the Edge List. Don't wan't to affect the datastructure
            var hEdges = new List<Edge>(FUsedGraph.GetEdgeIndices());
            var hNodeDictionary = FUsedGraph.GetNodeIndices();
            
            var hEdgeWeightComparerAsc = new EdgeWeightComparerAsc();
            hEdges.Sort(hEdgeWeightComparerAsc);

            var hNodePathList = new Dictionary<int,List<int>>();
            foreach (var hNodeValue in hNodeDictionary)
            {
                // Jeder Eintrag in der Path List hat eine Liste mit den in dem Path enthaltenen Knoten
                var hNodesInPath = new List<int>();
                hNodesInPath.Add(hNodeValue.Value.Id);
                hNodePathList.Add(hNodeValue.Value.Id, hNodesInPath);
            }


            double hCosts = 0;
            var hNodesAdded = 1;    // Initial ein Knoten schon hinzugefügt
            var hCurrentEdgeId = 0;
            var hEdgesCount = hEdges.Count;
            var hNodesInDictionaryCount = hNodeDictionary.Count;
            List<int> hBiggerPath;
            List<int> hSmallerPath;


            while (hCurrentEdgeId < hEdgesCount && hNodesAdded < hNodesInDictionaryCount)
            {
                var hEdgeEndPoints = hEdges[hCurrentEdgeId].GetPossibleEnpoints();
                var hNodeAId = hEdgeEndPoints[0].Id;
                var hNodeBId = hEdgeEndPoints[1].Id;

                if ( hNodePathList[hNodeAId] != hNodePathList[hNodeBId] )
                {
                    var hNewNodeA = new Node(hNodeAId);
                    var hNewNodeB = new Node(hNodeBId);
                    hMinimalSpanningTree.AddNode(hNewNodeA);
                    hMinimalSpanningTree.AddNode(hNewNodeB);
                    hMinimalSpanningTree.CreateUndirectedEdge(hNewNodeA, hNewNodeB, new CostWeighted(hEdges[hCurrentEdgeId].GetWeightValue()));


                    // Knoten A und B verweisen auf verschiedene Pfad-Listen. Also sind die nicht im selben Pfad und würden keinen Kreis bilden
                    // Noch prüfen welcher Pfad der größere ist. Es soll nämlich der kleinere Pfad in den größeren Eingefügt werden
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
                    hCosts += hEdges[hCurrentEdgeId].GetWeightValue();
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

            if (hXWeightValue == hYWeightValue)
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
