using System.Collections.Generic;
using System.IO;

namespace ExempluControale
{
    public static class Date
    {
        public static List<Tara> CitesteTari(string cale, List<string> erori)
        {
            List<Tara> tari = new List<Tara>();
            if (!File.Exists(cale))
            {
                erori.Add("Lipseste fisierul " + Path.GetFileName(cale) + " de langa aplicatie.");
                return tari;
            }

            string[] linii = File.ReadAllLines(cale);
            for (int i = 1; i < linii.Length; i++)
            {
                if (linii[i].Trim().Length == 0)
                {
                    continue;
                }

                string[] camp = linii[i].Split('#');
                if (camp.Length != 4)
                {
                    erori.Add("Randul " + (i + 1) + " este stricat si a fost sarit.");
                    continue;
                }

                tari.Add(new Tara
                {
                    Nume = camp[0],
                    Capitala = camp[1],
                    Continent = camp[2],
                    Steag = camp[3]
                });
            }
            return tari;
        }
    }
}
