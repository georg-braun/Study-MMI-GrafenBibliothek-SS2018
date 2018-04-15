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
        private double FWeightValue;

        public CostWeighted(double _WeightValue)
        {
            FWeightValue = _WeightValue;
        }

        public bool HasWeightValue()
        {
            return true;
        }

        public double WeightValue()
        {
            return FWeightValue;
        }
    }


}
