using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    class NodeEdge
    {
        public INode Node { get; }
        public Edge Edge { get; }

        public NodeEdge(INode _Node, Edge _Edge)
        {
            Node = _Node;
            Edge = _Edge;
        }
    }
}
