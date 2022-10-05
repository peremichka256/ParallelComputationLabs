using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PC_Task1
{
    internal class ColorValues
    {
        private int _aValue;

        private int _rValue;

        private int _gValue;

        private int _bValue;

        public int AValue
        {
            get => _aValue;
            set => _aValue = value;
        }

        public int RValue
        {
            get => _rValue;
            set => _rValue = value;
        }

        public int GValue
        {
            get => _gValue;
            set => _gValue = value;
        }

        public int BValue
        {
            get => _bValue;
            set => _bValue = value;
        }

        public ColorValues(int aValue, int rValue, int gValue, int bValue)
        {
            AValue = aValue;
            RValue = rValue; 
            GValue = gValue; 
            BValue = bValue;
        }
    }
}
