using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace terroristDataAnalyzer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Dictionary<string, object>> terrorist = JenerateTerrorists();
            //ShowMenu();
            //HandleMenuChoice(Console.ReadLine());
        }

        





        static List<Dictionary<string, object>> JenerateTerrorists()
        {
            var terrorists = new List<Dictionary<string, object>>();

            string[] firstNames = {
    "Ahmad", "Yousef", "Saeed", "Khaled", "Rami",
    "Maher", "Fadi", "Basem", "Nader", "Alaa",
    "Hussein", "Ali", "Wasim", "Mahmoud", "Ibrahim",
    "Tariq", "Jihad", "Samir", "Ammar", "Sharif"
};

            string[] lastNames = {
    "Khalil", "Mansour", "Al-Ali", "Zeidan", "Yousef",
    "Salim", "Darwish", "Radi", "Jaber", "Naim",
    "Barakat", "Omran", "Al-Zahar", "Hijazi", "Kamal",
    "Najm", "Shaaban", "Maqdad", "Abu-Salim", "Qasem"
};

            string[] weapons = { "Knife", "M16", "Handgun" };
            string[] affiliations = { "Hamas", "Islamic Jihad" };

            Random rand = new Random();

            for (int i = 0; i < 20; i++)
            {
                var person = new Dictionary<string, object>();

                person["name"] = firstNames[rand.Next(firstNames.Length)] + " " + lastNames[rand.Next(lastNames.Length)];
                person["weapons"] = GetRandomWeapons(rand, weapons);
                person["age"] = rand.Next(18, 51);
                var location = (
                    Math.Round(rand.NextDouble() * 180 - 90, 2),     // Latitude
                    Math.Round(rand.NextDouble() * 360 - 180, 2)     // Longitude
                );
                person["lastLocation"] = new Dictionary<string, string> {
                { "lat", location.Item1.ToString("F2") },
                { "lon", location.Item2.ToString("F2") }
};
                person["affiliation"] = affiliations[rand.Next(affiliations.Length)];

                terrorists.Add(person);
            }

            // Write JSON-style dict array to text file
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonOutput = JsonSerializer.Serialize(terrorists, options);
            // The file 'terrorists.txt' will be saved in the same folder where the program is running.
            // If you're using Visual Studio, it's usually in: bin/Debug/netX.X/terrorists.txt
            File.WriteAllText("terrorists.txt", jsonOutput);

            return terrorists;
        }

        static List<string> GetRandomWeapons(Random rand, string[] weapons)
        {
            var list = new List<string>();
            foreach (var weapon in weapons)
            {
                if (rand.NextDouble() < 0.5) // 50% chance to include each weapon
                    list.Add(weapon);
            }

            if (list.Count == 0)
                list.Add(weapons[rand.Next(weapons.Length)]); // ensure at least one weapon

            return list;
        }


    }
}

