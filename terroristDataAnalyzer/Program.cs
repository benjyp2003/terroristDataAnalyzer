using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
            while (true)
            {
                ShowMenu();
                string choice = Console.ReadLine();
                if (choice == "6")
                { break; }
                Console.WriteLine(HandleMenuChoice(choice, terrorist) + "\n");
            }

        }


        static void ShowMenu()
        {
            //Console.Clear();
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string[] menuItems = new[]
            {
    "1. Find the most common weapon",
    "2. Find the least common weapon",
    "3. Find the organization with the most members",
    "4. Find the organization with the least members",
    "5. Find the 2 terrorists who are closest to each other",
    "6. exit the program",
};

            int boxWidth = 80;
            string horizontalLine = new string('─', boxWidth - 2);

            Console.WriteLine("┌" + horizontalLine + "┐");
            Console.WriteLine("│" + CenterText("📊 Terrorist Data Analysis Menu 📊", boxWidth - 2) + "│");
            Console.WriteLine("├" + horizontalLine + "┤");

            foreach (var item in menuItems)
            {
                Console.WriteLine("│ " + item.PadRight(boxWidth - 3) + "│");
            }

            Console.WriteLine("└" + horizontalLine + "┘");
            Console.Write("Select an option (1-5): ");
        }

        static string CenterText(string text, int width)
        {
            int padding = (width - text.Length) / 2;
            return new string(' ', padding) + text + new string(' ', width - text.Length - padding);
        }



        static string HandleMenuChoice(string choice, List<Dictionary<string, object>> terrorist)
        {
            switch (choice)
            {
                case "1":
                    return FindMostCommonWeapon(terrorist);

                case "2":
                    return FindLeastCommonWeapon(terrorist);

                case "3":
                    return FindMostMembersOrg(terrorist);

                case "4":
                    return FindLeastMembersOrg(terrorist);

                case "5":
                    return FindClosestTerrorist(terrorist);


                default:
                    return "unvalid number, please try again.";

            }
            return "";
        }

        static string FindClosestTerrorist(List<Dictionary<string, object>> terrorists)
        {
            double minDistance = double.MaxValue;
            string terrorist1 = "";
            string terrorist2 = "";

            for (int i = 0; i < terrorists.Count; i++)
            {
                for (int j = i + 1; j < terrorists.Count; j++)
                {
                    var loc1 = (Dictionary<string, string>)terrorists[i]["lastLocation"];
                    var loc2 = (Dictionary<string, string>)terrorists[j]["lastLocation"];

                    double lat1 = double.Parse(loc1["lat"]);
                    double lon1 = double.Parse(loc1["lon"]);
                    double lat2 = double.Parse(loc2["lat"]);
                    double lon2 = double.Parse(loc2["lon"]);

                    double distance = Math.Sqrt(Math.Pow(lat1 - lat2, 2) + Math.Pow(lon1 - lon2, 2));

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        terrorist1 = (string)terrorists[i]["name"];
                        terrorist2 = (string)terrorists[j]["name"];
                    }
                }
            }

            return $"The closest terrorists are:\n{terrorist1}\n{terrorist2}\nDistance between them: {minDistance:F2}";
        }

        
        static string FindMostCommonWeapon(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> weaponDict = GetWeaponsDict(terrorist);
            return GetMaxDictVal(weaponDict);

        }


        static string FindLeastCommonWeapon(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> weaponDict = GetWeaponsDict(terrorist);
            return GetMinDictVal(weaponDict);
        }


        static Dictionary<string, int> GetWeaponsDict(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> weaponDict = new Dictionary<string, int>();

            foreach (var dict in terrorist)
            {
                if (dict.TryGetValue("weapons", out object weaponsObj) && weaponsObj is List<string> weapons)
                {
                    foreach (var weapon in weapons)
                    {
                        if (weaponDict.ContainsKey(weapon))
                        {
                            weaponDict[weapon] += 1;
                        }
                        else
                        {
                            weaponDict.Add(weapon, 1);
                        }
                    }

                }
                else
                {
                    Console.WriteLine("No weapons found or invalid format.");
                }
            }

            return weaponDict;
        }




        static string FindMostMembersOrg(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> orgDict = getOrgDict(terrorist);
            
            return GetMaxDictVal(orgDict);
        }

        static string FindLeastMembersOrg(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> orgDict = getOrgDict(terrorist);

            return GetMaxDictVal(orgDict);

        }

        static Dictionary<string, int> getOrgDict(List<Dictionary<string, object>> terrorist)
        {
            Dictionary<string, int> OrgMemberNumbersDict = new Dictionary<string, int>();

            foreach (var dict in terrorist)
            {
                if (dict.TryGetValue("affiliation", out object affiliationObj) && affiliationObj is string affiliation)
                {
                    
                        if (OrgMemberNumbersDict.ContainsKey((string)dict["affiliation"]))
                        {
                            OrgMemberNumbersDict[(string)dict["affiliation"]] += 1;
                        }
                        else
                        {
                            OrgMemberNumbersDict.Add((string)dict["affiliation"], 1);
                        }
                    
                }
                else
                {
                    Console.WriteLine("No affiliations found or invalid format.");
                }
            }

            return OrgMemberNumbersDict;
        }


        static string GetMaxDictVal(Dictionary<string, int> dict)
        {
            int max = dict.Max(x => x.Value);
            string maxKey = "";
            foreach (string weapon in dict.Keys)
            {
                if (dict[weapon] == max)
                {
                    maxKey = weapon;
                    break;
                }
            }
            return maxKey;
        }


        static string GetMinDictVal(Dictionary<string, int> dict)
        {
            int min = dict.Min(x => x.Value);
            string minKey = "";
            foreach (string weapon in dict.Keys)
            {
                if (dict[weapon] == min)
                {
                    minKey = weapon;
                    break;
                }
            }
            return minKey;
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

            //// Write JSON-style dict array to text file
            //var options = new JsonSerializerOptions { WriteIndented = true };
            //string jsonOutput = JsonSerializer.Serialize(terrorists, options);
            //// The file 'terrorists.txt' will be saved in the same folder where the program is running.
            //// If you're using Visual Studio, it's usually in: bin/Debug/netX.X/terrorists.txt
            //File.WriteAllText("terrorists.txt", jsonOutput);

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

