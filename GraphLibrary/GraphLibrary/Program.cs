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
        private const string cMatrixGraphPath = @"e:\Google Drive\Studium\Master\MMI\Praktikum\Beispielgraphen\Graph1.txt";
        static void Main(string[] args)
        {
            MatrixGraphImporter.ImportUnweighted(cMatrixGraphPath);
        }
    }
}
