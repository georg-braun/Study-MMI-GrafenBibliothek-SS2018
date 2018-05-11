using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.Algorithm;
using GraphLibrary.Importer;

namespace GraphLibrary
{
    class Program
    {

        static void Main(string[] args)
        {

            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine("Neuer Graph");
                var hFileName = GraphFileRessources.P3GraphFiles[i];
                var hGraph = AdjacentListGraphImporter.ImportAdjacentList(hFileName, EdgeKind.UndirectedWeighted);

                var hTSPBruteForceAlgorithm = new TSPBruteForce(hGraph);
                hTSPBruteForceAlgorithm.Execute();
                //hTSPBruteForceAlgorithm.TSPBruteIterativ();
            }


            Console.ReadLine();

        }

        private static void P1Aufgaben()
        {
            var hFileName = GraphFileRessources.P1AdjacentGraph3Path;

            var hGraph = AdjacentListGraphImporter.ImportAdjacentList(hFileName, EdgeKind.UndirectedUnweighted);

            FindSubTrees hFindSubTrees = new FindSubTrees(hGraph);
            Console.WriteLine("--- FindSubTrees BFS ---");
            hFindSubTrees.Execute<BreadthFirstSearch>();
            hFindSubTrees.PrintInfosToConsole();

            Console.WriteLine("--- FindSubTrees DFS ---");
            hFindSubTrees.Execute<DepthFirstSearch>();
            hFindSubTrees.PrintInfosToConsole();
        }

        private static void P2Aufgaben()
        {
            foreach (var hCurrentGraphFile in GraphFileRessources.P2GraphFiles)
            {
                var hNewGraph = AdjacentListGraphImporter.ImportAdjacentList(hCurrentGraphFile, EdgeKind.UndirectedWeighted);

                var hPrimAlgorithm = new PrimAlgorithm(hNewGraph);
                var hMstPrim = hPrimAlgorithm.Execute();

                var hKruskalAlgorithm = new KruskalAlgorithm(hNewGraph);
                var hMstKruskal = hKruskalAlgorithm.Execute();
                Console.WriteLine("");
            }
        }

        private static void P3NearestNeighbor()
        {
            for (int i = 0; i < 11; i++)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine("Neuer Graph");
                var hFileName = GraphFileRessources.P3GraphFiles[i];
                var hGraph = AdjacentListGraphImporter.ImportAdjacentList(hFileName, EdgeKind.UndirectedWeighted);

                var hNearestNeighborAlgorithm = new NearestNeighborAlgorithm(hGraph);
                hNearestNeighborAlgorithm.Execute(hGraph.GetNodeDictionary()[0]);
            }
        }

        private static void P3DoppelterBaum()
        {
            for (int i = 0; i < 11; i++)
            {
                Console.WriteLine("-------------------------------");
                Console.WriteLine("Neuer Graph");
                var hFileName = GraphFileRessources.P3GraphFiles[i];
                var hGraph = AdjacentListGraphImporter.ImportAdjacentList(hFileName, EdgeKind.UndirectedWeighted);

                var hDoppelterBaumAlgorithm = new DoppelterBaumAlgorithm(hGraph);
                hDoppelterBaumAlgorithm.Execute(hGraph.GetNodeDictionary()[0]);
            }

        }

    }


}
