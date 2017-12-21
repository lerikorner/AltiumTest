using System;
using System.IO;

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
            TextRecord[] trec = new TextRecord[File.ReadAllLines(fileName).Length];
            
           // FileStream aFile = new FileStream(fileName, FileMode.Open);
            StreamReader sr = new StreamReader(fileName);
            //aFile.Seek(0, SeekOrigin.Begin);
            string rec = "";          
            int i = 0;
            while (rec != null)
            {
                rec = sr.ReadLine();
                trec[i].Code = Convert.ToInt32(rec.Substring(0, rec.IndexOf(".")-1));
                i++;
            }
            sr.Close();
            Console.WriteLine(trec[0].Code);
        }
    }
}
