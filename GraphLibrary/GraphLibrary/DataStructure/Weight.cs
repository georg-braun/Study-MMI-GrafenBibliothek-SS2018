using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphLibrary.DataStructure
{
    interface IWeight
    {
        bool HasWeightValue();
        double WeightValue();
    }

    class Unweighted : IWeight
    {
        public bool HasWeightValue()
        {
            return false;
        }

        public double WeightValue()
        {
            throw new NotImplementedException();
        }
    }

    class CostWeighted : IWeight
    {
        private double FCostValue;

        public CostWeighted(double _CostValue)
        {
            FCostValue = _CostValue;
        }

        public bool HasWeightValue()
        {
            return true;
        }

        public double WeightValue()
        {
            return FCostValue;
        }
    }

    class CapacityWeighted : IWeight
    {
        private double FCapacityValue;

        public CapacityWeighted(double _CapacityValue)
        {
            FCapacityValue = _CapacityValue;
        }

        public bool HasWeightValue()
        {
            return true;
        }

        public double WeightValue()
        {
            return FCapacityValue;
        }
    }


}
