using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    static class ResidualGraphGenerator
    {
        

        public static List<string> HinkantenEdgeHashes { get; set; }

        public static List<string> RueckKantenEdgeHashes { get; set; }


        public static IGraph Generate(IGraph _SourceGraph, Dictionary<string, double> _FlussGraphDictionar)
        {
            IGraph hNewAugmentationsGraph = new Graph();
            HinkantenEdgeHashes = new List<string>();
            RueckKantenEdgeHashes = new List<string>();

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

                var hValueRestkapa = hEdgeInSourceGraphDictEntry.Value.GetWeightValue<CapacityWeighted>() - _FlussGraphDictionar[hEdgeInSourceGraphDictEntry.Key];
                var hValueAktuellerFluss = _FlussGraphDictionar[hEdgeInSourceGraphDictEntry.Key];

                DirectedEdge hEdgeInSourceGraph = (DirectedEdge)hEdgeInSourceGraphDictEntry.Value;
                var hStartNode = hEdgeInSourceGraph.GetEdgeSource();
                var hTargetNode = hEdgeInSourceGraph.GetPossibleEnpoints()[0];

                // Hinkante einfügen?
                if (hValueRestkapa > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hStartNode.Id, hTargetNode.Id, new CapacityWeighted(hValueRestkapa));
                    HinkantenEdgeHashes.Add(hStartNode.Id + "-" + hTargetNode.Id);
                }

                // Rückkante einfügen?
                if (hValueAktuellerFluss > 0.0)
                {
                    hNewAugmentationsGraph.CreateDirectedEdge(hTargetNode.Id, hStartNode.Id, new CapacityWeighted(hValueAktuellerFluss));
                    RueckKantenEdgeHashes.Add(hTargetNode.Id + "-" + hStartNode.Id);
                }
            }

            hNewAugmentationsGraph.UpdateNeighbourInfoInNodes();
            return hNewAugmentationsGraph;

        }

        public static IGraph GenerateCostCapacity(IGraph _SourceGraph, Dictionary<string, double> _FlussGraphDictionar)
        {
            IGraph hNewAugmentationsGraph = new Graph();
            HinkantenEdgeHashes = new List<string>();
            RueckKantenEdgeHashes = new List<string>();

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

                var hValueRestkapa = hEdgeInSourceGraphDictEntry.Value.GetWeightValue<CapacityWeighted>() - _FlussGraphDictionar[hEdgeInSourceGraphDictEntry.Key];
                var hValueAktuellerFluss = _FlussGraphDictionar[hEdgeInSourceGraphDictEntry.Key];
                var hCost = hEdgeInSourceGraphDictEntry.Value.GetWeightValue<CostWeighted>();

                DirectedEdge hEdgeInSourceGraph = (DirectedEdge)hEdgeInSourceGraphDictEntry.Value;
                var hStartNode = hEdgeInSourceGraph.GetEdgeSource();
                var hTargetNode = hEdgeInSourceGraph.GetPossibleEnpoints()[0];

                // Hinkante einfügen?
                if (hValueRestkapa > 0.0)
                {
                    var hNewEdge = hNewAugmentationsGraph.CreateDirectedEdge(hStartNode.Id, hTargetNode.Id, new CapacityWeighted(hValueRestkapa));
                    hNewEdge.AddWeight(new CostWeighted(hCost));
                    HinkantenEdgeHashes.Add(hStartNode.Id + "-" + hTargetNode.Id);
                }

                // Rückkante einfügen?
                if (hValueAktuellerFluss > 0.0)
                {
                    var hNewEdge = hNewAugmentationsGraph.CreateDirectedEdge(hTargetNode.Id, hStartNode.Id, new CapacityWeighted(hValueAktuellerFluss));
                    hNewEdge.AddWeight(new CostWeighted(-1.0*hCost));
                    RueckKantenEdgeHashes.Add(hTargetNode.Id + "-" + hStartNode.Id);
                }
            }

            hNewAugmentationsGraph.UpdateNeighbourInfoInNodes();
            return hNewAugmentationsGraph;

        }




    }
}
