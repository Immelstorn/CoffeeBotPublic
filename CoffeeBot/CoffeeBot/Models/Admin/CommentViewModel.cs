using System;

namespace CoffeeBot.Models.Admin
{
    public class CommentViewModel
    {
        public int Id { get; set; }
        public string User { get; set; }
        public string Place { get; set; }
        public int Stars { get; set; }
        public string Comment { get; set; }
        public bool NeedReview { get; set; }
        public DateTime Timestamp { get; set; }
    }
}