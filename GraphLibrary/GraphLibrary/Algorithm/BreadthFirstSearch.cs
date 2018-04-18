using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class BreadthFirstSearch : IGraphAlgorithm
    {
        public IGraph UsedGraph { get; }

        public BreadthFirstSearch(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
        }

        private const int cNodeIsUnvisisted = -1;
        public void Execute()
        {
            var hGraphNodes = UsedGraph.GetNodeIndices();

            var hSubGraphs = new Dictionary<int,int>();   // Knoten -> Subgraph
            LinkedList<int> hUnvisitedNodes = new LinkedList<int>();
            foreach (var hNode in hGraphNodes)
            {
                hSubGraphs.Add(hNode.Key,cNodeIsUnvisisted);
                hUnvisitedNodes.AddLast(hNode.Key);
            }

            var hVisitedNodes = new HashSet<int>(); // ToDo: Notwendig?
            var hSubGraphId = 0;
            // Mache solange bis alle Knoten mal besucht wurden
            while (hUnvisitedNodes.Count > 0)
            {
                var hDepthSearchQueue = new Queue<int>();
                hDepthSearchQueue.Enqueue(hUnvisitedNodes.First.Value);

                while (hDepthSearchQueue.Count > 0)
                {
                    var hCurrentNodeId = hDepthSearchQueue.Dequeue();
                    // Markiere den aktuellen Knoten als besucht
                    hVisitedNodes.Add(hCurrentNodeId);
                    hUnvisitedNodes.Remove(hCurrentNodeId);
                    hSubGraphs[hCurrentNodeId] = hSubGraphId;
                    // Finde die Nachbar Knoten heraus
                    var hNeighbourNodesIds = hGraphNodes[hCurrentNodeId].GetNeighbourIds();
                    // Packe die Nachbarknoten in die Queue
                    foreach (var hNeighbourNodesId in hNeighbourNodesIds)
                    {
                        // Prüfen ob Knoten schon besucht wurde
                        if (!hVisitedNodes.Contains(hNeighbourNodesId))
                        {
                            hDepthSearchQueue.Enqueue(hNeighbourNodesId);
                        }
                    }
                }
                hSubGraphId++;
            }
            // Alle Knoten wurden besucht
            var distinctList = hSubGraphs.Values.Distinct().ToList();
        }
        
    }
}
