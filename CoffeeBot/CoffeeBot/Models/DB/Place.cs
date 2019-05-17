using System;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeBot.Models.DB
{
    public class Place
    {
        public string GetAddress(Language lang)
        {
            if (Address == null)
            {
                Address = new List<Text>();
            }

            return Address.FirstOrDefault(a => a.Language == lang)?.Value
                ?? Address.FirstOrDefault()?.Value
                ?? string.Empty;
        }

        public void SetAddress(Language lang, string value)
        {
            if (Address == null)
            {
                Address = new List<Text>();
            }

            var existing = Address.FirstOrDefault(a => a.Language == lang);
            if (existing == null)
            {
                existing = new Text { Language = lang};
                Address.Add(existing);
            }

            existing.Value = value;
        }

        public string GetDescription(Language lang)
        {
            if (Description == null)
            {
                Description = new List<Text>();
            }

            return Description.FirstOrDefault(a => a.Language == lang)?.Value
                ?? Description.FirstOrDefault()?.Value
                ?? string.Empty;
        }

        public void SetDescription(Language lang, string value)
        {
            if (Description == null)
            {
                Description = new List<Text>();
            }

            var existing = Description.FirstOrDefault(a => a.Language == lang);
            if (existing == null)
            {
                existing = new Text { Language = lang };
                Description.Add(existing);
            }

            existing.Value = value;
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public TimeSpan? OpenTimeWeekend { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public TimeSpan? CloseTimeWeekend { get; set; }
        public City City { get; set; }
        public bool Active { get; set; }
        public Perk Perks { get; set; }

        public string FoursquareID { get; set; }
        public string GoogleID { get; set; }
        public int RealmID { get; set; }

        public virtual ICollection<Text> Description { get; set; }
        public virtual ICollection<Text> Address { get; set; }
        public virtual ICollection<Rating> Ratings { get; set; }
    }

}