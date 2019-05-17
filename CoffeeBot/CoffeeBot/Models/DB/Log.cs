using System;

namespace CoffeeBot.Models.DB
{
    public class Log
    {
        public Log()
        {
            Timestamp = DateTime.UtcNow;
        }

        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public virtual User User { get; set; }
        public string Message { get; set; }
        public string Addtitional { get; set; }
    }
}