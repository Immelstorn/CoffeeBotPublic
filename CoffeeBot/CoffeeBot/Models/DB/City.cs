using System.Collections.Generic;
using System.Linq;

namespace CoffeeBot.Models.DB
{
    public class City
    {
        public string GetName(Language lang)
        {
            if (Name == null)
            {
                Name = new List<Text>();
            }

            return Name.FirstOrDefault(a => a.Language == lang)?.Value
                ?? Name.FirstOrDefault()?.Value
                ?? string.Empty;
        }

        public void SetName(Language lang, string value)
        {
            if (Name == null)
            {
                Name = new List<Text>();
            }

            var existing = Name.FirstOrDefault(a => a.Language == lang);
            if (existing == null)
            {
                existing = new Text { Language = lang };
                Name.Add(existing);
            }

            existing.Value = value;
        }

        public int Id { get; set; }
        public string RealmID { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Place> Places { get; set; }
        public virtual ICollection<Text> Name { get; set; }
    }
}