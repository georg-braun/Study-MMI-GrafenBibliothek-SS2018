using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.Importer;

namespace GraphLibrary
{
    class Program
    {
        private const string cMatrixGraph1Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph1.txt";
        private const string cAdjacentGraph1Path = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph2.txt";
        
        static void Main(string[] args)
        {
            //MatrixGraphImporter.ImportUnweighted(cMatrixGraphPath);
            AdjacentListGraphImporter.ImportUnweighted(cAdjacentGraph1Path);
        }
    }
}
