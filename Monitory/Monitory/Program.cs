using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Monitory
{
    class Program
    {
        static int resourceA = 1;
        static int resourceB = 1;

        static bool reservationA = false;
        static bool reservationB = false;

        static List<Thread> threadsA = new List<Thread>();
        static List<Thread> threadsB = new List<Thread>();
        static List<Thread> threadsAB = new List<Thread>();

        static object blockadeA = new object();
        static object blockadeB = new object();
        static object blockadeAB = new object();

        static void Main(string[] args)
        {
            int numberOFThreads = 3;

            for (int i = 0; i < numberOFThreads; i++)
            {
                Thread A = new Thread(Request);
                Thread B = new Thread(Request);
                Thread AB = new Thread(Request);
                threadsA.Add(A);
                threadsB.Add(B);
                threadsAB.Add(AB);
                A.Name = "thread A" + (i + 1);
                B.Name = "thread B" + (i + 1);
                AB.Name = "thread AB" + (i + 1);
                A.Start();
                B.Start();
                AB.Start();
            }

            Console.ReadKey();
        }

        static void Request()
        {
            while (true)
            {
                if (threadsA.Contains(Thread.CurrentThread))
                {
                    if (Monitor.TryEnter(blockadeA))
                    {
                        try
                        {
                            if (resourceA > 0 && reservationA == false)
                            {
                                Console.WriteLine("Pobieranie zasobu A - ilość zasobów : " + (resourceA));
                                resourceA--;
                                Thread.Sleep(2000);
                                resourceA++;
                                Console.WriteLine("Oddawanie zasobu A");
                            }
                        }
                        finally
                        {
                            Monitor.Exit(blockadeA);
                        }
                    }
                }
                else if (threadsB.Contains(Thread.CurrentThread))
                {
                    if (Monitor.TryEnter(blockadeB))
                    {
                        try
                        {
                            if (resourceB > 0 && reservationB == false)
                            {
                                Console.WriteLine("Pobieranie zasobu B - ilość zasobów  : " + (resourceB));
                                resourceB--;
                                Thread.Sleep(2000);
                                resourceB++;
                                Console.WriteLine("Oddawanie zasobu B");
                            }
                        }
                        finally
                        {
                            Monitor.Exit(blockadeB);
                        }
                    }
                }
                else if (threadsAB.Contains(Thread.CurrentThread))
                {
                    if ((resourceA * resourceB) == 0)
                    {
                        if (Monitor.IsEntered(blockadeAB) == false)
                        {
                            reservationA = resourceA > 0 ? true : false;
                            reservationB = resourceB > 0 ? true : false;
                        }
                    }
                    if (Monitor.TryEnter(blockadeAB))
                    {
                        try
                        {
                            if (resourceA > 0 && resourceB > 0)
                            {
                                Console.WriteLine($"Pobieranie zasobu AB. Ilość zasobów A : {resourceA } , B : {resourceB}");
                                resourceA--;
                                resourceB--;
                                Thread.Sleep(2000);
                                resourceA++;
                                resourceB++;
                                reservationA = false;
                                reservationB = false;
                                Console.WriteLine("Oddawanie zasobu AB");
                            }
                        }
                        finally
                        {
                            Monitor.Exit(blockadeAB);
                        }

                    }
                }
            }
        }
    }
}
