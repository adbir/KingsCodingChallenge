using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;

namespace The_Kings_adbir_
{
    public partial class King
    {
        public static List<King> FromJson(string json) => JsonConvert.DeserializeObject<List<King>>(json, The_Kings_adbir_.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this List<King> self) => JsonConvert.SerializeObject(self, The_Kings_adbir_.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
    public partial class King
    {
        public List<King> data { get; set; }

        public King(int Id, string Name, string Country, string House, string Reign, int Duration)
        {
            this.Id = Id;
            this.Name = Name;
            this.Country = Country;
            this.House = House;
            this.Reign = Reign;
            this.Duration = Duration;
        }
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("nm")]
        public string Name { get; set; }
        [JsonProperty("cty")]
        public string Country { get; set; }
        [JsonProperty("hse")]
        public string House { get; set; }
        [JsonProperty("yrs")]
        public string Reign { get; set; }
        [JsonProperty("dur")]
        public int Duration { get; set; }
    }
    
    class Program
    {
        private static string  dataSource = "https://gist.githubusercontent.com/christianpanton/10d65ccef9f29de3acd49d97ed423736/raw/b09563bc0c4b318132c7a738e679d4f984ef0048/kings";
        static dynamic  kings = GetDataArray(dataSource);

        static readonly HttpClient client = new HttpClient();

        static void ShowKing(King king)
        {
            Console.WriteLine($"Navn: {king.Name}\tCountry:{king.Country}\tHouse:{king.House}\t");
        }

        private static JArray GetDataArray(string url)
        {
            using WebClient wc = new WebClient();
            var JsonString = wc.DownloadString(url);
            JsonTextReader reader = new JsonTextReader(new StringReader(JsonString));
            JArray JsonArray = JArray.Parse(JsonString) as JArray;
            
            Console.WriteLine(JsonString.ToString());
            return JsonArray;
        }
        private static void NumberOfMonarchs()
        {
            Console.WriteLine($"There are {GetDataArray(dataSource).Count} monarchs in the list."); //1. How many monarchs are in the list?
        }
        private static JArray AppendDuration()
        {
            foreach (dynamic king in kings)
            {
                int duration = CalculateDuration(king);
                JObject jObject = new JObject { { "dur", duration } };
                king.Merge(jObject, new JsonMergeSettings { MergeArrayHandling = MergeArrayHandling.Concat });
            }
            return kings;
        }

        private static int CalculateDuration(dynamic king)
        {
            try
            {
                string reign = king.GetValue("yrs");
                string[] calculationArray = reign.Split("-");
                int duration = Int32.Parse(calculationArray[1]) - Int32.Parse(calculationArray[0]);
                return duration;
            }
            catch (IndexOutOfRangeException e) //Only 1 year given, no dash present.
            {
                Console.WriteLine($"This regent only reigned for 1 year. ({king.GetValue("nm")})");
                //Console.Write(e);
                return 1;
            }
            catch (FormatException e) //Dash present, but no second year.
            {
                Console.WriteLine($"This regent is still alive. ({king.GetValue("nm")})");
                //Console.Write(e);
                
                return DateTime.Now.Year-1952; 
            }
            catch (Exception e)
            {
                Console.WriteLine("Unfortunately, there's been an error: " + e);
                return 0;
            }
        }


        private static void LongestRulingMonarch()
        {

        }

        

    /*while(reader.Read())
    {

        if (reader.Value != null)
        {
            Console.WriteLine("Token: {0}, Value: {1}", reader.TokenType, reader.Value);
        }
        else 
        {
            Console.WriteLine("Token: {0}", reader.TokenType);
        }
    }*/


    static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
            NumberOfMonarchs();
            JArray a = AppendDuration();
        
            LongestRulingMonarch();

        }
    }
}
