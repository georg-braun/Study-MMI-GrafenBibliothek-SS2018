using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    interface IGraph
    {
        Dictionary<int, INode> GetNodeDictionary();

        List<Edge> GetEdgeIndices();

        List<NodeEdge> GetNodeNeighboursAndEdges(INode _Node);

        void UpdateNeighbourInfoInNodes();

        void CreateNewNode(int _Node);

        void CreateDirectedEdge(INode _StartNode, INode _EndNode);

        void CreateDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight);

        void AddNode(INode _Node);

        void AddEdge(Edge _Edge);

        double GetTotalGraphWeight();

        INode GetNodeById(int _NodeId);

        void RemoveEdge(Edge _Edge);
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

        public Dictionary<int, INode> GetNodeDictionary()
        {
            return FNodeIndices;
        }

        public List<Edge> GetEdgeIndices()
        {
            return FEdgeIndices;
        }

        public List<NodeEdge> GetNodeNeighboursAndEdges(INode _Node)
        {
            return _Node.NeighbourEdges;
        }

        public void UpdateNeighbourInfoInNodes()
        {
            foreach (var hNode in FNodeIndices)
            {
                hNode.Value.FindNeighbours(); 
            } 
        }

        public void CreateNewNode(int _NodeId)
        {
            var hNewNode = new Node(_NodeId);
            if (!FNodeIndices.ContainsKey(_NodeId))
            {
                FNodeIndices.Add(hNewNode.Id, hNewNode);
            }
            
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
            if (!FNodeIndices.ContainsKey(_Node.Id))
            {
                FNodeIndices.Add(_Node.Id, _Node);
            }
            
        }

        public void AddEdge(Edge _Edge)
        {
            FEdgeIndices.Add(_Edge);
        }

        public double GetTotalGraphWeight()
        {
            double hWeight = 0;
            foreach (var hEdge in FEdgeIndices)
            {
                if (hEdge.IsWeighted())
                {
                    hWeight = hWeight + hEdge.GetWeightValue();
                }
            }

            return hWeight;
        }

        public INode GetNodeById(int _NodeId)
        {
            return FNodeIndices[_NodeId];
        }

        public void RemoveEdge(Edge _Edge)
        {
            var hEndPoints = _Edge.GetPossibleEnpoints();
            hEndPoints[0].RemoveEdge(_Edge);
            hEndPoints[1].RemoveEdge(_Edge);
            FEdgeIndices.Remove(_Edge);
            UpdateNeighbourInfoInNodes(); // Update der Kanten-Infos
        }
    }
}
