using System;
using System.Collections.Generic;
using System.Linq;
using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class FindSubTreesWithBFS
    {
        public IGraph UsedGraph { get; }

        private Dictionary<int, List<int>> FSubGraphs;

        private Dictionary<int, bool> FVisitedBfsNodes;

        int FSubGraphId = 0;

        public FindSubTreesWithBFS(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
            FSubGraphs = new Dictionary<int, List<int>>();
            FVisitedBfsNodes = new Dictionary<int, bool>();
        }

        public List<IGraph> Execute()
        {
            var hSubGraphs = new List<IGraph>();

            var hGraphNodes = UsedGraph.GetNodeIndices();
            FVisitedBfsNodes.Clear();
            FSubGraphs.Clear();
            FSubGraphId = 0;

            foreach (var hNode in hGraphNodes)
            {
                FVisitedBfsNodes.Add(hNode.Key, false);
            }

            while (FVisitedBfsNodes.Values.Contains(false))
            {
                FSubGraphs.Add(FSubGraphId, new List<int>());
                var hUnvisitedNode = hGraphNodes[FindUnvisitedNode(FVisitedBfsNodes)];
                var hBfsSearch = new BreadthFirstSearch(hUnvisitedNode);
                var hSubGraph = hBfsSearch.Execute();
                hSubGraphs.Add(hSubGraph);
                SetSubGraphNodesAsVisited(hSubGraph);
                FSubGraphId++;
            }

            return hSubGraphs;
        }

        private void SetSubGraphNodesAsVisited(IGraph _SubGraph)
        {
            foreach (var hNode in _SubGraph.GetNodeIndices())
            {
                var hNodeId = hNode.Value.Id;
                FVisitedBfsNodes[hNodeId] = true;
                AddNodeIdToSubgraph(hNodeId);
            } 
        }

        public void PrintInfosToConsole()
        {
            Console.WriteLine("--- Breitensuche ---");
            Console.WriteLine("Anzahl der Knoten: " + UsedGraph.GetNodeIndices().Count.ToString());
            Console.WriteLine("Anzahl der Kanten: " + UsedGraph.GetEdgeIndices().Count.ToString());
            Console.WriteLine("Anzahl der Teilgrafen: " + FSubGraphs.Keys.Count.ToString());
            foreach (var hSubGraphList in FSubGraphs)
            {
                Console.WriteLine("\tTeilgraf " + hSubGraphList.Key.ToString()
                                                + ": " + hSubGraphList.Value.Count);
            }
        }

        private void AddNodeIdToSubgraph(int _NodeId)
        {
            FSubGraphs[FSubGraphId].Add(_NodeId);
        }

        private int FindUnvisitedNode(Dictionary<int, bool> dict)
        {
            for (int i = 0; i < dict.Keys.Count; i++)
            {
                if (dict[i] == false)
                {
                    return i;
                }
            } //for (int i = 0; i < UPPER; i++)

            return -1;
        }
    }
}