using System.Data.Entity;

using CoffeeBot.Models.DB;

namespace CoffeeBot.Models
{
    public class DataContext : DbContext
    {
        public DataContext() : base("name=DefaultConnectionString") { }

        public DbSet<User> Users { get; set; }
        public DbSet<Place> Places { get; set; }
        public DbSet<Rating> Ratings { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Text> Texts { get; set; }
        public DbSet<Localization> Localization { get; set; }
        public DbSet<Suggestion> Suggestions { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDefaultSchema("coffee");

            modelBuilder.Entity<Place>().HasMany(p => p.Address).WithMany().Map(m => m.ToTable("PlacesTextsAddress"));
            modelBuilder.Entity<Place>().HasMany(p => p.Description).WithMany().Map(m => m.ToTable("PlacesTextsDescription"));
            modelBuilder.Entity<City>().HasMany(p => p.Name).WithMany();

            modelBuilder.Entity<Localization>().HasKey(l => l.Name);
            modelBuilder.Entity<Localization>().HasMany(l => l.Texts).WithMany();
        }
    }
}