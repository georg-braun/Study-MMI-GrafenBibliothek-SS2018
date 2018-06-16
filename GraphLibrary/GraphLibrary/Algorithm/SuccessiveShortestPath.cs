using System;
using System.Collections.Generic;
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
        }
    }
}
