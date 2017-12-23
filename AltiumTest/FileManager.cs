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
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).Length;
            }
            else return -1;
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
                for (int i = 0; i < counter; i++)
                {
                    tempPath = "c:\\temp\\splits\\out_slice" + i + ".txt";
                    for (int j = 0; j < ListSize; j++)
                    {
                        strSlice.Add(FiletoProcess.ReadLine());
                    }
                    strSlice = Sorting.TRSortedtoStrings(strSlice);
                    FileFromList(tempPath, strSlice, true);
                    strSlice.Clear();
                }

                if (FileSizeinStrings(path) % ListSize != 0)
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
            else if (ListSize >= FileSizeinStrings(path))
            {
               
                return 1;
            }
            else return -1;
        }

        public static void MergeSortedFile(string tempPath, int slicesCount, string inPath, string  outPath)
        {
            List<string> Tammy = new List<string>();
            List<string> Squirrel = new List<string>();
            string tempfileName = "";

            for (int k = 0; k < FileSizeinStrings(inPath); k++)
            {
                for (int i = 0; i < slicesCount; i++)
                {
                    tempfileName = tempPath + i + ".txt";
                    if (File.Exists(tempfileName))
                    {
                        StreamReader sr = new StreamReader(tempfileName);
                        Console.WriteLine("размер временного файла: {0}", FileSizeinStrings(tempfileName));
                        if (FileSizeinStrings(tempfileName) > 1)
                        {
                            Tammy.Add(sr.ReadLine());
                            sr.Close();
                        }
                        else if (FileSizeinStrings(tempfileName) == 1)
                        {
                            Tammy.Add(sr.ReadLine());
                            sr.Close();
                            File.Delete(tempfileName);
                           
                        }
                        else if (FileSizeinStrings(tempfileName) != -1)
                        {
                            sr.Close();
                            File.Delete(tempfileName);                            
                        }                                               
                    }
                }

                int flag = Tammy.IndexOf(Sorting.TRSortedtoStrings(Tammy).FirstOrDefault());
                StreamWriter swout = new StreamWriter(outPath, true);             
                swout.WriteLine(Sorting.TRSortedtoStrings(Tammy).FirstOrDefault());               
                Console.WriteLine("индекс минимальной записи: {0}", flag);               
                swout.Close();
                

                //Console.WriteLine("количество кусков: {0}", slicesCount);
                tempfileName = tempPath + flag + ".txt";
                Tammy.Clear();
                if (File.Exists(tempfileName))
                {
                    if (FileSizeinStrings(tempfileName) > 1)
                    {
                        List<string> cutFirst = File.ReadAllLines(tempfileName).ToList<string>();
                        File.Delete(tempfileName);
                        cutFirst.RemoveAt(0);
                        File.WriteAllLines(tempfileName, cutFirst);
                    }

                    else if (FileSizeinStrings(tempfileName) == 1)
                    {
                       // List<string> cutFirst = File.ReadAllLines(tempfileName).ToList<string>();
                        File.Delete(tempfileName);
                        // File.WriteAllLines(tempfileName, cutFirst);                       
                    }
                    else if (FileSizeinStrings(tempfileName) <= 0)
                    {
                        File.Delete(tempfileName);
                    }
                }


            }          
        }    
    }
}
