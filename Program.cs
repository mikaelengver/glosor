using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading;

namespace Glosor
{
    class Program
    {
        static void Main(string[] args)
        {
            string[] glosRader;
            if (args.Length != 1)
            {
                Console.WriteLine("Ange namn på glosfil.");
                return;
            }
            var filename = args[0];
            if (!Path.IsPathFullyQualified(filename))
            {
                filename = Path.Combine(AppContext.BaseDirectory, filename);
            }
            if (!File.Exists(filename))
            {
                Console.WriteLine("Det går inte att hitta glosfil.");
                return;
            }
            try
            {
                glosRader = File.ReadAllLines(filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Problem att läsa in glosfilen. Kontrolla glosfilen.");
                Console.WriteLine(ex.Message);
                return;
            }
            RättaGlosor(glosRader);
        }

        private static void RättaGlosor(string[] glosRader)
        {
            var glosLista = new Collection<Glosa>();
            foreach (var glosRad in glosRader)
            {
                if (!glosRad.Contains("-"))
                {
                    Console.WriteLine("Fel på följande rad i glosfilen:");
                    Console.WriteLine(glosRad);
                    continue;
                }
                var glosArray = glosRad.Split('-');
                glosLista.Add(new Glosa { Språk1 = glosArray[0], Språk2 = glosArray[1] });
            }

            FörhörGlosor(glosLista);
        }

        private static void FörhörGlosor(Collection<Glosa> glosLista)
        {
            var random = new Random();
            while (NågotFel(glosLista))
            {
                var index = random.Next(0, glosLista.Count);
                if (glosLista[index].Poäng == 2) continue;

                Console.WriteLine($"Vad är översättningen för \"{glosLista[index].Språk2}\"?");
                var svar = Console.ReadLine();
                if (svar.Trim() == glosLista[index].Språk1)
                {
                    glosLista[index].Poäng++;
                    Console.WriteLine($"Rätt du har nu {VisaPoäng(glosLista)}.");
                }
                else
                {
                    glosLista[index].Poäng = 0;
                    Console.WriteLine($"Tyvärr fel, det rätta svaret är \"{glosLista[index].Språk1}\". Poäng: {VisaPoäng(glosLista)}");
                }
                Thread.Sleep(4000);
                Console.Clear();
            }
            Console.WriteLine($"Bra jobbat, du fick {glosLista.Sum(x => x.Poäng)} poäng.");
        }

        private static string VisaPoäng(Collection<Glosa> glosLista) => $"{glosLista.Sum(x => x.Poäng)}/{glosLista.Count * 2}";

        private static bool NågotFel(Collection<Glosa> glosLista)
        {
            return glosLista.Any(x => x.Poäng != 2);
        }
    }
}
