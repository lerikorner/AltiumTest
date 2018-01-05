using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        public static int FileSizeinStrings(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).Length;
            }
            else return -1;
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

        class StateObject<T>
        {
            public T UserState { get; private set; }
            public object SyncRoot { get; private set; }
            public bool IsCancelled { get; set; }

            public StateObject(T state, object syncRoot)
            {
                UserState = state;
                SyncRoot = syncRoot;
            }
        }

        public static List<string> TammyGlobal = new List<string>();

        static void ThreadProc(object arg)
        {
            Console.WriteLine("Worker thread started.");

            var state = arg as StateObject<StreamReader>;
            var reader = state.UserState;
            var sync = state.SyncRoot;
           
            string line;

            // Считывание строки из файла.
            lock (sync)
                line = reader.ReadLine();
            
            Console.WriteLine("Processing line: {0}", line);

            TammyGlobal.Add(line);
            reader.BaseStream.Seek(0, SeekOrigin.Begin);
          
        }

        public static void MergeByQueues(string tempPath, string inPath, string outPath)
        {
            string[] paths = Directory.GetFiles(tempPath, "out_slice*.txt");
            int chunks = paths.Length; // количество кусков
            int recordsize = 1035; // estimated record size
            int records = 9000; // estimated total # records
            Int64 maxusage = 10000000; // max memory usage
            Int64 buffersize = maxusage / chunks; // bytes of each queue
            double recordoverhead = 7.5; // The overhead of using Queue<>
            int bufferlen = (int)(buffersize / recordsize /
              recordoverhead); // number of records in each queue

            // Open the files
            StreamReader[] readers = new StreamReader[chunks];
            for (int i = 0; i < chunks; i++)
                readers[i] = new StreamReader(paths[i]);

            // Make the queues
            Queue<string>[] queues = new Queue<string>[chunks];
            for (int i = 0; i < chunks; i++)
                queues[i] = new Queue<string>(bufferlen);

            // Load the queues
            for (int i = 0; i < chunks; i++)
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

                // Find the chunk with the lowest value
                
                lowest_index = -1;
                lowest_value = "";
                for (j = 0; j < chunks; j++)
                {
                    if (queues[j] != null)
                    {
                        if ((lowest_index < 0) ||
                            (String.CompareOrdinal(
                            queues[j].Peek().Substring(queues[j].Peek().ToString().IndexOf("."), queues[j].Peek().ToString().Length -
                            queues[j].Peek().ToString().IndexOf(".")),
                            lowest_value.Substring(lowest_value.IndexOf("."), lowest_value.Length -
                            lowest_value.IndexOf("."))) < 0)
                            &
                            (Convert.ToInt32(queues[j].Peek().Substring(0, queues[j].Peek().ToString().IndexOf("."))) <
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
            for (int i = 0; i < chunks; i++)
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

        public static void MergeSortedFile(string tempPath, int slicesCount, string inPath, string  outPath)
        {
            //List<string> Tammy = new List<string>();
            //List<string> Squirrel = new List<string>();
            string tempfileName = "";
            StreamReader[] sr = new StreamReader[slicesCount];
          
            //var threads=new Thread[slicesCount];
            //int[] threadEmpty=new int[slicesCount];

            StreamWriter swout = new StreamWriter(outPath, true);
            int flag;
            List<string> cutFirst = new List<string>();
            for (int k = 0; k < FileSizeinStrings(inPath); k++)
            {
                for (int i = 0; i < slicesCount; i++)
                {
                   
                    if (File.Exists(tempfileName = tempPath + i + ".txt"))
                    {
                        tempfileName = tempPath + i + ".txt";
                        Console.WriteLine("размер временного файла: {0}", FileSizeinStrings(tempfileName));

                        if (sr[i] == null)
                        {
                            //threadEmpty[i] = i;
                            sr[i] = new StreamReader(tempfileName);                                                                                  
                        }
                        //else if (sr[i] != null)
                        {                                                      
                            //if ((threads[i] == null))
                            //{
                            //   var state = new StateObject<StreamReader>(sr[i], new object());
                            //    threads[i] = new Thread(ThreadProc);
                            //    threads[i].Start(state);
                           // }

                            if (FileSizeinStrings(tempfileName) > 1)
                            {
                                //sr[threadEmpty[i]] = new StreamReader(tempfileName);
                                TammyGlobal.Add(sr[i].ReadLine());
                            }

                            else if (FileSizeinStrings(tempfileName) == 1)
                            {
                                TammyGlobal.Add(sr[i].ReadLine());
                                sr[i].Close();
                                File.Delete(tempfileName);

                            }
                            else if (FileSizeinStrings(tempfileName) == -1)
                            {
                                sr[i].Close();
                                File.Delete(tempfileName);
                            }
                        }
                                                                
                    }
                }

                flag = TammyGlobal.IndexOf(Sorting.TRSortedtoStrings(TammyGlobal).FirstOrDefault());
                           
                swout.WriteLine(Sorting.TRSortedtoStrings(TammyGlobal).FirstOrDefault());               
                Console.WriteLine("индекс минимальной записи: {0}", flag);               
                              
                //Console.WriteLine("количество кусков: {0}", slicesCount);
                tempfileName = tempPath + flag + ".txt";
                TammyGlobal.Clear();
                if (File.Exists(tempfileName))
                {
                    if (FileSizeinStrings(tempfileName) > 1)
                    {                       
                        //threads[flag].Join();
                        sr[flag].Close();
                        cutFirst = File.ReadAllLines(tempfileName).ToList<string>();
                        File.Delete(tempfileName);
                        cutFirst.RemoveAt(0);
                        File.WriteAllLines(tempfileName, cutFirst);
                        cutFirst.Clear();
                        sr[flag] = new StreamReader(tempfileName);
                    }

                    
                    else if (FileSizeinStrings(tempfileName) <= 1)
                    {
                        File.Delete(tempfileName);
                    }
                }
            }
            swout.Close();
        }    
    }
}
