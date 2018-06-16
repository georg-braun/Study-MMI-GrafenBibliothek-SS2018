using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{

    class BellmanFordAlgorithm
    {

        private IGraph FUsedGraph;

        private Dictionary<INode, double> FCostDictionary = new Dictionary<INode, double>();
        private Dictionary<INode, NodeEdge> FParentNodeEdge = new Dictionary<INode, NodeEdge>(); // Speichert den "Vaterknoten" und dessen Kante
        private Dictionary<int, INode> FNodeDictionary;

        public BellmanFordAlgorithm(IGraph _UsedGraph)
        {
            FUsedGraph = _UsedGraph;
        }

        private bool FHasNegativeCylces;
        public bool HasNegativeCycles => FHasNegativeCylces;

        public bool FTargetFound = false;

        public List<string> FShortestPathToTarget;

        private IGraph FShortestPathGraph;
        public IGraph ShortestPathGraph => FShortestPathGraph;

        private List<int> FCycleNodes;
        public List<int> CycleNodes => FCycleNodes;

        public List<string> CycleEdges;

        public double CycleCosts { get; set; } = 0.0;

        public void Execute(int _SourceNodeId)
        {
            Execute(_SourceNodeId, -1, true);
        }

        public void Execute(int _SourceNodeId, int _TargetNodeId, bool _IgnoreTarget = false)
        {
            //Console.WriteLine("Start Bellman Ford");

            FNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hNodeCount = FNodeDictionary.Count;
            var hStartNode = FNodeDictionary[_SourceNodeId];
            


            // Initialisierung
            FHasNegativeCylces = false;
            foreach (var hNode in FNodeDictionary.Values)
            {
                FCostDictionary.Add(hNode,Double.PositiveInfinity);
                FParentNodeEdge.Add(hNode,null);
            }

            FCostDictionary[hStartNode] = 0.0;

            // Bellman Ford Iterationen: |V|-1
            var hBellmanFordRanToEnd = true;
            for (int hBellmanFordIteration = 0; hBellmanFordIteration < hNodeCount-1; hBellmanFordIteration++)
            {
                var hCostsImproved = BellmanFordIteration();
                // Wenn in einer Iteration keine Verbesserung mehr stattfindet müssen die folgenden auch nicht mehr betrachtet werden
                if (!hCostsImproved)
                {
                    hBellmanFordRanToEnd = false;
                    break;
                }
            }

            // Detektion von Negativen Zyklen:
            // Wenn |V|-1 Durchläufe gemacht wurden wird jetzt noch ein Durchlauf gemacht um zu prüfen ob es einen negativen Zyklus gibt.
            var hGraphHasNegativeCycle = false;
            if (hBellmanFordRanToEnd)
            {
                var hCostsImproved = BellmanFordIteration();
                hGraphHasNegativeCycle = hCostsImproved;
            }

 

            // Ausgabe negativer Zyklus oder Ermittlung des Pfads
            if (hGraphHasNegativeCycle)
            {
                FHasNegativeCylces = true;

                // Bei einem Zyklus laufe ich solange die Parents durch bis ich wieder beim ursprünglichen Knoten auskomme
                // Ermitteln der negativen Zyklen
                var hAvailableNodes = new List<INode>(FParentNodeEdge.Keys);
                var hCycleList = new List<int>();
                var hCycleDetected = false;
                var hCycleCosts = 0.0;
                CycleEdges = new List<string>();

                while (!hCycleDetected)
                {
                    // Startknoten auswählen, dessen Parent nicht null ist (=> kann bei >= 2 Zusammenhangskomponenten auftreten
                    INode hStartNodeInCycleDetection = hAvailableNodes[0];
                    foreach (var hNode in hAvailableNodes)
                    {
                        if (FParentNodeEdge[hNode] != null)
                        {
                            hStartNodeInCycleDetection = hNode;
                            break;
                        }
                    } 
                    
                    var hCurrentNode = hStartNodeInCycleDetection;
                    hCycleList.Clear();
                    CycleEdges.Clear();
                    hCycleCosts = 0.0;

                    // Pfad entlang laufen
                    while (hCurrentNode != null)
                    {
                        // Prüfen ob der Knoten im aktuellen Pfad schon vorkam
                        if (hCycleList.Contains(hCurrentNode.Id))
                        {
                            if (hCycleList[0] == hCurrentNode.Id)
                            {
                                // Zyklus fertig. Beim Startknoten geschlossen
                                // Wert zurückgeben
                                hCycleDetected = true;
                            }
                            else
                            {
                                // Zyklus vorhanden, aber es sind noch Nicht-Zyklus-Knoten vorhanden
                                hAvailableNodes.Remove(hStartNodeInCycleDetection);
                            }
                            break;
                        }
                        else
                        {
                            // Knoten in den aktuellen Pfad einfügen
                            hCycleList.Add(hCurrentNode.Id);
                            CycleEdges.AddRange(FParentNodeEdge[hCurrentNode].Edge.EdgeHashInfo());
                            hCycleCosts += (FParentNodeEdge[hCurrentNode].Edge.GetWeightValue());
                            // Nächsten Knoten betrachten
                            hCurrentNode = FParentNodeEdge[hCurrentNode].Node;
                        }
                        
                    }

                    // Wenn der aktuelle Knoten == null ist gab es von dem ersten Knoten aus keinen Zyklus
                   hAvailableNodes.Remove(hStartNodeInCycleDetection);
                }

                // Zyklus gefunden. Die Elemente noch in eine richtige Reihenfolge bringen
                hCycleList.Reverse();
                FCycleNodes = hCycleList;
                CycleCosts = hCycleCosts;

                // Ausgabe: 
                //Console.WriteLine("Graph enthält einen negativen Zyklus");
                //Console.WriteLine(string.Join(",", FCycleNodes));
                //Console.WriteLine("Zykluskosten: " + CycleCosts);

            }
            else
            {
                if (!_IgnoreTarget && (FParentNodeEdge[FNodeDictionary[_TargetNodeId]] != null))
                {
                    FShortestPathToTarget = new List<string>();
                    var hTargetNode = FNodeDictionary[_TargetNodeId];
                    // Vom Zielknoten rückwärts bis zum Startknoten laufen
                    var hTmpNode = hTargetNode;
                    var hShortestPathStack = new Stack<int>();
                    var hCosts = 0.0;
                    while (hTmpNode != hStartNode)
                    {
                        FShortestPathToTarget.Add(FParentNodeEdge[hTmpNode].Node.Id.ToString()+ "-" + hTmpNode.Id.ToString());
                        hCosts += FParentNodeEdge[hTmpNode].Edge.GetWeightValue();
                        hShortestPathStack.Push(hTmpNode.Id);
                        hTmpNode = FParentNodeEdge[hTmpNode].Node; // Ermitteln des Parents
                    }

                    FTargetFound = true;
                    hShortestPathStack.Push(hStartNode.Id);
                    FShortestPathToTarget.Reverse();

                    // Ausgabe
                    //Console.WriteLine("Kürzeste Route:\t" + string.Join(",", hShortestPathStack));
                    //Console.WriteLine("Kosten:\t" + hCosts);
                }
                

                // Kürzeste Wege Baum erstellen
                // Neues Grafen Objekt erstellen und die Knoten anlegen
                IGraph hNewGraph = new Graph();
                foreach (var hParentNodeEdge in FParentNodeEdge)
                {
                    hNewGraph.CreateNewNode(hParentNodeEdge.Key.Id);
                }
                foreach (var hParentNodeEdge in FParentNodeEdge)
                {
                    // Startknoten hat keinen Value
                    if (hParentNodeEdge.Value != null)
                    {
                        IWeight hNewWeight = null;
                        if (hParentNodeEdge.Value.Edge.IsWeighted())
                        {
                            if (hParentNodeEdge.Value.Edge.GetWeight() is CostWeighted)
                            {
                                hNewWeight = new CostWeighted(hParentNodeEdge.Value.Edge.GetWeightValue());
                            }
                        }
                        else
                        {
                            hNewWeight = new Unweighted();
                        }


                        var hEdge = hParentNodeEdge.Value.Edge;
                        if (hEdge is DirectedEdge)
                        {
                            hNewGraph.CreateDirectedEdge(hParentNodeEdge.Value.Node.Id, hParentNodeEdge.Key.Id, hNewWeight);
                        }
                        else if (hEdge is UndirectedEdge)
                        {
                            hNewGraph.CreateUndirectedEdge(hParentNodeEdge.Value.Node.Id, hParentNodeEdge.Key.Id, hNewWeight);
                        }
                    }
                    
                }
                hNewGraph.UpdateNeighbourInfoInNodes();
                FShortestPathGraph = hNewGraph;



            }

            
        }

        private bool BellmanFordIteration()
        {
            var hCostsImproved = false;
            // Für alle Kanten( Für jeden Knoten -> Alle möglichen Kanten des Knotens)
            // Für jeden vorhandenen Knoten..
            foreach (var hCurrentNode in FNodeDictionary.Values)
            {
                // Für jede ausgehende Kante..
                foreach (var hCurrentNodeEdge in hCurrentNode.NeighbourEdges)
                {
                    var hNeighborNode = hCurrentNodeEdge.Node;
                    var hEdgeToNeighorNode = hCurrentNodeEdge.Edge;
                    var hCostToNeighbor = FCostDictionary[hCurrentNode] + hEdgeToNeighorNode.GetWeightValue();

                    // Prüfen ob ich den Nachbarn jetzt günstiger erreichen kann
                    if (hCostToNeighbor < FCostDictionary[hNeighborNode])
                    {
                        FCostDictionary[hNeighborNode] = hCostToNeighbor;
                        FParentNodeEdge[hNeighborNode] = new NodeEdge(hCurrentNode, hEdgeToNeighorNode);
                        hCostsImproved = true;
                    }
                }
            }

            return hCostsImproved;
        }
    }
}
