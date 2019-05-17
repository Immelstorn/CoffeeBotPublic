using System;

namespace CoffeeBot.Models.DB
{
    public class Rating
    {
        public Rating()
        {
           Timestamp = DateTime.UtcNow; 
        }

        public int Id { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public bool NeedReview { get; set; }
        public DateTime Timestamp { get; set; }

        public virtual User User { get; set; }
        public virtual Place Place { get; set; }
}
}