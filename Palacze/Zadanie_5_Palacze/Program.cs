using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zadanie_5_Palacze
{
    class Program
    {
        private static int iluDostawcow = 3;  // jeden dostawca działa w jednym wątku
        private static int iluPalaczy = 3;  // jeden palacz działa w jednym wątku 

        private static Thread[] dostawcy;     // wątki dla dostawców
        private static Thread[] palacze;     // wątki dla palaczy

        private static List<Palacz> ListaPalaczy = new List<Palacz>(3);

        private static Semaphore pusty = new Semaphore(4, 4);   // pojemność magazynu
        private static Semaphore pelny = new Semaphore(0, 4);
        private static Semaphore dostep = new Semaphore(1, 1);

        private static List<TypZasobu> magazyn = new List<TypZasobu>();

        private static int czasPalenia = 5000;

        static void Main(string[] args)
        {
            dostawcy = new Thread[iluDostawcow];
            palacze = new Thread[iluPalaczy];

            dostawcy[0] = new Thread(new ThreadStart(Dostawca));
            dostawcy[0].Name = "BT";
            dostawcy[0].Start();

            dostawcy[1] = new Thread(new ThreadStart(Dostawca));
            dostawcy[1].Name = "BZ";
            dostawcy[1].Start();

            dostawcy[2] = new Thread(new ThreadStart(Dostawca));
            dostawcy[2].Name = "ZT";
            dostawcy[2].Start();

            ListaPalaczy.Add(new Palacz("Palacz 1", TypZasobu.Bibułka));
            ListaPalaczy.Add(new Palacz("Palacz 2", TypZasobu.Tytoń));
            ListaPalaczy.Add(new Palacz("Palacz 3", TypZasobu.Zapałka));

            for (int i = 0; i < iluPalaczy; i++)
            {
                palacze[i] = new Thread(new ThreadStart(Palacz));
                palacze[i].Name = i.ToString();
                palacze[i].Start();
            }

            Console.ReadKey();

        }

        private static void Dostawca()
        {
            try
            {
                while (true)
                {
                    pusty.WaitOne();
                    dostep.WaitOne();
                    if (magazyn.Count <= 8)
                    {
                        switch (Thread.CurrentThread.Name)
                        {
                            case "BT":
                                magazyn.Add(TypZasobu.Bibułka);
                                magazyn.Add(TypZasobu.Tytoń);
                                Console.Write("\nDostawca dostarczył bibułkę i tytoń");
                                break;
                            case "BZ":
                                magazyn.Add(TypZasobu.Bibułka);
                                magazyn.Add(TypZasobu.Zapałka);
                                Console.Write("\nDostawca dostarczył bibułkę i zapałki ");
                                break;
                            case "ZT":
                                magazyn.Add(TypZasobu.Zapałka);
                                magazyn.Add(TypZasobu.Tytoń);
                                Console.Write("\nDostawca dostarczył zapałkę i tytoń");
                                break;
                            default:
                                break;
                        }
                    }
                    
                    PokazStanMagazynu();
                    dostep.Release();
                    pelny.Release();

                    Thread.Sleep(czasPalenia);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Się coś zepsuło");
                Console.ReadKey();
            }
        }

        private static void PokazStanMagazynu()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write("\t\t Magazyn->" /* + new String(' ', magazyn.Count)*/ + magazyn.Count);
        }

        private static void Palacz()
        {
            try
            {
                while (true)
                {
                    pelny.WaitOne();
                    dostep.WaitOne();

                    int name = Convert.ToInt32(Thread.CurrentThread.Name);
                    if (ListaPalaczy[name].SzukajZasobow(magazyn))
                    {
                        ListaPalaczy[name].CzyZapale();
                    }
                    dostep.Release();
                    pusty.Release();

                    Thread.Sleep(czasPalenia);
                }
            }
            catch (Exception)
            {
                Console.WriteLine("Się coś zepsuło");
                Console.ReadKey();
            }
        }
    }

    public enum TypZasobu
    {
        Default = 0,
        Bibułka,
        Tytoń,
        Zapałka
    }

    class Palacz
    {
        public string Imie { get; set; }
        public List<TypZasobu> Zasoby { get; set; }

        public Palacz(string imie, TypZasobu zasob)
        {
            Imie = imie;
            Zasoby = new List<TypZasobu>();
            Zasoby.Add(zasob);
        }

        public bool SzukajZasobow(List<TypZasobu> magazyn)
        {
            bool maBibulke = false;
            bool maTyton = false;
            bool maZapalke = false;

            foreach (var item in Zasoby)
            {
                if (item == TypZasobu.Bibułka)
                    maBibulke = true;
                else if (item == TypZasobu.Tytoń)
                    maTyton = true;
                else if (item == TypZasobu.Zapałka)
                    maZapalke = true;
            }

            TypZasobu typ;

            if (maBibulke == false)
            {
                typ = magazyn.FirstOrDefault(x => x == TypZasobu.Bibułka);
                if (typ != TypZasobu.Default)
                {
                    Zasoby.Add(typ);
                    magazyn.Remove(typ);
                    Console.WriteLine(Imie + " pobrał bibułkę");
                    return true;
                }
            }
            if (maTyton == false)
            {
                typ = magazyn.FirstOrDefault(x => x == TypZasobu.Tytoń);
                if (typ != TypZasobu.Default)
                {
                    Zasoby.Add(typ);
                    magazyn.Remove(typ);
                    Console.WriteLine(Imie + " pobrał tytoń");
                    return true;
                }
            }
            if (maZapalke == false)
            {
                typ = magazyn.FirstOrDefault(x => x == TypZasobu.Zapałka);
                if (typ != TypZasobu.Default)
                {
                    Zasoby.Add(typ);
                    magazyn.Remove(typ);
                    Console.WriteLine(Imie + " pobrał zapałkę");
                    return true;
                }
            }
            return false;
        }

        public bool CzyZapale()
        {
            if (Zasoby.Contains(TypZasobu.Bibułka) && Zasoby.Contains(TypZasobu.Tytoń) && Zasoby.Contains(TypZasobu.Zapałka))
            {
                Console.WriteLine($"Ja, {Imie}, palę");
                Zasoby.Clear();
                return true;
            }
            else
            {
                Console.WriteLine($"Ja, {Imie}, nie palę bo nie mam odpowiedniej liczby zasobów");
                return false;
            }
        }
    }
}
