using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    interface INode
    {
        
        int Id { get; }

        List<Tuple<INode, Edge>> Neighours { get; set; }

        void AddEdge(Edge _Edge);

        void FindNeighbours();
    }

    class Node : INode
    {
        private List<Edge> FEdges;

        public int Id { get; }

        public List<Tuple<INode, Edge>> Neighours { get; set; }

        public Node(int _Id)
        {
            Id = _Id;
            FEdges = new List<Edge>();
        }

        

        public void AddEdge(Edge _Edge)
        {
            FEdges.Add(_Edge);
        }


        /// <summary>
        ///  Inspiziert die Kanten und sichert diese mit den entsprechend gefunden Nachbarknoten ab.
        /// </summary>
        public void FindNeighbours()
        {
            Neighours = new List<Tuple<INode, Edge>>();

            var hNeighbours = new List<INode>();
            foreach (var hEdge in FEdges)
            {
                var hPossibleNeighbour = hEdge.GetPossibleEndpoint(this);
                if (hPossibleNeighbour != null)
                {
                    var hNewNeighourInfo = new Tuple<INode,Edge>(hPossibleNeighbour,hEdge);
                    Neighours.Add(hNewNeighourInfo);
                }
            } 

        }
    }
}
