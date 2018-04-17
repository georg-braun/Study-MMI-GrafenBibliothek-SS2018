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

        void CreateNewNode(int _Node);

        void AddDirectedEdge(INode _StartNode, INode _EndNode);

        void AddDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight);

        void AddUndirectedEdge(INode _NodeOne, INode _NodeTwo);

        void AddUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight);

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

        public void CreateNewNode(int _NodeId)
        {
            var hNewNode = new Node(_NodeId);
            FNodeIndices.Add(hNewNode.Id, hNewNode);
        }

        public void AddDirectedEdge(INode _StartNode, INode _EndNode)
        {
            AddDirectedEdge(_StartNode, _EndNode, new Unweighted());
        }

        public void AddDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight)
        {
            var hNewDirectedEdge = new DirectedEdge(_StartNode, _EndNode, _Weight);
            // Todo: Checken dass keine Duplikate entstehen?
            FEdgeIndices.Add(hNewDirectedEdge);
            
        }

        public void AddUndirectedEdge(INode _NodeOne, INode _NodeTwo)
        {
            AddUndirectedEdge(_NodeOne, _NodeTwo, new Unweighted());
        }

        public void AddUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight)
        {
            var hNewUndirectedEdge = new UndirectedEdge(_NodeOne, _NodeTwo, _Weight);
            // Todo: Checken dass keine Duplikate entstehen?
            FEdgeIndices.Add(hNewUndirectedEdge);

            
        }


        
    }
}
