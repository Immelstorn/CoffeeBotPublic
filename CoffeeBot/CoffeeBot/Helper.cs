using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using CoffeeBot.Models;
using CoffeeBot.Models.DB;

using GeoTimeZone;

using TimeZoneConverter;

namespace CoffeeBot
{
    public static class Helper
    {
        public static string IsoCountryCodeToFlagEmoji(string country)
        {
            return string.Concat(country.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
        }

        public static readonly Dictionary<Perk, string> Emoji = new Dictionary<Perk, string> {
            {Perk.WiFi, "📶" },
            {Perk.SaleOfCoffeeBeans, "🛒" },
            {Perk.NonDairyMilk, "🌱" },
            {Perk.Restroom, "🚽" },
            {Perk.PetFriendly, "🐶" },
            {Perk.Kitchen, "🍽️" },
            {Perk.CoffeeToGo, "🥤" },
            {Perk.CoWorking, "💻" },
            {Perk.Alcohol, "🍷" },
        };

        public static string GetPerks(Perk perks)
        {
            var retval = new StringBuilder();
            foreach (Perk p in Enum.GetValues(typeof(Perk)))
            {
                if (perks.HasFlag(p) && Emoji.ContainsKey(p))
                {
                    retval.Append($"{Emoji[p]}");
                }
            }

            return retval.ToString().Trim();
        }

        public static double Radians(double x)
        {
            return x * Math.PI / 180;
        }

        public static double GetDistance(double lon1, double lat1, double lon2, double lat2)
        {
            const int R = 6378137; // m

            var f1 = Radians(lat1);
            var f2 = Radians(lat2);
            var df = f2 - f1;
            var dl = Radians(lon2) - Radians(lon1);

            var a = Math.Sin(df / 2) * Math.Sin(df / 2) +
                Math.Cos(f1) * Math.Cos(f2) *
                Math.Sin(dl / 2) * Math.Sin(dl / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var d = R * c;

            return d;
        }

        public static string GetStars(int count)
        {
            var stars = new StringBuilder();
            for (var i = 0; i < count; i++)
            {
                stars.Append("⭐");
            }

            return stars.ToString();
        }

        public static string GetAddressLink(Place place, Language lang)
        {
            var placeLink = $"Google&query_place_id={place.GoogleID}";
            var locationLink = $"{place.Latitude},{place.Longitude}";
            var link = String.IsNullOrEmpty(place.GoogleID) ? locationLink : placeLink;
            return  $"📬 [{place.GetAddress(lang)}](https://www.google.com/maps/search/?api=1&query={link})";
        }

        public static string GetOpenStatus(Place place)
        {
            var tzIana = TimeZoneLookup.GetTimeZone(place.Latitude, place.Longitude).Result;
            var tzMs = TZConvert.IanaToWindows(tzIana);
            var tzInfo = TimeZoneInfo.FindSystemTimeZoneById(tzMs);
            var now = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, tzInfo);

            var isWeekend = now.DayOfWeek == DayOfWeek.Saturday || now.DayOfWeek == DayOfWeek.Sunday;

            return !isWeekend & now.TimeOfDay > place.OpenTime && now.TimeOfDay < place.CloseTime
                || isWeekend & now.TimeOfDay > place.OpenTimeWeekend && now.TimeOfDay < place.CloseTimeWeekend
                    ? String.Empty
                    : "🔒 ";
        }

        public static async Task<string> GetLocalizationAsync(LocalizationKeys key, Language lang, DataContext db)
        {
            var localization = await db.Localization.FirstOrDefaultAsync(l => l.Name == key.ToString());
            if (localization != null)
            {
                return localization.Texts.FirstOrDefault(t => t.Language == lang)?.Value ?? localization.Texts.First(t => t.Language == lang).Value;
            }

            return String.Empty;
        }
    }
}