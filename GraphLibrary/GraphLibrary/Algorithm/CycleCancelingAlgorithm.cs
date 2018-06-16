using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class CycleCancelingAlgorithm
    {
        private IGraph FUsedGraph;

        public CycleCancelingAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public void Execute()
        {
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hNodeCount = hNodeDictionary.Count;

            // Erstellen der SuperQuelle
            var hSuperSource = new BalancedNode(hNodeCount, 0.0);
            FUsedGraph.TryToAddNode(hSuperSource);

            // Erstellen der SuperSenke
            var hSuperTarget = new BalancedNode(hNodeCount + 1, 0.0);
            FUsedGraph.TryToAddNode(hSuperTarget);

            // Balance der Knoten prüfen und an SuperQuelle und SuperSenke anhängen
            foreach (var hNode in hNodeDictionary.Values)
            {
                var hNodeBalance = ((BalancedNode)hNode).Balance;
                if (hNodeBalance > 0.0)
                {
                    // Es ist eine Quelle. Die SuperQuelle soll nun eine Kante auf diesen Knoten haben. Die Kapazität der Kante entspricht der Balance des Knotens
                    var hNewEdge = new DirectedEdge(hSuperSource, hNode);
                    hNewEdge.AddWeight(new CapacityWeighted(hNodeBalance));
                    FUsedGraph.AddEdge(hNewEdge); // Todo: Prüfen ob es richtig funktioniert
                }
                else if (hNodeBalance < 0.0)
                {
                    // Es ist eine Senke. Dieser Knoten soll nun eine Kante zur SuperSenke haben. Die Kapazität der Kante entspricht der Balance des Knotens
                    var hNewEdge = new DirectedEdge(hNode, hSuperTarget);
                    hNewEdge.AddWeight(new CapacityWeighted(-1.0 * hNodeBalance));
                    FUsedGraph.AddEdge(hNewEdge); // Todo: Prüfen ob es richtig funktioniert
                }
            }
            
            FUsedGraph.UpdateNeighbourInfoInNodes();


            // Nun soll der Edmonds-Karp Algorithmus ausgeführt werden. Dabei werden die SuperQuelle und SuperSenke verwendet
            var hEdmondsKarpAlgorithm = new EdmondsKarpAlgorithm(FUsedGraph);
            var hFlow = hEdmondsKarpAlgorithm.Execute(hSuperSource.Id, hSuperTarget.Id);

            
            // Nutzt die SuperQuelle die Kantenkapazitäten voll aus? 
            foreach (var hNeighborEdge in hSuperSource.NeighbourEdges)
            {
                var hNeighborEdgeHash = hNeighborEdge.Edge.EdgeHashInfo()[0];
                var hFlowValue = hFlow[hNeighborEdgeHash];
                var hEdgeCapacity = hNeighborEdge.Edge.GetWeightValue<CapacityWeighted>();
                if (!hFlowValue.Equals(hEdgeCapacity))
                {
                    // Kein gültiger b-Fluss (Graph kann nicht alles übertragen)
                    FUsedGraph.RemoveNode(hSuperSource);
                    FUsedGraph.RemoveNode(hSuperTarget);
                    Console.WriteLine("Kein Fluss möglich");
                    return;
                }
            }

            // Löschen der SuperQuelle, SuperSenke und den verbindenden Kanten
            FUsedGraph.RemoveNode(hSuperSource);
            FUsedGraph.RemoveNode(hSuperTarget);

            // Den Fluss übernehmen. Dabei aber die Infos zur SuperQuelle und SuperSenke Ignorieren
            var hTotalFlow = 0.0;
            var hGraphEdgeDictionary = FUsedGraph.GenerateEdgeHashDictionary();
            var hFlussGraphDictionary = new Dictionary<string, double>();
            foreach (var hEdgeHash in hGraphEdgeDictionary.Keys)
            {
                hFlussGraphDictionary.Add(hEdgeHash, 0.0);
            }
            foreach (var hFlowInfo in hFlow)
            {
                if (hFlussGraphDictionary.ContainsKey(hFlowInfo.Key))
                {
                    hFlussGraphDictionary[hFlowInfo.Key] = hFlowInfo.Value;
                }
            }



            // Solange es noch negative Zyklen gibt
            var hFoundNegativeCycle = true;
            while (hFoundNegativeCycle)
            {
                hFoundNegativeCycle = false;

                // Residualgraphen erstellen
                var hResidualGraph = ResidualGraphGenerator.GenerateCostCapacity(FUsedGraph, hFlussGraphDictionary);
                var hEdgeHashDictionary = hResidualGraph.GenerateEdgeHashDictionary();
                
                // Ausgehend von jedem Knoten wird versucht ein negativer Zykel zu finden
                foreach (var hNode in hResidualGraph.GetNodeDictionary())
                {
                    // Negativen Zyklus bezüglich den Kosten suchen
                    var hBellmanFordAlgorithm = new BellmanFordAlgorithm(hResidualGraph);
                    hBellmanFordAlgorithm.Execute(hNode.Value.Id);
                    if (hBellmanFordAlgorithm.HasNegativeCycles)
                    {
                        // Negativen Zyklus gefunden
                        hFoundNegativeCycle = true;

                        // ToDo Auf diesem Zyklus die maximale Kapazität ermitteln
                        var hEdgeHashesOfCycle = hBellmanFordAlgorithm.CycleEdges;
                        var hMinCapacityInCycle = Double.PositiveInfinity;
                        foreach (var hEdgeInCycleHash in hEdgeHashesOfCycle)
                        {
                            var hEdgeCapcaityValue = hEdgeHashDictionary[hEdgeInCycleHash].GetWeightValue<CapacityWeighted>();
                            if (hEdgeCapcaityValue < hMinCapacityInCycle)
                            {
                                hMinCapacityInCycle = hEdgeCapcaityValue;
                            }
                        }

                        hTotalFlow += hMinCapacityInCycle;

                        // ToDo Fluss auf dem Originalen Graphen entsprechend anpassen (Orginial Kante, Nicht-Original Kante)
                        foreach (var hEdgeInCycleHash in hEdgeHashesOfCycle)
                        {
                            if (ResidualGraphGenerator.HinkantenEdgeHashes.Contains(hEdgeInCycleHash))
                            {
                                // Hinkante
                                // Diese Kante kann ich im Flussgraphen Dictionary direkt ansprechen da die Konten im Hash in der richtigen Reihenfolge stehen
                                hFlussGraphDictionary[hEdgeInCycleHash] += hMinCapacityInCycle;
                            }
                            else if (ResidualGraphGenerator.RueckKantenEdgeHashes.Contains(hEdgeInCycleHash))
                            {
                                // Bei der Breitensuche im Augmentieren Graphen wurde eine Rückkante verwendet.
                                // Diese Rückkante existiert so im originalen Graphen nicht weshalb auch der Hash-Value nicht passt. 
                                // Dieser ist lediglich verdreht. Also wird der HashWert vorher noch gespiegelt um die ursprüngliche Kante zu identifizieren
                                var hHashValueOfOriginalEdge = HelpFunctions.InvertEdgeHash(hEdgeInCycleHash);
                                hFlussGraphDictionary[hHashValueOfOriginalEdge] -= hMinCapacityInCycle;
                            }
                        } 


                        break; // ToDo Prüfen ob sich das break richtig verhält
                    }
                } 
                
            }

            // Kosten des Flusses ermitteln
            var hFlowCost = 0.0;
            foreach (var hEdgeInFlow in hFlussGraphDictionary)
            {
                var hEdge = hGraphEdgeDictionary[hEdgeInFlow.Key];
                hFlowCost += hEdge.GetWeightValue<CostWeighted>() * hEdgeInFlow.Value;
            }

            Console.WriteLine("Kostenminimaler Fluss:\t" + hFlowCost);


        }

        

    }
}
