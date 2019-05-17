using System;

namespace CoffeeBot.Models.DB
{
    [Flags]
    public enum Perk
    {
        NonDairyMilk = 1 << 1,
        CoffeeToGo = 1 << 2,
        Terrace = 1 << 3,
        IrishCoffee = 1 << 4,
        Kitchen = 1 << 5,
        WiFi = 1 << 6,
        InHouseRoster = 1 << 7,
        BaristaCourses = 1 << 8,
        CoWorking = 1 << 9,
        PetFriendly = 1 << 10,
        Restroom = 1 << 11,
        SaleOfCoffeeBeans = 1 << 12,
        Alcohol = 1 << 13
    }
}