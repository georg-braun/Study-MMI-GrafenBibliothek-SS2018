using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class SuccessiveShortestPath
    {
        private IGraph FUsedGraph;

        public SuccessiveShortestPath(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public void Execute()
        {
            var hGraphEdgeHashDictionary = FUsedGraph.GenerateEdgeHashDictionary();
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
  


            var hFlussGraphDictionary = new Dictionary<string, double>();
            foreach (var hEdge in hGraphEdgeHashDictionary)
            {
                var hFlowValue = 0.0;
                var hEdgeCost = hEdge.Value.GetWeightValue<CostWeighted>();
                if (hEdgeCost < 0.0)
                {
                     hFlowValue = hEdge.Value.GetWeightValue<CapacityWeighted>();
                }
                // Negative Kanten direkt voll auslasten
                hFlussGraphDictionary.Add(hEdge.Key, hFlowValue);
            }


            // Pseudo-Balance Initalisieren
            var hPseudoBalanceDictionary = new Dictionary<int, double>();
            foreach (var hNode in hNodeDictionary)
            {
                hPseudoBalanceDictionary.Add(hNode.Key, 0.0);
            }
            
            foreach (var hGraphEdgeHashDictionaryEntry in hGraphEdgeHashDictionary)
            {
                // Wenn es sich um eine Kante mit negativen Kosten handelt
                var hEdgeCost = hGraphEdgeHashDictionaryEntry.Value.GetWeightValue<CostWeighted>();
                if (hEdgeCost < 0.0)
                {
                    var hDirectedEdge = hGraphEdgeHashDictionaryEntry.Value as DirectedEdge;
                    var hTargetNodeId = hDirectedEdge.GetPossibleEnpoints()[0].Id;
                    var hSourceNodeId= hDirectedEdge.GetEdgeSource().Id;
                    
                    // Balance des Ursprungs-Knoten (bzgl. Kante) anpassen
                    hPseudoBalanceDictionary[hSourceNodeId] += hFlussGraphDictionary[hGraphEdgeHashDictionaryEntry.Key];

                    // Balance des Ziel-Knotens (der Kante) anpassen
                    hPseudoBalanceDictionary[hTargetNodeId] -= hFlussGraphDictionary[hGraphEdgeHashDictionaryEntry.Key];
                }
            }

            // Ermitteln der Pseudo-Quellen und Pseudo-Senken
            var hPseudoQuellen = new HashSet<int>();
            var hPseudoSenken = new HashSet<int>();
            foreach (var hNode in hNodeDictionary.Values)
            {
                if (!(hNode is BalancedNode)) throw new InvalidDataException("Nur Balanced Nodes");
                var hBalancedNode = hNode as BalancedNode;
                var hNodeBalance = hBalancedNode.Balance;
                var hPseudoBalance = hPseudoBalanceDictionary[hNode.Id];

                if (hNodeBalance > hPseudoBalance)
                {
                    hPseudoQuellen.Add(hNode.Id);
                }
                else if (hNodeBalance < hPseudoBalance)
                {
                    hPseudoSenken.Add(hNode.Id);
                }
            }

            // Solange es noch Pseudoquellen gibt
            while (hPseudoQuellen.Count > 0)
            {
                // Residualgraphen erstellen
                var hResidualGraph = ResidualGraphGenerator.GenerateCostCapacity(FUsedGraph, hFlussGraphDictionary);
                var hResidualGraphEdgeHashDictionary = hResidualGraph.GenerateEdgeHashDictionary();

                // Eine Pseudoquelle auswählen (z.B. die erste). 
                var hSelectedPseudoQuellenId = hPseudoQuellen.First();
                
                // Versuchen eine erreichbare Senke zu finden (Kürzeste Weg)
                var hFoundPath = false;
                foreach (var hCurrentPseudoSenkenId in hPseudoSenken)
                {
                    // ToDo unnötig. Einmal reicht
                    var hBellmanFordAlgorithm = new BellmanFordAlgorithm(hResidualGraph);
                    hBellmanFordAlgorithm.Execute(hSelectedPseudoQuellenId, hCurrentPseudoSenkenId);
                    if (hBellmanFordAlgorithm.FTargetFound)
                    {
                        hFoundPath = true;

                        // Minimum ermitteln (Pseudo-Quelle, Minimale Kantenkapazität, Pseudo-Senke)
                        // - Pseudo-Quelle
                        var hSelectedPseudoQuelle = hNodeDictionary[hSelectedPseudoQuellenId] as BalancedNode;
                        var hMinPseudoQuelle = hSelectedPseudoQuelle.Balance - hPseudoBalanceDictionary[hSelectedPseudoQuellenId];
                        // - Pseudo-Senke
                        var hCurrentPseudoSenke = hNodeDictionary[hCurrentPseudoSenkenId] as BalancedNode;
                        var hMinPseudoSenke = hPseudoBalanceDictionary[hCurrentPseudoSenkenId] - hCurrentPseudoSenke.Balance;
                        // - Pfad
                        var hMinCapacityInPath = double.PositiveInfinity;
                        foreach (var hPathEdgeHash in hBellmanFordAlgorithm.FShortestPathToTarget)
                        {
                            var hEdgeCapacity = hResidualGraphEdgeHashDictionary[hPathEdgeHash].GetWeightValue<CapacityWeighted>();
                            if (hEdgeCapacity < hMinCapacityInPath)
                            {
                                hMinCapacityInPath = hEdgeCapacity;
                            }
                        }
                        // Ermitteln was das Minimum von den drei ist
                        var hAugmentationValue = Math.Min(hMinCapacityInPath, Math.Min(hMinPseudoQuelle, hMinPseudoSenke));

                        // Fluss anpassen
                        foreach (var hPathEdgeHash in hBellmanFordAlgorithm.FShortestPathToTarget)
                        {
                            if (ResidualGraphGenerator.HinkantenEdgeHashes.Contains(hPathEdgeHash))
                            {
                                // Hinkante
                                // Diese Kante kann ich im Flussgraphen Dictionary direkt ansprechen da die Konten im Hash in der richtigen Reihenfolge stehen
                                hFlussGraphDictionary[hPathEdgeHash] += hAugmentationValue;
                            }
                            else if (ResidualGraphGenerator.RueckKantenEdgeHashes.Contains(hPathEdgeHash))
                            {
                                // Bei der Breitensuche im Augmentieren Graphen wurde eine Rückkante verwendet.
                                // Diese Rückkante existiert so im originalen Graphen nicht weshalb auch der Hash-Value nicht passt. 
                                // Dieser ist lediglich verdreht. Also wird der HashWert vorher noch gespiegelt um die ursprüngliche Kante zu identifizieren
                                var hHashValueOfOriginalEdge = HelpFunctions.InvertEdgeHash(hPathEdgeHash);
                                hFlussGraphDictionary[hHashValueOfOriginalEdge] -= hAugmentationValue;
                            }
                        }

                        // PseudoBalance anpassen
                        hPseudoBalanceDictionary[hSelectedPseudoQuellenId] += hAugmentationValue;
                        hPseudoBalanceDictionary[hCurrentPseudoSenkenId] -= hAugmentationValue;

                        // ggf. Pseudo-Quelle oder Pseudo-Senke entfernen
                        // ToDo Nur die betrachten, welche auch in der Pfad-Detektion verwendet wurde
                        hPseudoQuellen.RemoveWhere(
                            _PseudoQuellenId =>
                            {
                                var hPseudoQuelle = hNodeDictionary[_PseudoQuellenId] as BalancedNode;
                                if ((hPseudoBalanceDictionary[_PseudoQuellenId] - hPseudoQuelle.Balance) == 0)
                                {
                                    return true;
                                }
                                return false;
                            }
                        );

                   
                        hPseudoSenken.RemoveWhere(
                            _PseudoSenkenId =>
                            {
                                var hPseudoSenke = hNodeDictionary[_PseudoSenkenId] as BalancedNode;
                                if ((hPseudoBalanceDictionary[_PseudoSenkenId] - hPseudoSenke.Balance) == 0)
                                {
                                    return true;
                                }
                                return false;
                            });


                        // Es wurde eine Senke gefunden. Also kann eine neue Pseudo-Quelle ausgewählt werden
                        break;
                    }
                }

                if (!hFoundPath)
                {
                    // Wird keine erreichbare Senke gefunden so kann der Algortihmus aufhören. Es gibt keinen B-Fluss
                    Console.WriteLine("Von der Pseudo-Quelle " + hSelectedPseudoQuellenId + " aus konnte keine Pseudo-Senke erreicht werden.");
                    return;
                }
                
            }

            // Jetzt sollte es keine Pseudo-Quellen mehr geben.
            // Ausgabe des Kostenminimalen Flusses
            var hFlowCost = 0.0;
            foreach (var hEdgeInFlow in hFlussGraphDictionary)
            {
                var hEdge = hGraphEdgeHashDictionary[hEdgeInFlow.Key];
                hFlowCost += hEdge.GetWeightValue<CostWeighted>() * hEdgeInFlow.Value;
            }

            Console.WriteLine("Kostenminimaler Fluss:\t" + hFlowCost);

        }
    }
}
