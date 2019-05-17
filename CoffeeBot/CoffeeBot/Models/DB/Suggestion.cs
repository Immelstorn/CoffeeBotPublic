using System;

namespace CoffeeBot.Models.DB
{
    public class Suggestion
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime DateTime { get; set; }
        public User User { get; set; }
    }
}