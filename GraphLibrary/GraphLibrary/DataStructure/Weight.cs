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
        void SetValue(double _Value);
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

        public void SetValue(double _Value)
        {
            throw new NotImplementedException();
        }
    }

    class CostWeighted : IWeight
    {
        private double FCostValue;

        public void SetValue(double _Value)
        {
            FCostValue = _Value;
        }

        public CostWeighted(double _CostValue)
        {
            FCostValue = _CostValue;
        }

        public CostWeighted()
        {
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
        public CapacityWeighted(double _CapacityValue)
        {
            FCapacityValue = _CapacityValue;
        }

        public CapacityWeighted()
        {
        }

        private double FCapacityValue;

        public void SetValue(double _Value)
        {
            FCapacityValue = _Value;
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
