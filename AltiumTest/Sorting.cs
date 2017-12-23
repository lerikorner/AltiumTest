using System;
using System.Collections.Generic;
using System.Linq;

namespace AltiumTest
{
    class Sorting
    {
        public static List<string> TRSortedtoStrings(List<string> stringBuf)
        {
            //преобразуем строки в объекты TextRecord
            List<TextRecord> textrecords = new List<TextRecord>();

            foreach (string stbuf in stringBuf)
            {
                Int32 code = Convert.ToInt32(stbuf.Substring(0, stbuf.IndexOf(".")));
                string description = stbuf.Substring(stbuf.IndexOf("."), stbuf.Length - stbuf.IndexOf("."));
                textrecords.Add(new TextRecord() { Code = code, Description = description });
            }

            //сортируем объекты по полям Code и Description
            IList<TextRecord> TRsorted = textrecords.OrderBy(x => x.Code).ThenBy(x => x.Description).ToList();

            //преобразуем список для копирования в файл
            List<string> TRtoString = new List<string>();
            foreach (TextRecord trs in TRsorted)
            {
                TRtoString.Add(trs.ToString());
            }
            return TRtoString;
        }
    }
}
