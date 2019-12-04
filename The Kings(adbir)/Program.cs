using System;
using System.Collections.Generic;
using System.Net;
using Newtonsoft.Json;
using System.Data;
using System.Linq;

namespace The_Kings_adbir_
{

    public class King
    {

        public King(int id, string nm, string cty, string hse, string yrs, int dur)
        {
            this.Id = id;
            this.Name = nm;
            this.Country = cty;
            this.House = hse;
            this.Years = yrs;
            this.Duration = dur;
        }

        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("nm")]
        public string Name { get; set; }
        [JsonProperty("cty")]
        public string Country { get; set; }
        [JsonProperty("hse")]
        public string House { get; set; }
        [JsonProperty("yrs")]
        public string Years { get; set; }
        [JsonProperty("dur")]
        public long Duration { get; set; }
    }
    
    class Program
    {
        private static readonly string dataSource = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
        private static readonly string jsonString = GetData(dataSource);
        private static readonly List<King> KingsList = JsonConvert.DeserializeObject<List<King>>(jsonString);

        static void ToString(King king)
        {
            Console.WriteLine($"Name: {king.Name}\tCountry:{king.Country}\tHouse:{king.House}\tReign:{king.Years}\tDuration:{king.Duration}");
        }

        private static string GetData(string url)
        {
            using WebClient wc = new WebClient();
            var JsonString = wc.DownloadString(url);
            return JsonString;
        }
        
        private static int CalculateDuration(King king)
        {
            try
            {
                string[] yearsArray = king.Years.Split("-");
                int duration = Int32.Parse(yearsArray[1]) - Int32.Parse(yearsArray[0]);
                return duration;  
            }
            catch (IndexOutOfRangeException e) //Only 1 year given, no dash present.
            {
                Console.WriteLine($"This regent only reigned for 1 year. ({king.Name}, {king.Years})");
                //Console.Write(e);
                return 1;
            }
            catch (FormatException e) //Dash present, but no second year.
            {
                Console.WriteLine($"This regent is still alive. ({king.Name}, {king.Years})");
                //Console.Write(e);
                return DateTime.Now.Year-Int32.Parse(king.Years.Remove(4)); 
            }
            catch (Exception e)
            {
                Console.WriteLine("Unfortunately, there's been an error: " + e);
                return 0;
            }
        }

        private static List<King> AppendDuration()
        {
            foreach (dynamic king in KingsList)
            {
                int duration = CalculateDuration(king);
                king.Duration = duration;
            }
            return KingsList;
        }

        private static void NumberOfMonarchs()
        {
            Console.WriteLine($"There are {KingsList.Count} monarchs in the list.");
        }

        private static void LongestRulingMonarch()
        {
            var timedKings = KingsList.OrderByDescending(x => x.Duration).ToArray();

            Console.WriteLine($"{timedKings[0].Name} reigned for {timedKings[0].Duration} years.");
        }

        private static void LongestRulingHouse()
        {
            var buildingHouseDuration =
            from king in KingsList
            group king by king.House into houseGroup
            select new
            {
                House = houseGroup.Key,
                TotalYears = houseGroup.Sum(x => x.Duration),
            };
            var houseDurations = buildingHouseDuration.OrderByDescending(x => x.TotalYears).ToArray();
            
            Console.WriteLine($"{houseDurations[0].House} reigned for {houseDurations[0].TotalYears} years.");
        }

        private static void PopularNames()
        {
            var namedSortedList = KingsList.OrderBy(x => x.Name).ToArray();

            var buildingNamesList =
                from king in namedSortedList
                group king by king.Name.Split(" ")[0] into nameGroup
                select new
                {
                    Name = nameGroup.Key,
                    CountOfNames = nameGroup.Count(),
                };
            var names = buildingNamesList.OrderByDescending(x => x.CountOfNames).ToArray();
            
            Console.WriteLine($"{names[0].Name} was used {names[0].CountOfNames} times.");
        }

        static void Main(string[] args)
        {
            AppendDuration();
            Console.WriteLine();
            NumberOfMonarchs();
            LongestRulingMonarch();
            LongestRulingHouse();
            PopularNames();
        }

        
    }
}
