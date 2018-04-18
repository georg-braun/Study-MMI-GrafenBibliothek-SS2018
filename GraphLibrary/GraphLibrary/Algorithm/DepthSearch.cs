using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GraphLibrary.DataStructure;

namespace GraphLibrary.Algorithm
{
    class DepthSearch : IGraphAlgorithm
    {
        public IGraph UsedGraph { get; }

        public DepthSearch(IGraph _UsedGraph)
        {
            UsedGraph = _UsedGraph;
        }
        

        public void Execute()
        {
            throw new NotImplementedException();
        }

        public void PrintInfosToConsole()
        {
            throw new NotImplementedException();
        }
    }
}
