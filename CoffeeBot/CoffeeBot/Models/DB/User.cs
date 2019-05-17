using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.SqlTypes;

namespace CoffeeBot.Models.DB
{
    public class User
    {
        public User()
        {
            Created = SqlDateTime.MinValue.Value;
        }

        public int Id { get; set; }
        public string Username { get; set; }
        public long ChatId { get; set; }
        public DateTime Created { get; set; }
        public DateTime LastMessageTime { get; set; }
        public UserMode UserMode { get; set; }
        public int MessageCount { get; set; }
        public string UserModeParams { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public virtual UserSettings Settings { get; set; }
        public virtual List<Rating> Ratings { get; set; }

        [NotMapped]
        public bool IsNew { get; set; }

    }

}