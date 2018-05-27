using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    abstract class Edge
    {
        private IWeight FWeight;

        public abstract INode GetOtherEndpoint(INode _Node);

        public abstract INode[] GetPossibleEnpoints();

        public abstract List<string> EdgeHashInfo();

        protected Edge(IWeight _Weight)
        {
            FWeight = _Weight;
        }

        
        public bool IsWeighted()
        {
            return FWeight.HasWeightValue();
        }

        public double GetWeightValue()
        {
            return FWeight.WeightValue();
        }

        public IWeight GetWeight()
        {
            return FWeight;
        }

        

    }

    class UndirectedEdge : Edge
    {
        private INode FNodeA;
        private INode FNodeB;

        public UndirectedEdge(INode _NodeA, INode _NodeB, IWeight _Weight) : base(_Weight)
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

        public override INode[] GetPossibleEnpoints()
        {
            return new[] { FNodeA, FNodeB };
        }

        public override List<string> EdgeHashInfo()
        {
            var hResult = new List<string>();
            hResult.Add(FNodeA.Id + "-" + FNodeB.Id);
            hResult.Add(FNodeB.Id + "-" + FNodeA.Id);
            return hResult;
        }
    }

    class DirectedEdge : Edge
    {
        private INode FStartNode;
        private INode FEndNode;

        public DirectedEdge(INode _StartNode, INode _EndNode, IWeight _Weight) : base(_Weight)
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

        public INode GetEdgeSource()
        {
            return FStartNode;
        }

        public override INode[] GetPossibleEnpoints()
        {
            return new[] { FEndNode };
        }

        public override List<string> EdgeHashInfo()
        {
            var hResult = new List<string>();
            hResult.Add(FStartNode.Id + "-" + FEndNode.Id);
            return hResult;
        }
    }
}
