namespace CoffeeBot.Models.Admin
{
    public class AddViewModel
    {
        public int CityId { get; set; }
        public string Name { get; set; }
        public string Lat { get; set; }
        public string Long { get; set; }
        public string OpenTime { get; set; }
        public string CloseTime { get; set; }
        public string OpenTimeWeekend { get; set; }
        public string CloseTimeWeekend { get; set; }
        public string DescUa { get; set; }
        public string DescRu { get; set; }
        public string DescEn { get; set; }
        public string AddressUa { get; set; }
        public string AddressRu { get; set; }
        public string AddressEn { get; set; }

        public bool NonDairyMilk { get; set; }
        public bool CoffeeToGo { get; set; }
        public bool Terrace { get; set; }
        public bool Kitchen { get; set; }
        public bool WiFi { get; set; }
        public bool CoWorking { get; set; }
        public bool PetFriendly { get; set; }
        public bool Restroom { get; set; }
        public bool SaleOfCoffeeBeans { get; set; }
        public bool Alcohol { get; set; }

        public bool Active { get; set; }
    }
}