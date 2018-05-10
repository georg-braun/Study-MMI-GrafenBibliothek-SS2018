using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class FindSubTrees
    {
        public IGraph UsedGraph { get; }

        private Dictionary<int, List<int>> FSubGraphs;

        private Dictionary<int, bool> FVisitedBfsNodes;

        int FSubGraphId = 0;

        private Stopwatch FStopwatch;

        public FindSubTrees(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
            FSubGraphs = new Dictionary<int, List<int>>();
            FVisitedBfsNodes = new Dictionary<int, bool>();
            FStopwatch = new Stopwatch();
        }

        public List<IGraph> Execute<T>() where T : IGraphTraverseAlgorithm, new()
        {
            FStopwatch.Start();

            var hSubGraphs = new List<IGraph>();

            var hGraphNodes = UsedGraph.GetNodeDictionary();
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
                var hTraverseAlgorithm = new T();
                var hSubGraph = hTraverseAlgorithm.Execute(hUnvisitedNode);
                hSubGraphs.Add(hSubGraph);
                SetSubGraphNodesAsVisited(hSubGraph);
                FSubGraphId++;
            }

            FStopwatch.Stop();
            return hSubGraphs;
        }

        private void SetSubGraphNodesAsVisited(IGraph _SubGraph)
        {
            foreach (var hNode in _SubGraph.GetNodeDictionary())
            {
                var hNodeId = hNode.Value.Id;
                FVisitedBfsNodes[hNodeId] = true;
                AddNodeIdToSubgraph(hNodeId);
            } 
        }

        public void PrintInfosToConsole()
        {
            
            Console.WriteLine("Anzahl der Knoten: " + UsedGraph.GetNodeDictionary().Count.ToString());
            Console.WriteLine("Anzahl der Kanten: " + UsedGraph.GetEdgeIndices().Count.ToString());
            Console.WriteLine("Benötigte Zeit: " + FStopwatch.ElapsedMilliseconds.ToString() + " ms");
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