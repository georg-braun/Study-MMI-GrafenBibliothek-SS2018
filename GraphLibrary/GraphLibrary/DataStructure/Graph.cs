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

        void CreateDirectedEdge(INode _StartNode, INode _EndNode);

        void CreateDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight);

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
            return _Node.NeighboursEdges;
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

        public void CreateDirectedEdge(INode _StartNode, INode _EndNode)
        {
            CreateDirectedEdge(_StartNode, _EndNode, new Unweighted());
        }

        public void CreateDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight)
        {
            var hNewDirectedEdge = new DirectedEdge(_StartNode, _EndNode, _Weight);
            // Todo: Checken dass keine Duplikate entstehen?
            FEdgeIndices.Add(hNewDirectedEdge);
            _StartNode.AddEdge(hNewDirectedEdge);
            
        }

        public void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo)
        {
            CreateUndirectedEdge(_NodeOne, _NodeTwo, new Unweighted());
        }

        public void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight)
        {
            var hNewUndirectedEdge = new UndirectedEdge(_NodeOne, _NodeTwo, _Weight);
            FEdgeIndices.Add(hNewUndirectedEdge);
            _NodeOne.AddEdge(hNewUndirectedEdge);
            _NodeTwo.AddEdge(hNewUndirectedEdge);

            
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
