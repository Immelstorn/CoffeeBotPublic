using CoffeeBot.Models.DB;

namespace CoffeeBot.Models.Admin
{
    public class PlaceViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DescriptionRu { get; set; }
        public string DescriptionEn { get; set; }
        public string DescriptionUa { get; set; }
        public string AddressRu { get; set; }
        public string AddressEn { get; set; }
        public string AddressUa { get; set; }
        public double Longitude { get; set; }
        public double Latitude { get; set; }
        public string FoursquareId { get; set; }
        public string GoogleId { get; set; }
        public bool Active { get; set; }
    }
}