using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Importer
{


    enum EdgeKind
    {
        UndirectedUnweighted,
        UndirectedWeighted,
        DirectedUnweighted,
        DirectedWeighted,
        DirectedCostCapacityWeighted
    } 

    static class AdjacentListGraphImporter
    {
        /// <summary>
        /// Deprecated!
        /// </summary>
        /// <param name="_FilePath"></param>
        /// <param name="_EdgeKind"></param>
        /// <returns></returns>
        public static IGraph ImportAdjacentList(string _FilePath, EdgeKind _EdgeKind)
        {
            return ImportAdjacentList<CostWeighted>(_FilePath, _EdgeKind);
        }

        public static IGraph ImportAdjacentList<T>(string _FilePath, EdgeKind _EdgeKind) where T : IWeight, new()
        {
            Console.WriteLine("Starte Importieren: " + _FilePath);
            var hStopwatch = new Stopwatch();
            hStopwatch.Start();

            String[] hAdjacentListFileContent = { };
            if (File.Exists(_FilePath))
            {
                hAdjacentListFileContent = File.ReadAllLines(_FilePath);
            }

            // Initiales Anlegen der Knoten.
            var hGraph = new Graph();
            var hNodeCount = Convert.ToInt32(hAdjacentListFileContent[0]);
            for (int hNodeId = 0; hNodeId < hNodeCount; hNodeId++)
            {
                hGraph.CreateNewNode(hNodeId);
            }

            

            // Anlegen der Kanten
            switch (_EdgeKind)
            {
                case EdgeKind.UndirectedUnweighted:
                    ImportUnweightedUndirected(hGraph, hAdjacentListFileContent);
                    break;
                case EdgeKind.UndirectedWeighted:
                    ImportWeightedUndirected<T>(hGraph, hAdjacentListFileContent);
                    break;
                case EdgeKind.DirectedWeighted:
                    ImportWeightedDirected<T>(hGraph, hAdjacentListFileContent);
                    break;
            } //switch (_EdgeKind)
            
            

            hStopwatch.Stop();
            Console.WriteLine("Anzahl eingelesener Knoten:\t" + hGraph.GetNodeDictionary().Count.ToString());
            Console.WriteLine("Anzahl eingelesener Kanten:\t" + hGraph.GetEdgeIndices().Count.ToString());
            Console.WriteLine("Dauer Einlesevorgang:\t\t" + hStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine();

            return hGraph;
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="_Graph"></param>
        /// <param name="_AdjacentListFileContent"></param>
        private static void ImportWeightedDirected(IGraph _Graph, String[] _AdjacentListFileContent)
        {
            ImportWeightedDirected<CostWeighted>(_Graph, _AdjacentListFileContent);
        }

        private static void ImportWeightedDirected<T>(IGraph _Graph, String[] _AdjacentListFileContent) where  T : IWeight, new()
        {
            var hGraphNodes = _Graph.GetNodeDictionary();
            for (int hRowIndex = 1; hRowIndex < _AdjacentListFileContent.Length; hRowIndex++)
            {
                var hRow = _AdjacentListFileContent[hRowIndex];
                var hEdgeInfo = hRow.Split('\t');

                var hNodeAId = Convert.ToInt32(hEdgeInfo[0]);
                var hNodeBId = Convert.ToInt32(hEdgeInfo[1]);
                var hEdgeWeight = Convert.ToDouble(hEdgeInfo[2], CultureInfo.InvariantCulture);

                var hWeightValue = new T();
                hWeightValue.SetValue(hEdgeWeight);

                var hNodeA = hGraphNodes[hNodeAId];
                var hNodeB = hGraphNodes[hNodeBId];

                _Graph.CreateDirectedEdge(hNodeA, hNodeB, hWeightValue);
            }
        }

        private static void ImportUnweightedUndirected(IGraph _Graph, String [] _HAdjacentListFileContentStrings)
        {
            var hGraphNodes = _Graph.GetNodeDictionary();
            for (int hRowIndex = 1; hRowIndex < _HAdjacentListFileContentStrings.Length; hRowIndex++)
            {
                var hRow = _HAdjacentListFileContentStrings[hRowIndex];
                var hEdgeInfo = hRow.Split('\t');

                var hNodeAId = Convert.ToInt32(hEdgeInfo[0]);
                var hNodeBId = Convert.ToInt32(hEdgeInfo[1]);

                var hNodeA = hGraphNodes[hNodeAId];
                var hNodeB = hGraphNodes[hNodeBId];

                _Graph.CreateUndirectedEdge(hNodeA, hNodeB);
            }
        }

        private static void ImportWeightedUndirected(IGraph _Graph, String[] _HAdjacentListFileContentStrings)
        {
            ImportWeightedUndirected<CostWeighted>(_Graph, _HAdjacentListFileContentStrings);
        }

        private static void ImportWeightedUndirected<T>(IGraph _Graph, String[] _HAdjacentListFileContentStrings) where T : IWeight, new()
        {
            var hGraphNodes = _Graph.GetNodeDictionary();
            for (int hRowIndex = 1; hRowIndex < _HAdjacentListFileContentStrings.Length; hRowIndex++)
            {
                var hRow = _HAdjacentListFileContentStrings[hRowIndex];
                var hEdgeInfo = hRow.Split('\t');

                var hNodeAId = Convert.ToInt32(hEdgeInfo[0]);
                var hNodeBId = Convert.ToInt32(hEdgeInfo[1]);
                var hEdgeWeight = Convert.ToDouble(hEdgeInfo[2],CultureInfo.InvariantCulture);

                var hWeightValue = new T();
                hWeightValue.SetValue(hEdgeWeight);

                var hNodeA = hGraphNodes[hNodeAId];
                var hNodeB = hGraphNodes[hNodeBId];

                _Graph.CreateUndirectedEdge(hNodeA, hNodeB, hWeightValue);
            }
        }

        public static IGraph ImportAdjacentListBalancedNodesCostCapacityEdges(string _FilePath)
        {
            Console.WriteLine("Starte Importieren: " + _FilePath);
            var hStopwatch = new Stopwatch();
            hStopwatch.Start();

            String[] hAdjacentListFileContent = { };
            if (File.Exists(_FilePath))
            {
                hAdjacentListFileContent = File.ReadAllLines(_FilePath);
            }

            // Initiales Anlegen der Knoten.
            var hGraph = new Graph();
            var hNodeCount = Convert.ToInt32(hAdjacentListFileContent[0]);
            int hCurrentRow;
            int hCurrentNodeId = 0;
            for (hCurrentRow = 1; hCurrentRow < hNodeCount+1; hCurrentRow++)
            {
                var hNodeBalance = Convert.ToDouble(hAdjacentListFileContent[hCurrentRow], CultureInfo.InvariantCulture);
                hGraph.TryToAddNode(new BalancedNode(hCurrentNodeId, hNodeBalance));
                hCurrentNodeId++;
            }

            var hGraphNodes = hGraph.GetNodeDictionary();

            while (hCurrentRow < hAdjacentListFileContent.Length)
            {
                var hRow = hAdjacentListFileContent[hCurrentRow];
                var hEdgeInfo = hRow.Split('\t');

                var hNodeAId = Convert.ToInt32(hEdgeInfo[0]);
                var hNodeBId = Convert.ToInt32(hEdgeInfo[1]);
                var hCostWeightValue = Convert.ToDouble(hEdgeInfo[2], CultureInfo.InvariantCulture);
                var hCapacityWeightValue = Convert.ToDouble(hEdgeInfo[3], CultureInfo.InvariantCulture);
                
                var hNodeA = hGraphNodes[hNodeAId];
                var hNodeB = hGraphNodes[hNodeBId];
                var hCostWeight = new CostWeighted();
                hCostWeight.SetValue(hCostWeightValue);
                var hCapcacityWeight = new CapacityWeighted();
                hCapcacityWeight.SetValue(hCapacityWeightValue);

                hGraph.CreateDirectedEdge(hNodeA, hNodeB, new IWeight[]{hCostWeight, hCapcacityWeight});

                hCurrentRow++;
            }
            

            hStopwatch.Stop();
            Console.WriteLine("Anzahl eingelesener Knoten:\t" + hGraph.GetNodeDictionary().Count.ToString());
            Console.WriteLine("Anzahl eingelesener Kanten:\t" + hGraph.GetEdgeIndices().Count.ToString());
            Console.WriteLine("Dauer Einlesevorgang:\t\t" + hStopwatch.ElapsedMilliseconds.ToString() + " ms");
            Console.WriteLine();

            return hGraph;
        }
    }
}