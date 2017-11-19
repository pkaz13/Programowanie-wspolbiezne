using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Spotkania
{
    class Program
    {
        static Barrier barrier;

        static int amountA = 2;
        static int amountB = 1;

        static int resourceA;
        static int resourceB;

        static List<Thread> threadsA = new List<Thread>();
        static List<Thread> threadsB = new List<Thread>();
        static List<Thread> threadsAB = new List<Thread>();

        static bool oldStateA;
        static bool oldStateB;
        static bool oldStateAB;

        static bool currentStateA;
        static bool currentStateB;
        static bool currentStateAB;

        static int sleepingTime = 5000;

        static void Main(string[] args)
        {
            int numberOFThreads = 1;
            barrier = new Barrier(numberOFThreads * 3);

            oldStateA = false;
            oldStateB = false;
            oldStateAB = true;

            resourceA = amountA;
            resourceB = amountB;

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
                    barrier.SignalAndWait();
                    if (CanTakeResource(ResourceType.A))
                    {
                        Console.WriteLine("Pobieranie zasobu A - ilość zasobów : " + (resourceA));
                        resourceA--;
                        Thread.Sleep(sleepingTime);
                        resourceA++;
                        Console.WriteLine("Oddawanie zasobu A");
                    }
                    else
                    {
                        Console.WriteLine("A nie może pobrać zasobu");
                    }

                }
                else if (threadsB.Contains(Thread.CurrentThread))
                {
                    barrier.SignalAndWait();
                    if (CanTakeResource(ResourceType.B))
                    {
                        Console.WriteLine("Pobieranie zasobu B - ilość zasobów  : " + (resourceB));
                        resourceB--;
                        Thread.Sleep(sleepingTime);
                        resourceB++;
                        Console.WriteLine("Oddawanie zasobu B");
                    }
                    else
                    {
                        Console.WriteLine("B nie może pobrać zasobu");
                    }
                }
                else if (threadsAB.Contains(Thread.CurrentThread))
                {
                    barrier.SignalAndWait();
                    if (CanTakeResource(ResourceType.AB))
                    {
                        Console.WriteLine($"Pobieranie zasobu AB. Ilość zasobów A : {resourceA } , B : {resourceB}");
                        resourceA--;
                        resourceB--;
                        Thread.Sleep(sleepingTime);
                        resourceA++;
                        resourceB++;
                        Console.WriteLine("Oddawanie zasobu AB");
                    }
                    else
                    {
                        Console.WriteLine("AB nie może pobrać zasobu");
                    }
                }
                Clear();
            }
        }

        private static bool CanTakeResource(ResourceType type)
        {
            bool result = false;
            switch (type)
            {
                case ResourceType.A:
                    if (resourceA > 1 || (resourceA > 0 && oldStateA == false))
                    {
                        currentStateA = true;
                        result = true;
                    }
                    else
                    {
                        currentStateA = false;
                        result = false;
                    }
                    break;
                case ResourceType.B:
                    if (resourceB > 1 || (resourceB > 0 && oldStateB == false))
                    {
                        currentStateB = true;
                        result = true;
                    }
                    else
                    {
                        currentStateB = false;
                        result = false;
                    }
                    break;
                case ResourceType.AB:
                    if ((resourceA > 1 && resourceB > 1) || (resourceA > 1 && oldStateB == true) || (resourceB > 1 && oldStateA == true) || (resourceA > 0 && resourceB > 0 && oldStateAB == false))
                    {
                        currentStateAB = true;
                        result = true;
                    }
                    else
                    {
                        currentStateAB = false;
                        result = false;
                    }
                    break;
            }
            return result;
        }

        public enum ResourceType
        {
            A,
            B,
            AB
        }

        private static void Clear()
        {
            oldStateA = currentStateA;
            oldStateB = currentStateB;
            oldStateAB = currentStateAB;
        }
    }
}
