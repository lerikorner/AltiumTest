using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;


namespace AltiumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            /*int CodeRandom, DescriptionRandom;
            string fileName = "c:\\temp\\out_small.txt";
            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            Random rndCode = new Random();
            Random rndDescription = new Random();
            string copier = "";
            for (int i = 0; i < 100; i++)
            {
                CodeRandom=rndCode.Next(0, int.MaxValue);
                DescriptionRandom = rndDescription.Next(0, 3);
                if (CodeRandom % (DescriptionRandom + 1) != 0)
                {
                    sw.WriteLine(CodeRandom.ToString() + "." + AltiumTest.KeyGenerator.GetUniqueKey(DescriptionRandom));
                    copier = AltiumTest.KeyGenerator.GetUniqueKey(DescriptionRandom);
                }
                else
                {
                    sw.WriteLine(CodeRandom.ToString() + "." + copier);
                }
            }
            sw.Close();*/

            string fileName = "c:\\temp\\out_small.txt";
            string[] stringBuf = File.ReadAllLines(fileName);
            List<TextRecord> textrecords = new List<TextRecord>();
            foreach (string stbuf in stringBuf)
            {
                Int32 code = Convert.ToInt32(stbuf.Substring(0, stbuf.IndexOf(".")));
              string description = stbuf.Substring(stbuf.IndexOf("."), stbuf.Length-stbuf.IndexOf("."));
                textrecords.Add(new TextRecord() { Code = code, Description=description });
            }
            IList<TextRecord> TRsorted = textrecords.OrderBy(x => x.Code).ThenBy(x => x.Description).ToList();

            fileName = "c:\\temp\\out_small_sorted.txt";
            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);

            foreach (TextRecord trs in TRsorted)
            {
                sw.WriteLine(trs.ToString());
            }
        }
    }
}
