using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    abstract class Edge
    {
        private List<IWeight> _weights;
        private List<INode> _endpoints;


        protected Edge(List<IWeight> weights)
        {
            this._weights = new List<IWeight>();
            this._endpoints = new List<INode>();
            _weights = weights;
        }

        public abstract INode GetPossibleEndpoint(INode node);

        public List<IWeight> GetWeights()
        {
            return _weights;
        }

    }

    class UndirectedEdge : Edge
    {
        private INode _nodeA;
        private INode _nodeB;

        public UndirectedEdge(INode nodeA, INode nodeB, List<IWeight> weights) : base(weights)
        {
            _nodeA = nodeA;
            _nodeB = nodeB;
        }

        /// <summary>
        /// Gibt einen erreichbaren Knoten zurück. Richtung ist egal
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Der verbundene Knoten wenn der übergebene Knoten auch in der Kante steckt</returns>
        public override INode GetPossibleEndpoint(INode node)
        {
            INode hPossibleEndpoint = null;
            if (node == _nodeA)
            {
                hPossibleEndpoint = _nodeB;
            }
            else if (node == _nodeB)
            {
                hPossibleEndpoint = _nodeA;
            }

            return hPossibleEndpoint;
        }
    }

    class DirectedEdge : Edge
    {
        private INode _startNode;
        private INode _endNode;

        public DirectedEdge(INode startNode, INode endNode, List<IWeight> weights) : base(weights)
        {
            _startNode = startNode;
            _endNode = endNode;
        }


        /// <summary>
        /// Gibt einen möglichen Knonten zurück. Dabei wird die Richtung der Kante beachtet
        /// </summary>
        /// <param name="node"></param>
        /// <returns>Potenzieller Knoten oder null</returns>
        public override INode GetPossibleEndpoint(INode node)
        {
            INode hPossibleEndpoint = null;
            if (node == _startNode)
            {
                hPossibleEndpoint = _endNode;
            }

            return hPossibleEndpoint;
        }
    }
}
