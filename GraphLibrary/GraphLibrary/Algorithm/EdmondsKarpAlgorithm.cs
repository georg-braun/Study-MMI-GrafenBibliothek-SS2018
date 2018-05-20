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

        public void Execute(int _StartNodeId, int _TargetNodeId)
        {
            var hStartNode = FUsedGraph.GetNodeDictionary()[_StartNodeId];

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
            var hBfsGraph = hBfsSearch.Execute(hAugmentationsGraph.GetNodeDictionary()[_StartNodeId]);

            // Solange im Augmentationsnetzwerk noch ein Pfad zum Zielknoten gefunden wird.
            while (hBfsGraph.GetNodeDictionary().ContainsKey(_TargetNodeId))
            {
                // Ermittle im Augmentationsgraph den Pfad zum Zielknoten
            }


        }

        public IGraph GenerateAugmentationsGraph(IGraph _SourceGraph)
        {
            IGraph hNewAugmentationsGraph = new Graph();

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
                var hStartNode = hEdgeInSourceGraph.GetPossibleEnpoints()[0];
                var hTargetNode = hEdgeInSourceGraph.GetEdgeSource();
                
                // Hinkante einfügen?
                if (hValueRestkapa > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hStartNode.Id, hTargetNode.Id, new CostWeighted(hValueRestkapa));
                }

                // Rückkante einfügen?
                if (hValueAktuellerFluss > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hTargetNode.Id, hStartNode.Id, new CostWeighted(hValueAktuellerFluss));
                }
            }

            return hNewAugmentationsGraph;

        }
    }
}
