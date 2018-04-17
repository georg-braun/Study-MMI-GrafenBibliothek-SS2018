﻿using System;
using System.IO;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Importer
{
    static class AdjacentListGraphImporter
    {
        public static IGraph ImportUnweighted(string _FilePath)
        {
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
            var hGraphNodes = hGraph.GetNodeIndices();
            for (int hRowIndex = 1; hRowIndex < hAdjacentListFileContent.Length; hRowIndex++)
            {
                var hRow = hAdjacentListFileContent[hRowIndex];
                var hEdgeInfo = hRow.Split('\t');

                var hStartNodeId = Convert.ToInt32(hEdgeInfo[0]);
                var hEndNodeId = Convert.ToInt32(hEdgeInfo[1]);

                var hStartNode = hGraphNodes[hStartNodeId];
                var hEndNode = hGraphNodes[hEndNodeId];

                hGraph.AddDirectedEdge(hStartNode,hEndNode);
            }


            return hGraph;
        }

    }
}