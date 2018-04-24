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

            foreach (var hCurrentGraphFile in GraphFileRessources.P2GraphFiles)
            {
                var hNewGraph = AdjacentListGraphImporter.ImportAdjacentList(hCurrentGraphFile, EdgeKind.UndirectedWeighted);
                Console.WriteLine("--- Kruskal ---");
                var hKruskalAlgorithm = new KruskalAlgorithm(hNewGraph);
                hKruskalAlgorithm.Execute();
                Console.WriteLine("");
            }

            Console.ReadLine();

        }

        private void P1Aufgaben()
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
    }

    
}
