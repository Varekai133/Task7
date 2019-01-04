using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using OfficeOpenXml;

namespace Data
{
    public class XLSXLoader : ILoader
    {
        string _fileName;

        public XLSXLoader(string fileName)
        {
            _fileName = fileName;
        }

        public Queue<Candle> GetCandles()
        {
            Queue<Candle> r = new Queue<Candle>();

            using (ExcelPackage xlPackage = new ExcelPackage(new FileInfo(_fileName)))
            {
                var myWorksheet = xlPackage.Workbook.Worksheets.First();
                var totalRows = myWorksheet.Dimension.End.Row;
                var totalColumns = myWorksheet.Dimension.End.Column;

                for (int rowNum = 2; rowNum <= totalRows; rowNum++)
                {
                    var row = myWorksheet.Cells[rowNum, 1, rowNum, totalColumns]
                        .Select(c => c.Value == null ? string.Empty : c.Value.ToString()).ToArray();

                    string rawDateTime = row[2] + " " + row[3];

                    DateTime dateTime = DateTime.ParseExact(rawDateTime, "yyyyMMdd HHmmss", null);

                    Candle candle = new Candle()
                    {
                        High = int.Parse(row[5]),
                        Low = int.Parse(row[6]),
                        Open = int.Parse(row[4]),
                        Close = int.Parse(row[7]),
                        Time = dateTime,
                        TimeStamp = ((DateTimeOffset)dateTime).ToUnixTimeMilliseconds(),
                        Volume = int.Parse(row[8]),
                    };

                    r.Enqueue(candle);
                }
            }
            return r;
        }
    }
}
