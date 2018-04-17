using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{

    interface IGraph
    {
        Dictionary<int, INode> GetNodeIndices();

        List<Edge> GetEdgeIndices();

        List<NodeEdge> GetNodeNeighboursAndEdges(INode _Node);

        void FillNeighbourInfoInNodes();

        void AddNode(INode _Node);

        void AddEdge(Edge _Edge);
    } 

    class Graph : IGraph
    {
        private readonly Dictionary<int, INode> FNodeIndices;

        private readonly List<Edge> FEdgeIndices;

        public Graph()
        {
            FNodeIndices = new Dictionary<int, INode>();
            FEdgeIndices = new List<Edge>();
        }

        public Dictionary<int, INode> GetNodeIndices()
        {
            return FNodeIndices;
        }

        public List<Edge> GetEdgeIndices()
        {
            return FEdgeIndices;
        }

        public List<NodeEdge> GetNodeNeighboursAndEdges(INode _Node)
        {
            return _Node.Neighbours;
        }

        public void FillNeighbourInfoInNodes()
        {
            foreach (var hNode in FNodeIndices)
            {
                hNode.Value.FindNeighbours(); 
            } 
        }

        public void AddNode(INode _Node)
        {
            FNodeIndices.Add(_Node.Id, _Node);
        }

        public void AddEdge(Edge _Edge)
        {
            FEdgeIndices.Add(_Edge);
        }
    }
}
