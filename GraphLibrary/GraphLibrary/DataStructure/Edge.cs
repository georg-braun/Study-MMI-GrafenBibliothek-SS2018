using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    abstract class Edge
    {
        private List<IWeight> FWeight;

        public abstract INode GetOtherEndpoint(INode _Node);

        public abstract INode[] GetPossibleEnpoints();

        public abstract List<string> EdgeHashInfo();

        protected Edge()
        {
            FWeight = new List<IWeight>();
        }

        protected Edge(IWeight _Weight) 
            : this()
        {
            FWeight.Add(_Weight);
        }

        public void AddWeight(IWeight _Weight)
        {
            FWeight.Add(_Weight);
        }

        /// <summary>
        /// Deprecated. Für Kompatibilitätszwecke noch vorhanden
        /// Prüft ob es ein Kostengewichtetes Kantengewicht gibt
        /// </summary>
        /// <returns>Existenz eines Kostengewichts</returns>
        public bool IsWeighted()
        {
            return IsWeighted<CostWeighted>();
        }

        public bool IsWeighted<T>() where T : IWeight
        {
            var hIsWeighted = false;
            foreach (var hWeight in FWeight)
            {
                if (hWeight is T)
                {
                    hIsWeighted = true;
                    break;
                }
            }
            return hIsWeighted;
        }


        /// <summary>
        /// Deprecated. Für Kompatibilitätszwecke noch vorhanden
        /// Gibt das Kantengewicht zurück wenn es sich um eine Kosten gewichtete Kante handelt.
        /// </summary>
        /// <returns>Wert eines Kostengewichts</returns>
        public double GetWeightValue()
        {
            return GetWeightValue<CostWeighted>();
        }

        public double GetWeightValue<T>() where T : IWeight
        {
            foreach (var hWeight in FWeight)
            {
                switch (hWeight)
                {
                    case CapacityWeighted hCapacityWeighted:
                        return hCapacityWeighted.WeightValue();
                    case CostWeighted hCostWeighted:
                        return hCostWeighted.WeightValue();
                    case Unweighted hUnweighted:
                        return hUnweighted.WeightValue();
                } 
            }

            throw new Exception("No valid Weight Class");
        }

        public IWeight GetWeight()
        {
            return GetWeight<CostWeighted>();
        }

        public IWeight GetWeight<T>() where T : IWeight
        {
            foreach (var hWeight in FWeight)
            {
                switch (hWeight)
                {
                    case CapacityWeighted hCapacityWeighted:
                        return hCapacityWeighted;
                    case CostWeighted hCostWeighted:
                        return hCostWeighted;
                    case Unweighted hUnweighted:
                        return hUnweighted;
                }
            }

            throw new Exception("No valid Weight Class");
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

        public DirectedEdge(INode _StartNode, INode _EndNode) : base()
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
