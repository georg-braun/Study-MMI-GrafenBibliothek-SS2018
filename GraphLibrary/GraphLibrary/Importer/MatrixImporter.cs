using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Importer
{
    static class MatrixGraphImporter
    {
        
        public static IGraph ImportUnweighted(string _FilePath)
        {
            String[] hGraphMatrixFileContent = { };
            if (File.Exists(_FilePath))
            {
                hGraphMatrixFileContent = File.ReadAllLines(_FilePath);
            }

            var hGraph = new Graph();
            
            // Initiales Anlegen der Knoten. Zeilennummer entspricht Knoten Index
            for (int hMatrixRowIdx = 1; hMatrixRowIdx < hGraphMatrixFileContent.Length; hMatrixRowIdx++)
            {
                var hNewNodeId = hMatrixRowIdx - 1;
                hGraph.CreateNewNode(hNewNodeId);
            }

            var hGraphNodes = hGraph.GetNodeIndices();


            // Erneutes durchlaufen der Matrix um die Knoten zu verbinden. 
            // Symmetrieeigenschaft der Matrix nutzen, sodass nicht alle Spalten gelesen werden müssen.
            // Dies verhindert auch Duplikate Kanten
            // \ ? ?
            // 1 \ ?
            // 1 2 \
            int hReadTillColumn = 0; 
            for (int hMatrixRowIdx = 1; hMatrixRowIdx < hGraphMatrixFileContent.Length; hMatrixRowIdx++)
            {
                var hCurrentNodeId = hMatrixRowIdx - 1;
                var hCurrentNodeMatrixLine = hGraphMatrixFileContent[hMatrixRowIdx];
                var hCurrentNodeConnectedNodes = hCurrentNodeMatrixLine.Split('\t');
                
                // Prüfen welche Knoten mit dem aktuellen verbunden sind
                for (var hMatrixColumnIdx = 0 ; hMatrixColumnIdx < hReadTillColumn; hMatrixColumnIdx++)
                {
                    var hNodeConnectionInfo = hCurrentNodeConnectedNodes[hMatrixColumnIdx];                    
                    if (hNodeConnectionInfo != "0")
                    {
                        var hNewNeighbourNodeId = hMatrixColumnIdx;
                        hGraph.AddUndirectedEdge(hGraphNodes[hCurrentNodeId],hGraphNodes[hNewNeighbourNodeId]);
                    }
                }
                hReadTillColumn++; // Die nächste Zeile kann eine Spalte weiter gelesen werden
            }
            
            return hGraph;
        }
        
    }
}
