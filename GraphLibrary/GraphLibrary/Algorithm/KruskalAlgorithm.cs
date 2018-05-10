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
    /// <summary>
    /// Idee des Algorithmus:
    /// Alle Kanten des Algorithmus werden aufsteigend nach ihrem Kantengewicht sortiert
    /// Solange noch nicht alle Knoten im MST drin sind
    /// (a) Füge die kleinste Kante in den MST ein, falls diese zu keinem Kreis im MST führt. 
    /// </summary>
    class KruskalAlgorithm
    {
        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;

        
        public KruskalAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        /// <summary>
        /// Ermittelt den Minimalen Spannbaum mit dem Kruskal Algorithmus
        /// </summary>
        /// <returns>Neuer Graf der den MST repräsentiert</returns>
        public IGraph Execute()
        {
            // Umsetzung des Algorithmus:
            // Zunächst werden die Kanten aufsteigend nach ihrem Gewicht sortiert
            // hNodePathList: Dictionary über KnotenIds (Key). Als Value ist ein Verweis auf eine Pfadliste enthalten, die alle KnotenIds eines Pfads enthält.
            //   initial hat also jeder Knoten seine eigene Liste (= PFad nur aus sich selbst).


            FStopwatch.Start();

            var hMinimalSpanningTree = new Graph();

            // Use a copy of the Edge List. Don't wan't to affect the datastructure
            var hEdgesAsc = new List<Edge>(FUsedGraph.GetEdgeIndices());
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            
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

            // ToDo Fehlerfall mit ausgeben wenn es keinen MST gibt
            while (hCurrentEdgeId < hEdgesCount && hNodesAdded < hNodesInDictionaryCount)
            {
                var hEdgeEndPoints = hEdgesAsc[hCurrentEdgeId].GetPossibleEnpoints();
                var hNodeAId = hEdgeEndPoints[0].Id;
                var hNodeBId = hEdgeEndPoints[1].Id;

                if ( hNodePathList[hNodeAId] != hNodePathList[hNodeBId] )
                {
                    // ToDo Kann sicher sein dass es ein MST ist (abprüfen), vorher schon Knoten initialisieren
                    // Knoten A und B verweisen auf verschiedene Pfad-Listen. Also sind die nicht im selben Pfad und würden keinen Kreis bilden
                    var hNewNodeA = new Node(hNodeAId);
                    var hNewNodeB = new Node(hNodeBId);
                    var hNodeA = hMinimalSpanningTree.TryToAddNode(hNewNodeA);
                    var hNodeB = hMinimalSpanningTree.TryToAddNode(hNewNodeB);
                    hMinimalSpanningTree.CreateUndirectedEdge(hNodeA, hNodeB, new CostWeighted(hEdgesAsc[hCurrentEdgeId].GetWeightValue()));
                    
                    
                    // Noch prüfen welcher Pfad der größere ist. Es soll nämlich die kleinere Pfadliste in die Größere eingefügt werden
                    List<int> hBiggerPath;
                    List<int> hSmallerPath;
                    if (hNodePathList[hNodeAId].Count >= hNodePathList[hNodeBId].Count)
                    {
                        hBiggerPath = hNodePathList[hNodeAId];
                        hSmallerPath = hNodePathList[hNodeBId];
                    }
                    else
                    {
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

                    // Kante wurde hinzugefügt. Also Gewicht aufaddieren
                    // Todo Nach oben zu der MST Generierung verschieben
                    hCosts += hEdgesAsc[hCurrentEdgeId].GetWeightValue();
                    hNodesAdded++;
                }

                hCurrentEdgeId++;
            } 
            hMinimalSpanningTree.UpdateNeighbourInfoInNodes();

            FStopwatch.Stop();
            Console.WriteLine("--- Kruskal ---");
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
