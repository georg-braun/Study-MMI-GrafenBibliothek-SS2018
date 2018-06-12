using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.Algorithm
{
    class HelpFunctions
    {
        public static string InvertEdgeHash(string _BfsPathEdgeHash)
        {
            var hNodes = _BfsPathEdgeHash.Split('-');
            return hNodes[1] + "-" + hNodes[0];
        }
    }
}
