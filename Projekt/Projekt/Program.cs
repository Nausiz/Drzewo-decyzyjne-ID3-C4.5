using System;
using System.Collections.Generic;
using System.Linq;

namespace Projekt
{
    class Program
    {
        public static int atrybuty;
        public static string[] rekordy;

        static void Main(string[] args)
        {
            string nazwaPliku = "breast-cancer.data"; //plik z danymi

            rekordy = System.IO.File.ReadAllLines(@"SCIEZKA" + nazwaPliku);
            atrybuty = Convert.ToInt32(rekordy[0].Split(",").Length);
            string[,] dane = PrzygotowanieDanych();

            //WyswietlTablice(dane);
            //WyswietlWartosci(dane);

            BudujDrzewo(dane);
        }

        public static string[,] PrzygotowanieDanych()
        {
            string[,] dane = new string[rekordy.Length, atrybuty];

            for (int i = 0; i < rekordy.Length; i++)
            {
                string[] daneRekord = rekordy[i].Split(",");
                for (int j = 0; j < atrybuty; j++)
                {
                    dane[i, j] = daneRekord[j];
                }
            }

            return dane;
        }

        public static void WyswietlTablice(string[,] dane)
        {
            for (int i = 0; i < dane.GetLength(0); i++)
            {
                for (int j = 0; j < dane.GetLength(1); j++)
                {
                    Console.Write(dane[i, j] + " ");
                }

                Console.WriteLine();
            }
        }

        public static void WyswietlWartosci(string[,] dane)
        {
            List<Dictionary<string, int>> listaWartosciAtrybutow = new List<Dictionary<string, int>>();

            for (int i = 0; i < dane.GetLength(1); i++)
            {
                Dictionary<string, int> wartosciAtrybutu = new Dictionary<string, int>();
                for (int j = 0; j < dane.GetLength(0); j++)
                {
                    if (wartosciAtrybutu.ContainsKey(dane[j, i]))
                    {
                        wartosciAtrybutu[dane[j,i]]++;
                    }
                    else
                    {
                        wartosciAtrybutu.Add(dane[j,i], 1);
                    }
                }
                listaWartosciAtrybutow.Add(wartosciAtrybutu);
            }

            for(int i = 0; i < listaWartosciAtrybutow.Count; i++)
            {
                var slownik = listaWartosciAtrybutow[i];
                if (i == listaWartosciAtrybutow.Count - 1)
                {
                    Console.WriteLine("Atrybut {0} (decyzyjny) ma {1} wartości: ", i + 1, slownik.Count);
                }
                else
                {
                    Console.WriteLine("Atrybut {0} ma {1} wartości: ", i + 1, slownik.Count);
                }
                
                foreach (var wartosc in slownik)
                {
                  Console.WriteLine("{0} - {1}", wartosc.Key, wartosc.Value);   
                }
            }
        }

        public static List<Dictionary<string, int>> Wystapienia(string[,] dane)
        {
            List<Dictionary<string, int>> listaWartosciAtrybutow = new List<Dictionary<string, int>>();

            for (int i = 0; i < dane.GetLength(1); i++)
            {
                Dictionary<string, int> wartosciAtrybutu = new Dictionary<string, int>();
                for (int j = 0; j < dane.GetLength(0); j++)
                {
                    if (wartosciAtrybutu.ContainsKey(dane[j, i]))
                    {
                        wartosciAtrybutu[dane[j, i]]++;
                    }
                    else
                    {
                        wartosciAtrybutu.Add(dane[j, i], 1);
                    }
                }
                listaWartosciAtrybutow.Add(wartosciAtrybutu);
            }

            return listaWartosciAtrybutow;
        }

        public static string[,] Podtablica(string[,] dane, int atrybut, string wartosc)
        {
            List<int> rekordy = new List<int>();
            for (int i = 0; i < dane.GetLength(0); i++)
            {
                for (int j = 0; j < dane.GetLength(1); j++)
                {
                    if (j == atrybut && dane[i, j] == wartosc)
                    {
                        rekordy.Add(i);
                    }
                }
            }

            string[,] podtablica = new string[rekordy.Count, atrybuty];


            for (int i = 0; i < podtablica.GetLength(0); i++)
            {
                for (int j = 0; j < podtablica.GetLength(1); j++)
                {
                    podtablica[i, j] = dane[rekordy[i], j];
                }
            }

            return podtablica;
        }

        public static float Entropia(float[] prawdopodobienstwa)
        {
            float wynik = 0;

            for (int i = 0; i < prawdopodobienstwa.Length; i++)
            {   
                if(prawdopodobienstwa[i]>0)
                    wynik += (prawdopodobienstwa[i] * (float)Math.Log2(prawdopodobienstwa[i]));
            }

            if (wynik != 0)
                return -wynik;
            else
                return wynik;
        }

        public static float Info(string[,] dane)
        {
            Dictionary<string, int> wartosciAtrybutu = new Dictionary<string, int>();

            for (int i = 0; i < dane.GetLength(0); i++)
            {
                if (wartosciAtrybutu.ContainsKey(dane[i, dane.GetLength(1)-1]))
                {
                        wartosciAtrybutu[dane[i, dane.GetLength(1) - 1]]++;
                }
                else
                {
                    wartosciAtrybutu.Add(dane[i, dane.GetLength(1) - 1], 1);
                }
            }

            float[] prawdopodobienstwa = new float[wartosciAtrybutu.Count];

            for (int i = 0; i < wartosciAtrybutu.Count; i++)
            {
                var slownik = wartosciAtrybutu.ElementAt(i);
                prawdopodobienstwa[i] = (float)slownik.Value / dane.GetLength(0);
            }

            return Entropia(prawdopodobienstwa);
        }

        public static float Info(int atrybut, string[,] dane)
        {
            Dictionary<string, int> wartosciAtrybutu = new Dictionary<string, int>();

            for (int i = 0; i < dane.GetLength(0); i++)
            {
                if (wartosciAtrybutu.ContainsKey(dane[i, atrybut]))
                {
                    wartosciAtrybutu[dane[i, atrybut]]++;
                }
                else
                {
                    wartosciAtrybutu.Add(dane[i, atrybut], 1);
                }
            }

            Dictionary<string, int> wartosciAtrybutuDecyzyjnego = new Dictionary<string, int>();

            for (int i = 0; i < dane.GetLength(0); i++)
            {
                if (wartosciAtrybutuDecyzyjnego.ContainsKey(dane[i, dane.GetLength(1) - 1]))
                {
                    wartosciAtrybutuDecyzyjnego[dane[i, dane.GetLength(1) - 1]]++;
                }
                else
                {
                    wartosciAtrybutuDecyzyjnego.Add(dane[i, dane.GetLength(1) - 1], 1);
                }
            }

            float wynik = 0;

            for (int i = 0; i < wartosciAtrybutu.Count; i++)
            {
                string[,] tablicaWartosci = Podtablica(dane, atrybut, wartosciAtrybutu.ElementAt(i).Key);
                float[] prawdopodobienstwa = new float[wartosciAtrybutuDecyzyjnego.Count];

                for (int j = 0; j < wartosciAtrybutuDecyzyjnego.Count; j++)
                {
                    int ileRazyDecyzyjny = 0;
                    for (int k = 0; k < tablicaWartosci.GetLength(0); k++)
                    {
                        if (tablicaWartosci[k, tablicaWartosci.GetLength(1) - 1] == wartosciAtrybutuDecyzyjnego.ElementAt(j).Key)
                        {
                            ileRazyDecyzyjny++;
                        }
                    }
                    prawdopodobienstwa[j] = ((float)ileRazyDecyzyjny / wartosciAtrybutu.ElementAt(i).Value);
                }

                wynik += (float)wartosciAtrybutu.ElementAt(i).Value/dane.GetLength(0) * Entropia(prawdopodobienstwa);
            }

            return wynik;
        }

        public static float Gain(int atrybut, string[,] dane)
        {
            return Info(dane) - Info(atrybut, dane);
        }

        public static float SplitInfo(int atrybut, string[,] dane)
        {
            Dictionary<string, int> wartosciAtrybutu = new Dictionary<string, int>();

            for (int i = 0; i < dane.GetLength(0); i++)
            {
                if (wartosciAtrybutu.ContainsKey(dane[i, atrybut]))
                {
                    wartosciAtrybutu[dane[i, atrybut]]++;
                }
                else
                {
                    wartosciAtrybutu.Add(dane[i, atrybut], 1);
                }
            }

            float[] prawdopodobienstwa = new float[wartosciAtrybutu.Count];

            for (int i = 0; i < wartosciAtrybutu.Count; i++)
            {
                prawdopodobienstwa[i] = (float)wartosciAtrybutu.ElementAt(i).Value / dane.GetLength(0);
            }

            return Entropia(prawdopodobienstwa);
        }

        public static float GainRatio(int atrybut, string[,] dane)
        {
            if (SplitInfo(atrybut, dane) == 0)
                return 0.0f;
            else
                return Gain(atrybut,dane)/SplitInfo(atrybut,dane);
        }

        public static void BudujDrzewo(string[,] dane, string wciecie= "", int poprzedni= -1)
        {
            int kolumny = dane.GetLength(1);

            List<Dictionary<string, int>> wystapieniaWartosci = Wystapienia(dane);

            float[] gainRatioTab = new float[kolumny-1];
            for (int i = 0; i < gainRatioTab.Length; i++)
            {
                gainRatioTab[i] = GainRatio(i, dane);
            }

            int indexMaxWartosc = 0;
            float maxWartosc = GainRatio(indexMaxWartosc, dane);
            for (int i = 0; i < kolumny-1; i++)
            {
                if (GainRatio(i, dane) > maxWartosc)
                {
                    maxWartosc = GainRatio(i, dane);
                    indexMaxWartosc = i;
                }
            }

            if (0.0 < maxWartosc)
            {
                if (poprzedni != -1)
                    Console.WriteLine("{0}{1} -> Atrybut: {2}", wciecie, wystapieniaWartosci[poprzedni].Keys.ElementAt(0), indexMaxWartosc + 1);
                
                else
                    Console.WriteLine("{0}Atrybut: {1}", wciecie, indexMaxWartosc + 1);

                poprzedni = indexMaxWartosc;
                wciecie += "    ";
                
                for(int i = 0; i<wystapieniaWartosci[indexMaxWartosc].Count; i++)
                {
                    string[,] wezel = Podtablica(dane, indexMaxWartosc, wystapieniaWartosci[indexMaxWartosc].ElementAt(i).Key);
                    BudujDrzewo(wezel, wciecie, poprzedni);
                }
            }
            else
            {
                Console.WriteLine("{0}{1} -> D: {2}", wciecie, wystapieniaWartosci[poprzedni].Keys.ElementAt(0), wystapieniaWartosci[kolumny - 1].Keys.ElementAt(0));
            }
        }
    }
}