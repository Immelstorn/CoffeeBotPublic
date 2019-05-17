using System.Collections.Generic;

namespace CoffeeBot.Models.DB
{
    public class Localization
    {
        public string Name { get; set; }
        public virtual ICollection<Text> Texts { get; set; }
    }
}