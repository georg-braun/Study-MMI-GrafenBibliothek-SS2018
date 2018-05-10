using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class DoppelterBaumAlgorithm
    {

        private IGraph FUsedGraph;

        public DoppelterBaumAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        /// <summary>
        ///  Führt den Doppelten Baum Algorithmus aus.
        /// </summary>
        /// <param name="_StartNode"></param>
        /// <returns>Neues Graphen Objekt.</returns>
        public IGraph Execute(INode _StartNode)
        {
            var hStopwatch = new Stopwatch();
            var hTourCosts = 0.0;
            Console.WriteLine("Start Doppelter Baum Algorithmus");
            hStopwatch.Start();
            
            var hDoppelterBaumGraph = new Graph();

            // MST ermitteln
            var hMstGraph = new KruskalAlgorithm(FUsedGraph).Execute();
            // Tiefensuche auf MST anwenden
            var hDfsGraph = new DepthFirstSearch().Execute(hMstGraph.GetNodeDictionary()[0]);
            var hDfsGraphNodeOrder = hDfsGraph.GetNodeDictionary().Values.ToList();

            Console.WriteLine("Berechnung Tour");
            // Ergebnis der Tiefensuche durchlaufen und die Knoten mit der direkten Kante verbinden (entspricht fast Hamilton-Tour)
            for (var hDfsOrderIndex = 0; hDfsOrderIndex < hDfsGraphNodeOrder.Count-1; hDfsOrderIndex++)
            {
                // Verbindungsinformationen ermitteln
                var hNodeA = hDfsGraphNodeOrder[hDfsOrderIndex];
                var hNodeB = hDfsGraphNodeOrder[hDfsOrderIndex+1];
                var hEdgeAtoB = FUsedGraph.GetEdge(hNodeA.Id, hNodeB.Id, true);

                Console.WriteLine("Von \t" + hNodeA.Id + "\tnach\t" +hNodeB.Id + "\tKosten:\t" + hEdgeAtoB.GetWeightValue());

                // Neue Graphen Objekt befüllen
                hDoppelterBaumGraph.CreateNewNode(hNodeA.Id);
                hDoppelterBaumGraph.CreateNewNode(hNodeB.Id);
                hDoppelterBaumGraph.CreateUndirectedEdge(hNodeA.Id, hNodeB.Id, new CostWeighted(hEdgeAtoB.GetWeightValue()));
                hTourCosts += hEdgeAtoB.GetWeightValue();
            }

            // Kreis schließen
            var hTourStartNode = hDfsGraphNodeOrder[0];
            var hTourLastVisitedNode = hDfsGraphNodeOrder[hDfsGraphNodeOrder.Count-1];
            var hTourFinalEdge = FUsedGraph.GetEdge(hTourStartNode.Id, hTourLastVisitedNode.Id, true);
            Console.WriteLine("Von \t" + hTourLastVisitedNode.Id + "\tnach\t" + hTourStartNode.Id + "\tKosten:\t" + hTourFinalEdge.GetWeightValue());
            hDoppelterBaumGraph.CreateUndirectedEdge(hTourStartNode.Id, hTourLastVisitedNode.Id, new CostWeighted(hTourFinalEdge.GetWeightValue()));
            hTourCosts += hTourFinalEdge.GetWeightValue();


            hStopwatch.Stop();
            Console.WriteLine("Gesamtkosten:\t" + hTourCosts);
            Console.WriteLine("Dauer:\t" + hStopwatch.ElapsedMilliseconds + " ms");

            return hDoppelterBaumGraph;
        }

    }
}
