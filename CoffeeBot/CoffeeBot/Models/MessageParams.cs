using System.Collections.Generic;

namespace CoffeeBot.Models
{
    public class MessageParams
    {
        public MessageParams() { }

        public MessageParams(IReadOnlyList<string> param)
        {
            Page = int.Parse(param[0]);
            Longitude = float.Parse(param[1]);
            Latitude = float.Parse(param[2]);
            Distance = int.Parse(param[3]);
        }

        public int Page { get; set; }
        public int Distance { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
    }
}