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

        void CreateDirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo);

        void CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight);

        void CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight);

        INode TryToAddNode(INode _Node);

        void AddEdge(Edge _Edge);

        double GetTotalGraphWeight();

        INode GetNodeById(int _NodeId);

        void RemoveEdge(Edge _Edge);

        Edge GetEdge(int _FromNodeId, int _ToNodeId, bool _Undirected);

        
        Dictionary<string, Edge> GenerateEdgeHashDictionary();
    } 

    class Graph : IGraph
    {
        private readonly Dictionary<int, INode> FNodeIndices;

        private readonly List<Edge> FEdgeIndices;

        private Dictionary<string, Edge> FEdgeHashDictionary;

        public Graph()
        {
            FNodeIndices = new Dictionary<int, INode>();
            FEdgeIndices = new List<Edge>();
        }

        public Dictionary<string, Edge> GenerateEdgeHashDictionary()
        {
            if (FEdgeHashDictionary == null)
            {
                FEdgeHashDictionary = new Dictionary<string, Edge>();

                foreach (var hEdge in FEdgeIndices)
                {
                    var hEdgeHashes = hEdge.EdgeHashInfo();

                    foreach (var hEdgeHash in hEdgeHashes)
                    {
                        FEdgeHashDictionary.Add(hEdgeHash, hEdge);
                    }
                }
            }

            return FEdgeHashDictionary;
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

        public void CreateDirectedEdge(int _StartNodeId, int _TargetNodeId, IWeight _Weight)
        {
            var hStartNode = FNodeIndices[_StartNodeId];
            var hTargetNode = FNodeIndices[_TargetNodeId];

            var hNewDirectedEdge = new DirectedEdge(hStartNode, hTargetNode, _Weight);
            FEdgeIndices.Add(hNewDirectedEdge);
            hStartNode.AddEdge(hNewDirectedEdge);
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

        public void CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight)
        {
            var hNodeOne = FNodeIndices[_NodeOneId];
            var hNodeTwo = FNodeIndices[_NodeTwoId];

            var hNewUndirectedEdge = new UndirectedEdge(hNodeOne, hNodeTwo, _Weight);
            FEdgeIndices.Add(hNewUndirectedEdge);
            hNodeOne.AddEdge(hNewUndirectedEdge);
            hNodeTwo.AddEdge(hNewUndirectedEdge);
        }


        /// <summary>
        /// Versucht einen Knoten hinzuzufügen. Ggf. ist dieser schon vorhanden.
        /// </summary>
        /// <param name="_Node"></param>
        /// <returns>Referenz auf den Knoten (hinzugefügter, oder war schon vorhanden)</returns>
        public INode TryToAddNode(INode _Node)
        {
            if (!FNodeIndices.ContainsKey(_Node.Id))
            {
                FNodeIndices.Add(_Node.Id, _Node);
            }

            return FNodeIndices[_Node.Id];


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

        public Edge GetEdge(int _FromNodeId, int _ToNodeId, bool _Undirected)
        {
            var hFromNode = FNodeIndices[_FromNodeId];
            var hToNode = FNodeIndices[_ToNodeId];

            Edge hEdge = null;
            if (_Undirected)
            {
                foreach (var hCurrentEdge in FEdgeIndices)
                {
                    var hPossibleEndpoints = hCurrentEdge.GetPossibleEnpoints();
                    if (hPossibleEndpoints.Contains(hFromNode) && hPossibleEndpoints.Contains(hToNode))
                    {
                        hEdge = hCurrentEdge;
                        break;
                    }
                }
            }
            else
            {
                foreach (var hCurrentEdge in FEdgeIndices)
                {
                    var hPossibleEndpoints = hCurrentEdge.GetPossibleEnpoints();
                    if (hPossibleEndpoints[0] == hFromNode && hPossibleEndpoints[1] == hToNode)
                    {
                        hEdge = hCurrentEdge;
                        break;
                    }
                }
            }

            return hEdge;
        }
    }
}
