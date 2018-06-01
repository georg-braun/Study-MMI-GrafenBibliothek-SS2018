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

        private List<string> FHinkantenEdgeHashes;

        private List<string> FRueckKantenEdgeHashes;

        public EdmondsKarpAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        public void Execute(int _StartNodeId, int _TargetNodeId)
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

            // Erstelle ein Augmentationsnetzwerk
            var hAugmentationsGraph = GenerateAugmentationsGraph(FUsedGraph);

            // Eine Breitensuche auf dem Augmentationsnetzwerk
            var hBfsSearch = new BreadthFirstSearch();
            var hBfsGraph = hBfsSearch.Execute<CapacityWeighted>(hAugmentationsGraph.GetNodeDictionary()[_StartNodeId]);

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
                    if (hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Edge.GetWeightValue() < hAugmentationsValue)
                    {
                        hAugmentationsValue = hBfsGraphParentInfo[hCurrentNodeIdInBFSPath].Edge.GetWeightValue();
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
                    if (FHinkantenEdgeHashes.Contains(hBfsPathEdgeHash))
                    {
                        // Bei der Breitensuche im Augmentieren Graphen wurde eine Hinkante verwendet. 
                        // Diese Kante kann ich im Flussgraphen Dictionary direkt ansprechen da die Konten im Hash in der richtigen Reihenfolge stehen
                        FFlussGraphDictionary[hBfsPathEdgeHash] += hAugmentationsValue;
                    }
                    else if (FRueckKantenEdgeHashes.Contains(hBfsPathEdgeHash))
                    {
                        // Bei der Breitensuche im Augmentieren Graphen wurde eine Rückkante verwendet.
                        // Diese Rückkante existiert so im originalen Graphen nicht weshalb auch der Hash-Value nicht passt. 
                        // Dieser ist lediglich verdreht. Also wird der HashWert vorher noch gespiegelt um die ursprüngliche Kante zu identifizieren
                        var hHashValueOfOriginalEdge = InvertEdgeHash(hBfsPathEdgeHash);
                        FFlussGraphDictionary[hHashValueOfOriginalEdge] -= hAugmentationsValue;
                    }
                } 

                // Neues Augmentationsnetzwerk
                hAugmentationsGraph = GenerateAugmentationsGraph(FUsedGraph);

                // Bfs darauf
                hBfsSearch = new BreadthFirstSearch();
                hBfsGraph = hBfsSearch.Execute(hAugmentationsGraph.GetNodeDictionary()[_StartNodeId]);
            }

            Console.WriteLine("Flussverlauf:");
            foreach (var hFlussInfo in FFlussGraphDictionary)
            {
                if (hFlussInfo.Value > 0.0)
                {
                    Console.WriteLine(hFlussInfo.Key + "\t" + hFlussInfo.Value);
                }
            } 
            Console.WriteLine("Gesamtfluss:\t" + hGesamtfluss);

        }

        private string InvertEdgeHash(string _BfsPathEdgeHash)
        {
            var hNodes = _BfsPathEdgeHash.Split('-');
            return hNodes[1] + "-" + hNodes[0];
        }

        public IGraph GenerateAugmentationsGraph(IGraph _SourceGraph)
        {
            IGraph hNewAugmentationsGraph = new Graph();
            FHinkantenEdgeHashes = new List<string>();
            FRueckKantenEdgeHashes = new List<string>();

            // Erstelle die Knoten
            foreach (var hNode in _SourceGraph.GetNodeDictionary().Values)
            {
                hNewAugmentationsGraph.CreateNewNode(hNode.Id);
            }

            // Füge die Kanten ein (Annahme: Es sind gerichtete Kanten)
            // Nur wenn ein Wert > 0.
            // Hinkante = Restkapazität
            // Rückkante = Aktueller Fluss
            var hSourceGraphEdgeDictionary = _SourceGraph.GenerateEdgeHashDictionary();

            foreach (var hEdgeInSourceGraphDictEntry in hSourceGraphEdgeDictionary)
            {

                var hValueRestkapa = hEdgeInSourceGraphDictEntry.Value.GetWeightValue() - FFlussGraphDictionary[hEdgeInSourceGraphDictEntry.Key];
                var hValueAktuellerFluss = FFlussGraphDictionary[hEdgeInSourceGraphDictEntry.Key];

                DirectedEdge hEdgeInSourceGraph = (DirectedEdge)hEdgeInSourceGraphDictEntry.Value;
                var hStartNode = hEdgeInSourceGraph.GetEdgeSource(); 
                var hTargetNode = hEdgeInSourceGraph.GetPossibleEnpoints()[0];

                // Hinkante einfügen?
                if (hValueRestkapa > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hStartNode.Id, hTargetNode.Id, new CapacityWeighted(hValueRestkapa));
                    FHinkantenEdgeHashes.Add(hStartNode.Id + "-" + hTargetNode.Id);
                }

                // Rückkante einfügen?
                if (hValueAktuellerFluss > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hTargetNode.Id, hStartNode.Id, new CapacityWeighted(hValueAktuellerFluss));
                    FRueckKantenEdgeHashes.Add(hTargetNode.Id + "-" + hStartNode.Id);
                }
            }

            hNewAugmentationsGraph.UpdateNeighbourInfoInNodes();
            return hNewAugmentationsGraph;

        }
    }
}
