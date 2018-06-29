using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    
    class MaxMatchingAlgorithm
    {

        private IGraph FUsedGraph;

        private int FNodesGroupALimit;

        public MaxMatchingAlgorithm(IGraph _UsedGraph, int _NodesGroupALimit)
        {
            FUsedGraph = _UsedGraph;
            FNodesGroupALimit = _NodesGroupALimit;
        }

        public void Execute()
        {
            var hNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hNodeCount = hNodeDictionary.Count;

            // Erstellen der SuperQuelle
            var hSuperSource = new Node(hNodeCount);
            FUsedGraph.TryToAddNode(hSuperSource);

            // Erstellen der SuperSenke
            var hSuperTarget = new Node(hNodeCount + 1);
            FUsedGraph.TryToAddNode(hSuperTarget);

            var hPseudoEdges = new List<Edge>();
            // Die SuperQuelle mit den Knoten der Gruppe A verbinden
            for (var hNodeInGroupAId = 0; hNodeInGroupAId < FNodesGroupALimit; hNodeInGroupAId++)  
            {
                var hNodeInGroupA = hNodeDictionary[hNodeInGroupAId];
                var hNewEdge = new DirectedEdge(hSuperSource, hNodeInGroupA);
                hNewEdge.AddWeight(new CapacityWeighted(1));
                FUsedGraph.AddEdge(hNewEdge);
                hPseudoEdges.Add(hNewEdge);
            }

            for (var hNodeInGroupAId = FNodesGroupALimit; hNodeInGroupAId < hNodeCount; hNodeInGroupAId++)
            {
                var hNodeInGroupB = hNodeDictionary[hNodeInGroupAId];
                var hNewEdge = new DirectedEdge(hNodeInGroupB, hSuperTarget);
                hNewEdge.AddWeight(new CapacityWeighted(1));
                FUsedGraph.AddEdge(hNewEdge);
                hPseudoEdges.Add(hNewEdge);
            }

            FUsedGraph.UpdateNeighbourInfoInNodes();
            var hEdmondsKarpAlgorithm = new EdmondsKarpAlgorithm(FUsedGraph);
            var hFlow = hEdmondsKarpAlgorithm.Execute(hSuperSource.Id, hSuperTarget.Id);
            
            
            // PseudoKanten wieder entfernen
            foreach (var hPseudoEdge in hPseudoEdges)
            {
                FUsedGraph.RemoveEdge(hPseudoEdge);
            }
            // Superquelle und -senke wieder entfernen.
            FUsedGraph.RemoveNode(hSuperSource);
            FUsedGraph.RemoveNode(hSuperTarget);
    
            var hFlowCopy = new List<string>(hFlow.Keys);
            foreach (var hFlowEdge in hFlowCopy)
            {
                var hNodes = hFlowEdge.Split('-');
                var hEdgeSourceNodeId = Convert.ToInt32(hNodes[0]);
                var hEdgeTargetNodeId = Convert.ToInt32(hNodes[1]);

                if (hEdgeSourceNodeId == hSuperSource.Id)
                {
                    hFlow.Remove(hFlowEdge);
                }
                else if (hEdgeTargetNodeId == hSuperTarget.Id)
                {
                    hFlow.Remove(hFlowEdge);
                }
                else if (hFlow[hFlowEdge] == 0.0)
                {
                    hFlow.Remove(hFlowEdge);
                }
            } 

            Console.WriteLine("Die Anzahl der maximalen Matchings beträgt\t" + hFlow.Count);

        }
    }
}
