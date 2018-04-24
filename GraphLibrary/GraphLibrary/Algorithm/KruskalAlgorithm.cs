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
    class KruskalAlgorithm
    {
        private IGraph FUsedGraph;

        private Stopwatch FStopwatch;

        public KruskalAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
            FStopwatch = new Stopwatch();
        }

        public void Execute()
        {
            // ToDo: Ggf. sinnvoll aus dem übergebenen Grafen die Subgrafen zu ermitteln und dann zu wissen welche Knoten darin vorhanden sind
            // Der Algorithmus könnte sonst Probleme bekommen wenn es mehr als eine Zusammenhangskomponente gibt

            FStopwatch.Start();

            // Use a copy of the Edge List. Don't wan't to affect the datastructure
            var hEdges = new List<Edge>(FUsedGraph.GetEdgeIndices());
            var hNodes = FUsedGraph.GetNodeIndices();
            
            var hEdgeWeightComparerAsc = new EdgeWeightComparerAsc();
            hEdges.Sort(hEdgeWeightComparerAsc);

            var hNodeCircleMarkers = new List<int>();
            for (var i = 0; i < hNodes.Count; i++)
            {
                hNodeCircleMarkers.Add(-1);
            }


            double hCosts = 0;
            var hCircleId = 0;
            var hCurrentEdgeId = 0;

            while (hCurrentEdgeId < hEdges.Count)
            {
                var hEdgeEndPoints = hEdges[hCurrentEdgeId].GetPossibleEnpoints();
                var hNodeA = hEdgeEndPoints[0];
                var hNodeB = hEdgeEndPoints[1];

                if ( (hNodeCircleMarkers[hNodeA.Id] == -1) && (hNodeCircleMarkers[hNodeB.Id] ==-1))
                {
                    // Knoten A und B sind noch in keinem Teilgraf. Also kann die Kante rein
                    hNodeCircleMarkers[hNodeA.Id] = hCircleId;
                    hNodeCircleMarkers[hNodeB.Id] = hCircleId;
                    hCircleId++;
                    hCosts += hEdges[hCurrentEdgeId].GetWeightValue();
                }
                else if ( hNodeCircleMarkers[hNodeA.Id] != -1 && (hNodeCircleMarkers[hNodeB.Id] == -1) )
                {
                    // Knoten A ist schon teil eines Teilgrafs, B aber noch nicht. B wird an A Teilgraf angehangen
                    hNodeCircleMarkers[hNodeB.Id] = hNodeCircleMarkers[hNodeA.Id];
                    hCosts += hEdges[hCurrentEdgeId].GetWeightValue();

                }
                else if (hNodeCircleMarkers[hNodeA.Id] == -1 && (hNodeCircleMarkers[hNodeB.Id] != -1))
                {
                    // Knoten B ist schon teil eines Teilgrafs, A aber noch nicht. A wird an B Teilgraf angehangen
                    hNodeCircleMarkers[hNodeA.Id] = hNodeCircleMarkers[hNodeB.Id];
                    hCosts += hEdges[hCurrentEdgeId].GetWeightValue();

                }
                else if (hNodeCircleMarkers[hNodeA.Id] != hNodeCircleMarkers[hNodeB.Id])
                {
                    // A und B sind in unterschiedlichen Teilgrafen, bilden aber keinen Kreis. Also werden die Teilgrafen verschmolzen (B in A)
                    UpdateCircleIdInList(hNodeCircleMarkers, hNodeCircleMarkers[hNodeB.Id], hNodeCircleMarkers[hNodeA.Id]);
                    hCosts += hEdges[hCurrentEdgeId].GetWeightValue();
                }


                if (!hNodeCircleMarkers.Contains(-1))
                {
                    break;
                }

                hCurrentEdgeId++;
            } 

            FStopwatch.Stop();
            Console.WriteLine("Kruskal-Zeit:\t" + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine("Kruskal-Kosten:\t " + hCosts.ToString());
        }

        private void UpdateCircleIdInList(List<int> _List, int _OriginalValue, int _ReplaceValue)
        {
            for (int i = 0; i < _List.Count; i++)
            {
                if (_List[i] == _OriginalValue)
                {
                    _List[i] = _ReplaceValue;
                }
            } 
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

            if (hXWeightValue == hYWeightValue)
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
