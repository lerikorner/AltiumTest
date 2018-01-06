using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.IO;

namespace AltiumTest
{
    public class FileManager
    {
        public static int StringRange = 1024;
        public static Int32 FileSize = 50000;
        public static Int32 SliceSize = 2900;
        public static ulong TotalRam = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;

        public static List<string> StringListRandomizer(int length)
        {
            int CodeRandom, DescriptionRandom;
            Random rndCode = new Random();
           

            //диапазоны значений
            int codeRange = int.MaxValue;
            int stringRange = StringRange;

            //повторители для Code и Description
            string copier = "";
            Int32 dubcode = 0;

            List<string> strBlocks = new List<string>();

            for (Int32 i = 0; i < length; i++)
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

        public static int FileSplit(string path, Int32 FileSize, Int32 ListSize)
        {
            var FiletoProcess = new StreamReader(path);
            List<string> strSlice = new List<string>();
            int counter = 0;
            string tempPath = "";

            if (ListSize < FileSize)
            {
                counter = FileSize / ListSize;
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

                if (FileSize % ListSize != 0)
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
            else if (ListSize >= FileSize)
            {
                return 1;
            }
            else return -1;
        }

        public static void MergeByQueues(string tempPath, string inPath, string outPath)
        {
            string[] paths = Directory.GetFiles(tempPath, "out_slice*.txt");
            int slices = paths.Length; // количество кусков
            int recordsize = StringRange + Int32.MaxValue.ToString().Length; // оценочная длина записи
            int records = FileSize; // ожидаемое количество записей в файле
            Int64 maxusage = Convert.ToInt64(TotalRam / 8); // максимальное использование памяти
            Int64 buffersize = maxusage / slices; // байт на каждый кусок
            double recordoverhead = 64; // The overhead of using Queue<> - как я понял, тут коэффициент превращения байт в строки, с небольшим запасом

            //int bufferlen = (int)(buffersize / (recordsize * recordoverhead)); //количество записей в очереди
            int bufferlen = records/slices;
            Console.WriteLine(bufferlen);
            Console.WriteLine(TotalRam);
            Console.WriteLine(recordsize);
            // Open the files
            StreamReader[] readers = new StreamReader[slices];
            for (int i = 0; i < slices; i++)
                readers[i] = new StreamReader(paths[i]);

            // Make the queues
            Queue<string>[] queues = new Queue<string>[slices];
            for (int i = 0; i < slices; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Load the queues
            for (int i = 0; i < slices; i++)
                LoadQueue(queues[i], readers[i], bufferlen);

            // Merge!
            StreamWriter sw = new StreamWriter(outPath);
            bool done = false;
            int lowest_index, j, progress = 0;
            string lowest_value;
            while (!done)
            {
                // Report the progress
                if (++progress % 5000 == 0)
                    Console.Write("{0:f2}%   \r",
                      100.0 * progress / records);

                // Find the slice with the lowest value
                
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < slices; j++)
                {
                    if (queues[j] != null)
                    {
                        if ((lowest_index < 0) ||
                            (String.CompareOrdinal(
                            queues[j].Peek().Substring(queues[j].Peek().IndexOf("."), queues[j].Peek().Length -
                            queues[j].Peek().IndexOf(".")),
                            lowest_value.Substring(lowest_value.IndexOf("."), lowest_value.Length -
                            lowest_value.IndexOf("."))) < 0)
                            &
                            (Convert.ToInt32(queues[j].Peek().Substring(0, queues[j].Peek().IndexOf("."))) <
                            Convert.ToInt32(lowest_value.Substring(0, lowest_value.IndexOf(".")))))
                        {
                            lowest_index = j;
                            lowest_value = queues[j].Peek();
                        }
                    }
                }
                             
                // Was nothing found in any queue? We must be done then.
                if (lowest_index == -1) { done = true; break; }

                // Output it
                sw.WriteLine(lowest_value);


                // Remove from queue
                queues[lowest_index].Dequeue();
                // Have we emptied the queue? Top it up
                if (queues[lowest_index].Count == 0)
                {
                    LoadQueue(queues[lowest_index],
                      readers[lowest_index], bufferlen);
                    // Was there nothing left to read?
                    if (queues[lowest_index].Count == 0)
                    {
                        queues[lowest_index] = null;
                    }
                }
            }
            sw.Close();

            // Close and delete the files
            for (int i = 0; i < slices; i++)
            {
                readers[i].Close();
                File.Delete(paths[i]);
            }
        }

        static void LoadQueue(Queue<string> queue,
            StreamReader file, int records)
        {
            for (int i = 0; i < records; i++)
            {
                if (file.Peek() < 0) break;
                queue.Enqueue(file.ReadLine());
            }
        }

        
    }
}
