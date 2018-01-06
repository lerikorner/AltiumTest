using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace AltiumTest
{
    class MethodsBIN
    {
        public static int FileSizeinStrings(string path)
        {
            if (File.Exists(path))
            {
                return File.ReadAllLines(path).Length;
            }
            else return -1;
        }

        public static void MergeSortedFile(string tempPath, int slicesCount, string inPath, string outPath)
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
    }
}
