using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AltiumTest;

namespace AltiumTest
{
    class Program
    {
        static void Main(string[] args)
        {
            int CodeRandom, DescriptionRandom;
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
            sw.Close();
        }
    }
}
