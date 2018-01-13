using System;
using System.Collections.Generic;
using System.Linq;

namespace BigFileSorting
{
    // MARK: - sorting methods
    class SortingMethods
    {
        // MARK: - quick sort by 2 parameters using LINQ
        public static List<string> TextRecordSortedInStrings(List<string> stringBuf) 
        {
            // MARK: - transforming strings to TextRecord objects
            List<TextRecord> textrecords = new List<TextRecord>();

            foreach (string stbuf in stringBuf)
            {
                if (stbuf != null)
                {
                    UInt32 codeid = Convert.ToUInt32(stbuf.Substring(0, 
                        stbuf.IndexOf(".")));
                    string description = stbuf.Substring(stbuf.IndexOf("."), 
                        stbuf.Length - stbuf.IndexOf("."));
                    textrecords.Add(new TextRecord()
                    {
                        CodeID = codeid,
                        Description = description
                    });
                }                
            }

            // MARK: - sorting by CodeID and Description values
            IList<TextRecord> TRsorted = 
                textrecords.OrderBy(x => x.CodeID).ThenBy(x => x.Description).ToList();

            // MARK: - transforming TextRecord to string
            List<string> TRtoString = new List<string>();
            foreach (TextRecord trs in TRsorted)
            {
                TRtoString.Add(trs.ToString());
            }
            return TRtoString;
        }

        // MARK: - insertion sort
        public static List<string> TRSortedtoStringsByInserts(List<string> stringBuf) 
        {           
            int i, j;           
            string tmpString;
            for (i = 1; i < stringBuf.Count; i++)
            {
                j = i;
                while ((j > 0) && 
                    (Convert.ToUInt32(stringBuf[j].Substring(0, 
                    stringBuf[j].IndexOf("."))) 
                    <
                    Convert.ToUInt32(stringBuf[j - 1].Substring(0, 
                    stringBuf[j - 1].IndexOf(".")))))
                {
                    tmpString = stringBuf[j];
                    stringBuf[j] = stringBuf[j - 1];
                    stringBuf[j - 1] = tmpString;
                    j--;
                }
            }
            return stringBuf;
        }
    }
}
