using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Data
{
    public class JSONLoader : ILoader
    {
        private string _fileName;

        public JSONLoader(string fileName)
        {
            _fileName = fileName;
        }
        public Queue<Candle> GetCandles()
        {
            Queue<Candle> candles = new Queue<Candle>();
            var text = File.ReadAllText(_fileName);
            dynamic jsonDe = JsonConvert.DeserializeObject(text);

            List<decimal> open = jsonDe.o.ToObject<List<decimal>>();
            List<decimal> high = jsonDe.h.ToObject<List<decimal>>();
            List<decimal> low = jsonDe.l.ToObject<List<decimal>>();
            List<decimal> close = jsonDe.c.ToObject<List<decimal>>();
            List<long> timestamp = jsonDe.t.ToObject<List<long>>();

            for (int i = 0; i < open.Count; i++)
            {
                Candle candle = new Candle()
                {
                    High = high[i],
                    Low = low[i],
                    Open = open[i],
                    Close = close[i],
                    Time = DateTimeOffset.FromUnixTimeSeconds(timestamp[i]).UtcDateTime,
                    TimeStamp = timestamp[i],
                    Volume = 999999
                };

                candles.Enqueue(candle);
            }

            return candles;
        }
    }
}
