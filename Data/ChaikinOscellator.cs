using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ChaikinOscellator : IIndicator 
    {
        public ChaikinOscellator()
        {

        }

        public decimal Calculate(Candle candle)
        {
            return Calc(candle, 3) - Calc(candle, 10);
        }

        public decimal Calc(Candle candle, int id)
        {
            return ((2 / (id - 1)) * candle.Close + 1);
        }
    }
}
