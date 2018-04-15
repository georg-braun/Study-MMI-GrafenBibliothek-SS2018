using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    abstract class Edge
    {
        private List<IWeight> FWeights;

        public abstract INode GetOtherEndpoint(INode _Node);

        public abstract List<INode> GetPossibleEnpoints();

        protected Edge(List<IWeight> _Weights)
        {
            this.FWeights = new List<IWeight>();
            FWeights = _Weights;
        }

        

        /// <summary>
        /// Gibt an ob die Kante ein Gewicht hat
        /// </summary>
        /// <returns></returns>
        public bool IsWeighted()
        {
            var hIsWeighted = false;
            foreach (var hCurrentWeight in FWeights)
            {
                hIsWeighted = hCurrentWeight.HasWeightValue();
            }

            return hIsWeighted;
        }

        public List<IWeight> GetWeights()
        {
            return FWeights;
        }

        

    }

    class UndirectedEdge : Edge
    {
        private INode FNodeA;
        private INode FNodeB;

        public UndirectedEdge(INode _NodeA, INode _NodeB, List<IWeight> _Weights) : base(_Weights)
        {
            FNodeA = _NodeA;
            FNodeB = _NodeB;
        }

        /// <summary>
        /// Gibt einen erreichbaren Knoten zurück. Richtung ist egal
        /// </summary>
        /// <param name="_Node"></param>
        /// <returns>Der verbundene Knoten wenn der übergebene Knoten auch in der Kante steckt</returns>
        public override INode GetOtherEndpoint(INode _Node)
        {
            INode hPossibleEndpoint = null;
            if (_Node == FNodeA)
            {
                hPossibleEndpoint = FNodeB;
            }
            else if (_Node == FNodeB)
            {
                hPossibleEndpoint = FNodeA;
            }

            return hPossibleEndpoint;
        }

        public override List<INode> GetPossibleEnpoints()
        {
            var hPossibleEndpoints = new List<INode> { FNodeA, FNodeB };
            return hPossibleEndpoints;
        }
    }

    class DirectedEdge : Edge
    {
        private INode FStartNode;
        private INode FEndNode;

        public DirectedEdge(INode _StartNode, INode _EndNode, List<IWeight> _Weights) : base(_Weights)
        {
            FStartNode = _StartNode;
            FEndNode = _EndNode;
        }


        /// <summary>
        /// Gibt einen möglichen Knonten zurück. Dabei wird die Richtung der Kante beachtet
        /// </summary>
        /// <param name="_Node"></param>
        /// <returns>Potenzieller Knoten oder null</returns>
        public override INode GetOtherEndpoint(INode _Node)
        {
            INode hPossibleEndpoint = null;
            if (_Node == FStartNode)
            {
                hPossibleEndpoint = FEndNode;
            }

            return hPossibleEndpoint;
        }

        public override List<INode> GetPossibleEnpoints()
        {
            var hPossibleEndpoints = new List<INode> { FEndNode };
            return hPossibleEndpoints;
        }
    }
}
