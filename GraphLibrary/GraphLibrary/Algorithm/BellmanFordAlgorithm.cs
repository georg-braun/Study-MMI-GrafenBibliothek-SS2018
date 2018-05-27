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

        private IGraph FShortestPathGraph;
        public IGraph ShortestPathGraph => FShortestPathGraph;

        public void Execute(int _SourceNodeId, int _TargetNodeId)
        {
            Console.WriteLine("Start Bellman Ford");

            FNodeDictionary = FUsedGraph.GetNodeDictionary();
            var hNodeCount = FNodeDictionary.Count;
            var hStartNode = FNodeDictionary[_SourceNodeId];
            var hTargetNode = FNodeDictionary[_TargetNodeId];


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
                Console.WriteLine("Graph enthält einen negativen Zyklus");
                // ToDo Ausgeben welche negativen Zyklen es gibt
            }
            else
            {
                // Vom Zielknoten rückwärts bis zum Startknoten laufen
                var hTmpNode = hTargetNode;
                var hShortestPathStack = new Stack<int>();
                var hCosts = 0.0;
                while (hTmpNode != hStartNode)
                {
                    hCosts += FParentNodeEdge[hTmpNode].Edge.GetWeightValue();
                    hShortestPathStack.Push(hTmpNode.Id);
                    hTmpNode = FParentNodeEdge[hTmpNode].Node; // Ermitteln des Parents
                }
                hShortestPathStack.Push(hStartNode.Id);

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


                // Ausgabe
                Console.WriteLine("Kürzeste Route:\t" + string.Join(",", hShortestPathStack));
                Console.WriteLine("Kosten:\t" + hCosts);
            }

            
        }

        private bool BellmanFordIteration()
        {
            var hCostsImproved = false;
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
