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
        private const string cMatrixGraph1Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph1.txt";
        private const string cAdjacentGraph2Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph2.txt";
        private const string cAdjacentGraph3Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph3.txt";
        private const string cAdjacentGraph4Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph4.txt";
        static void Main(string[] args)
        {

            var hFileName = cAdjacentGraph4Path;
            
            Console.WriteLine("Importiere Graph: " + hFileName);
            
            var hGraph = AdjacentListGraphImporter.ImportUnweighted(cAdjacentGraph4Path);

            var hBreadthFirstSearch = new BreadthFirstSearch(hGraph);
            hBreadthFirstSearch.Execute();
            hBreadthFirstSearch.PrintInfosToConsole();

            Console.ReadLine();

        }
    }
}
