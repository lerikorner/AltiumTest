using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;

namespace AltiumTest
{
    public class FileManager
    {
        public static List<string> StringListRandomizer(int length)
        {
            int CodeRandom, DescriptionRandom;
            Random rndCode = new Random();
           

            //диапазоны значений
            int codeRange = int.MaxValue;
            int stringRange = 1024;

            //повторители для Code и Description
            string copier = "";
            Int32 dubcode = 0;

            List<string> strBlocks = new List<string>();

            for (int i = 0; i < length; i++)
            {               
                CodeRandom = rndCode.Next(0, codeRange);
                DescriptionRandom = rndCode.Next(0, stringRange);
                string desc = KeyGenerator.GetUniqueKeySimply(DescriptionRandom);

                if (CodeRandom % (DescriptionRandom + 1) != 0)
                {
                    copier = desc;
                }
                else
                {
                    desc = KeyGenerator.GetUniqueKeySimply(DescriptionRandom);
                }

                dubcode = CodeRandom;
                strBlocks.Add(dubcode.ToString() + "." + copier);
            }
            return strBlocks;
        }

        public static void FileFromList(string path, List<string>data, bool create)
        {
            if (create)
            {
                if (File.Exists(path)) File.Delete(path);
                //File.Create(path);
            }
            File.WriteAllLines(path, data);           
        }

        public static int FileSizeinStrings(string path)
        {
            int counter = 0;
            string line;
            StreamReader file = new StreamReader(path);
            while ((line = file.ReadLine()) != null)
            {
                if (!string.IsNullOrEmpty(line))
                {
                    counter++;
                }
            }
            file.Close();
            return (counter);
        }

        public static int FileSplit(string path, Int32 ListSize)
        {
            var FiletoProcess = new StreamReader(path);
            List<string> strSlice = new List<string>();
            int counter = 0;
            string tempPath = "";

            if (ListSize < FileSizeinStrings(path))
            {
                counter = FileSizeinStrings(path) / ListSize;
                for (int i = 0; i<counter; i++)
                {
                    tempPath = "c:\\temp\\splits\\out_slice" + i + ".txt";
                    for  (int j = 0; j < ListSize; j++)
                    {
                        strSlice.Add(FiletoProcess.ReadLine());
                    }
                    strSlice = Sorting.TRSortedtoStrings(strSlice);
                    FileFromList(tempPath, strSlice, true);
                    strSlice.Clear();
                }

                if (FileSizeinStrings(path) % ListSize != 0)
                {
                    counter += 1;
                    tempPath = "c:\\temp\\splits\\out_slice" + (counter-1) + ".txt";
                    while (FiletoProcess.Peek()!=-1)
                    {
                        strSlice.Add(FiletoProcess.ReadLine());
                    }
                    strSlice = Sorting.TRSortedtoStrings(strSlice);
                    FileFromList(tempPath, strSlice, false);
                    strSlice.Clear();
                }
            }
            else
            {            
                tempPath = "c:\\temp\\splits\\out_slice" + counter + ".txt";
                while (FiletoProcess.Peek() != -1)
                {
                    strSlice.Add(FiletoProcess.ReadLine());
                }
                strSlice = Sorting.TRSortedtoStrings(strSlice);
                FileFromList(tempPath, strSlice, false);
                strSlice.Clear();
            }
            return counter + 1;
        }

        public static void MergeSortedFile(string tempPath, int slicesCount, string inPath, string  outPath)
        {
            List<string> Tammy = new List<string>();
            List<string> Squirrel = new List<string>();

            for (int k = 0; k < FileSizeinStrings(inPath); k++)
            {
                for (int i = 0; i < slicesCount - 1; i++)
                {
                    string tempfileName = tempPath + i + ".txt";

                    if (File.Exists(tempfileName))
                    {
                        StreamReader sr = new StreamReader(tempfileName);

                        if (FileSizeinStrings(tempfileName) >= 1)
                        {
                            Tammy.Add(sr.ReadLine());
                        }
                        else if (FileSizeinStrings(tempfileName) == 1)
                        {
                            Tammy.Add(sr.ReadLine());
                            File.Delete(tempfileName);                         
                        }                    
                        sr.Close();
                    }
                }

                StreamWriter swout = new StreamWriter(outPath, true);             
                swout.WriteLine(Sorting.TRSortedtoStrings(Tammy).FirstOrDefault());               
                Console.WriteLine("индекс минимальной записи: {0}", Tammy.IndexOf(Sorting.TRSortedtoStrings(Tammy).FirstOrDefault()));               
                swout.Close();

                for (int i = 0; i < slicesCount - 1; i++)
                {
                    string tempfileName = tempPath + i + ".txt";

                    if (File.Exists(tempfileName))
                    {
                        if (i != Tammy.IndexOf(Sorting.TRSortedtoStrings(Tammy).FirstOrDefault()))
                        {
                            Squirrel.Add(Tammy[i]);
                            foreach (string tline in File.ReadAllLines(tempfileName))
                            {
                                Squirrel.Add(tline);
                            }
                            File.Delete(tempfileName);
                            File.WriteAllLines(tempfileName, Squirrel);
                        }

                        else
                        {
                            List<string> cutFirst = File.ReadAllLines(tempfileName).ToList<string>();
                            File.Delete(tempfileName);
                            cutFirst.RemoveAt(0);
                            File.WriteAllLines(tempfileName, cutFirst);
                        }
                    }
                    Tammy.Clear();
                    Squirrel.Clear();
                }
            }          
        }    
    }
}
