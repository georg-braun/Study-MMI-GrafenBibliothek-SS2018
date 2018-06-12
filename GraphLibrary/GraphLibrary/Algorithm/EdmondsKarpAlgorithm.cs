using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class EdmondsKarpAlgorithm
    {

        private IGraph FUsedGraph;

        private Dictionary<string, double> FFlussGraphDictionary;

        public EdmondsKarpAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public Dictionary<string, double> Execute(int _StartNodeId, int _TargetNodeId)
        {
            

            Console.WriteLine("Starte Edmonds Karp Algorithmus");
            var hStartNode = FUsedGraph.GetNodeDictionary()[_StartNodeId];
            double hGesamtfluss = 0.0;

            // Initialisierung Flussgraph
            var hUsedGraphEdgeDictionary = FUsedGraph.GenerateEdgeHashDictionary();
            FFlussGraphDictionary = new Dictionary<string,double>();
            foreach (var hEdgeHash in hUsedGraphEdgeDictionary.Keys)
            {
                FFlussGraphDictionary.Add(hEdgeHash,0.0);
            }
            
            // Erstelle den Residualgraph
            
            var hResidualGraph = ResidualGraphGenerator.Generate(FUsedGraph, FFlussGraphDictionary);
            
            // Eine Breitensuche auf dem Residualgraph
            var hBfsSearch = new BreadthFirstSearch();
            var hBfsGraph = hBfsSearch.Execute<CapacityWeighted>(hResidualGraph.GetNodeDictionary()[_StartNodeId]);

            // Solange im Augmentationsnetzwerk noch ein Pfad zum Zielknoten gefunden wird.
            while (hBfsGraph.GetNodeDictionary().ContainsKey(_TargetNodeId))
            {
                
                // Ermittle im BFS Ergebnis des Augmentationsgraph den Pfad zum Zielknoten und merke den Augmentationswert
                var hBfsGraphParentInfo = hBfsSearch.GetParentNodeEdge();
                var hCurrentNodeIdInBFSPath =_TargetNodeId;
                var hBfsPathEdgeHashesStack = new Stack<string>();
                var hAugmentationsValue = double.PositiveInfinity;
                while (hCurrentNodeIdInBFSPath != _StartNodeId)
                {
                    if (hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Edge.GetWeightValue<CapacityWeighted>() < hAugmentationsValue)
                    {
                        hAugmentationsValue = hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Edge.GetWeightValue<CapacityWeighted>();
                    }
                    // Merken welche Kanten in der BFS Suche involviert waren. Dies wird als "VonKnotenId-NachKnotenId" String gespeichert. Dies entspricht dem EdgeHash
                    hBfsPathEdgeHashesStack.Push(hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Node.Id + "-" + hCurrentNodeIdInBFSPath);
                    hCurrentNodeIdInBFSPath = hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Node.Id; // Ermitteln des Parents
                }

                // Gesamtfluss
                hGesamtfluss += hAugmentationsValue;

                // Bisherigen Flusspfad anpassen
                // Beim zurücklaufen zum Startknoten wurden alle relevanten Kanten besucht und die Hashes der Kanten gespeichert
                foreach (var hBfsPathEdgeHash in hBfsPathEdgeHashesStack)
                {
                    if (ResidualGraphGenerator.HinkantenEdgeHashes.Contains(hBfsPathEdgeHash))
                    {
                        // Bei der Breitensuche im Augmentieren Graphen wurde eine Hinkante verwendet. 
                        // Diese Kante kann ich im Flussgraphen Dictionary direkt ansprechen da die Konten im Hash in der richtigen Reihenfolge stehen
                        FFlussGraphDictionary[hBfsPathEdgeHash] += hAugmentationsValue;
                    }
                    else if (ResidualGraphGenerator.RueckKantenEdgeHashes.Contains(hBfsPathEdgeHash))
                    {
                        // Bei der Breitensuche im Augmentieren Graphen wurde eine Rückkante verwendet.
                        // Diese Rückkante existiert so im originalen Graphen nicht weshalb auch der Hash-Value nicht passt. 
                        // Dieser ist lediglich verdreht. Also wird der HashWert vorher noch gespiegelt um die ursprüngliche Kante zu identifizieren
                        var hHashValueOfOriginalEdge = InvertEdgeHash(hBfsPathEdgeHash);
                        FFlussGraphDictionary[hHashValueOfOriginalEdge] -= hAugmentationsValue;
                    }
                } 

                // Neuer Residualgraph
                hResidualGraph = ResidualGraphGenerator.Generate(FUsedGraph, FFlussGraphDictionary);

                // Bfs darauf
                hBfsSearch = new BreadthFirstSearch();
                hBfsGraph = hBfsSearch.Execute<CapacityWeighted>(hResidualGraph.GetNodeDictionary()[_StartNodeId]);
            }

            Dictionary<string, double> hResultFlow = new Dictionary<string, double>();
            Console.WriteLine("Flussverlauf:");
            foreach (var hFlussInfo in FFlussGraphDictionary)
            {
                if (hFlussInfo.Value > 0.0)
                {
                    Console.WriteLine(hFlussInfo.Key + "\t" + hFlussInfo.Value);
                    hResultFlow.Add(hFlussInfo.Key, hFlussInfo.Value);
                }
            } 
            Console.WriteLine("Gesamtfluss:\t" + hGesamtfluss);

            return hResultFlow;
        }

        private string InvertEdgeHash(string _BfsPathEdgeHash)
        {
            var hNodes = _BfsPathEdgeHash.Split('-');
            return hNodes[1] + "-" + hNodes[0];
        }


        public void PrintEdgesInfo(IGraph _Graph)
        {
            foreach (var _Edge in _Graph.GetEdgeIndices())
            {
                Console.WriteLine(_Edge.ToString() + "\t with " + _Edge.GetWeight<CapacityWeighted>().WeightValue());
                
            } 
        }
    }
}
