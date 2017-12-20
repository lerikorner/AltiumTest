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
            string fileName = "c:\\temp\\out2.txt";
            FileStream aFile = new FileStream(fileName, FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(aFile);
            aFile.Seek(0, SeekOrigin.End);
            Random rnd = new Random();
            for (int i = 0; i < 1000; i++)
            {
                sw.WriteLine(AltiumTest.KeyGenerator.GetUniqueKey(rnd.Next(0,1024)));
            }
            sw.Close();
        }
    }
}
