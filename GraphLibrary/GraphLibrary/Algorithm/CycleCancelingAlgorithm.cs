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
            var hSuperTarget = new BalancedNode(hNodeCount+1, 0.0);
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
                    FUsedGraph.AddEdge(hNewEdge);   // Todo: Prüfen ob es richtig funktioniert
                }
                else if (hNodeBalance < 0.0)
                {
                    // Es ist eine Senke. Dieser Knoten soll nun eine Kante zur SuperSenke haben. Die Kapazität der Kante entspricht der Balance des Knotens
                    var hNewEdge = new DirectedEdge(hNode, hSuperTarget);
                    hNewEdge.AddWeight(new CapacityWeighted(-1.0 * hNodeBalance));
                    FUsedGraph.AddEdge(hNewEdge);   // Todo: Prüfen ob es richtig funktioniert
                }
            } 

            // Todo: Ggf. noch Knoten aktualisieren?
            

            // Nun soll der Edmonds-Karp Algorithmus ausgeführt werden. Dabei werden die SuperQuelle und SuperSenke verwendet
            var hEdmondsKarpAlgorithm = new EdmondsKarpAlgorithm(FUsedGraph);
            var hFlow = hEdmondsKarpAlgorithm.Execute(hSuperSource.Id, hSuperTarget.Id);
        }

    }
}
