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

        INode CreateNewNode(int _Node);

        void CreateDirectedEdge(INode _StartNode, INode _EndNode);

        void CreateDirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight);

        Edge CreateDirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight);

        Edge CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo);

        Edge CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId);

        Edge CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight);

        Edge CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight);

        INode TryToAddNode(INode _Node);

        void AddEdge(Edge _Edge);

        double GetTotalGraphWeight();

        INode GetNodeById(int _NodeId);

        void RemoveEdge(Edge _Edge);

        Edge GetEdge(int _FromNodeId, int _ToNodeId, bool _Undirected);

        Edge GetEdge(string _EdgeHash);

        Dictionary<string, Edge> GenerateEdgeHashDictionary();

        void RemoveNode(INode _Node);
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

        public Edge GetEdge(string _EdgeHash)
        {
            return FEdgeHashDictionary[_EdgeHash];
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

        public INode CreateNewNode(int _NodeId)
        {
            var hNewNode = new Node(_NodeId);
            if (!FNodeIndices.ContainsKey(_NodeId))
            {
                FNodeIndices.Add(hNewNode.Id, hNewNode);
            }

            return hNewNode;

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

        public void CreateDirectedEdge(INode _StartNode, INode _EndNode, IWeight[] _Weight)
        {
            var hNewDirectedEdge = new DirectedEdge(_StartNode, _EndNode);
            foreach (var hWeight in _Weight)
            {
                hNewDirectedEdge.AddWeight(hWeight);
            } 
            // Todo: Checken dass keine Duplikate entstehen?
            FEdgeIndices.Add(hNewDirectedEdge);
            _StartNode.AddEdge(hNewDirectedEdge);

        }

        public Edge CreateDirectedEdge(int _StartNodeId, int _TargetNodeId, IWeight _Weight)
        {
            var hStartNode = FNodeIndices[_StartNodeId];
            var hTargetNode = FNodeIndices[_TargetNodeId];

            var hNewDirectedEdge = new DirectedEdge(hStartNode, hTargetNode, _Weight);
            FEdgeIndices.Add(hNewDirectedEdge);
            hStartNode.AddEdge(hNewDirectedEdge);

            return hNewDirectedEdge;
        }

        public Edge CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo)
        {
            return CreateUndirectedEdge(_NodeOne, _NodeTwo, new Unweighted());
        }

        public Edge CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId)
        {
            var hNodeOne = FNodeIndices[_NodeOneId];
            var hNodeTwo = FNodeIndices[_NodeTwoId];

            return CreateUndirectedEdge(hNodeOne, hNodeTwo);
        }

        public Edge CreateUndirectedEdge(INode _NodeOne, INode _NodeTwo, IWeight _Weight)
        {
            var hNewUndirectedEdge = new UndirectedEdge(_NodeOne, _NodeTwo, _Weight);
            FEdgeIndices.Add(hNewUndirectedEdge);
            _NodeOne.AddEdge(hNewUndirectedEdge);
            _NodeTwo.AddEdge(hNewUndirectedEdge);

            return hNewUndirectedEdge;
        }

        public Edge CreateUndirectedEdge(int _NodeOneId, int _NodeTwoId, IWeight _Weight)
        {
            var hNodeOne = FNodeIndices[_NodeOneId];
            var hNodeTwo = FNodeIndices[_NodeTwoId];

            var hNewUndirectedEdge = new UndirectedEdge(hNodeOne, hNodeTwo, _Weight);
            FEdgeIndices.Add(hNewUndirectedEdge);
            hNodeOne.AddEdge(hNewUndirectedEdge);
            hNodeTwo.AddEdge(hNewUndirectedEdge);

            return hNewUndirectedEdge;
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

            
            // Die Kante noch in den Knoten ergänzen
            if (_Edge is DirectedEdge)
            {
                var hStartNode = ((DirectedEdge)_Edge).GetEdgeSource();
                hStartNode.AddEdge(_Edge);
            }
            else if (_Edge is UndirectedEdge)
            {
                var hEndPoints = _Edge.GetPossibleEnpoints();
                var hNodeA = hEndPoints[0];
                var hNodeB = hEndPoints[1];
                hNodeA.AddEdge(_Edge);
                hNodeB.AddEdge(_Edge);
            }

            UpdateNeighbourInfoInNodes();

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

        /// <summary>
        /// Entfernt den Knoten und seine Kanten zu anderne Knoten
        /// </summary>
        /// <param name="_Node"></param>
        public void RemoveNode(INode _Node)
        {
            // Alle Kanten entfernen die vom zu entfernenden Knoten abgehen
            foreach (var hNeighborEdges in _Node.NeighbourEdges)
            {
                FEdgeIndices.Remove(hNeighborEdges.Edge);
            }
            // Alle Kanten entfernen die auf zu entfernenden Knoten zeigen
            var hEdgesToRemove = new List<Edge>();
            foreach (var hEdge in FEdgeIndices)
            {
                var hEndpoints = hEdge.GetPossibleEnpoints();
                if (hEndpoints.Contains(_Node))
                {
                    hEdgesToRemove.Add(hEdge);
                }
            }
            foreach (var hEdge in hEdgesToRemove)
            {
                FEdgeIndices.Remove(hEdge);
            }
            
            // Knoten selber entfernen
            FNodeIndices.Remove(_Node.Id);
            UpdateNeighbourInfoInNodes();

            FEdgeHashDictionary.Clear();
            FEdgeHashDictionary = null;
            GenerateEdgeHashDictionary();
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
