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
            var sWatch = System.Diagnostics.Stopwatch.StartNew();
            
            int CodeRandom, DescriptionRandom;
            string fileName = "c:\\temp\\out_small.txt";
            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            Random rndCode = new Random();
            Random rndDescription = new Random();
            string copier = "";
            Int32 dubcode=0;
            for (int i = 0; i < 10000; i++)
            {
                CodeRandom=rndCode.Next(0, int.MaxValue);
                DescriptionRandom = rndDescription.Next(0, 10);
                Random rnd3 = new Random();
                if (CodeRandom % (DescriptionRandom + 1) == 0)
                {
                    sw.WriteLine(CodeRandom.ToString() + "." + AltiumTest.KeyGenerator.GetUniqueKeySimply(DescriptionRandom, rnd3));
                    copier = AltiumTest.KeyGenerator.GetUniqueKeySimply(DescriptionRandom, rnd3);
                }
                else
                if (CodeRandom % (DescriptionRandom + 1) == 1)
                {
                    sw.WriteLine(CodeRandom.ToString() + "." + AltiumTest.KeyGenerator.GetUniqueKeySimply(DescriptionRandom, rnd3));
                    dubcode = CodeRandom;                
                }
                else
                {
                    sw.WriteLine(dubcode.ToString() + "." + copier);
                }
            }
            sw.Close();          

            //считываем все строки файла в массив
            string[] stringBuf = File.ReadAllLines(fileName);

            //преобразуем строки в объекты TextRecord
            List<TextRecord> textrecords = new List<TextRecord>();
            foreach (string stbuf in stringBuf)
            {
                Int32 code = Convert.ToInt32(stbuf.Substring(0, stbuf.IndexOf(".")));
                string description = stbuf.Substring(stbuf.IndexOf("."), stbuf.Length - stbuf.IndexOf("."));
                textrecords.Add(new TextRecord() { Code = code, Description=description });
            }

            //сортируем объекты по полям Code и Description
            IList<TextRecord> TRsorted = textrecords.OrderBy(x => x.Code).ThenBy(x => x.Description).ToList();


            //пишем сортированные данные в файл
            fileName = "c:\\temp\\out_small_sorted.txt";
            FileStream fileRandom = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter swRandom = new StreamWriter(fileRandom);
            fileRandom.Seek(0, SeekOrigin.End);

            foreach (TextRecord trs in TRsorted)
            {
                swRandom.WriteLine(trs.ToString());
            }
            swRandom.Close();

            sWatch.Stop();
            Console.WriteLine("затрачено времени:{0}", sWatch.Elapsed);
            Console.ReadKey();
        }
    }
}
