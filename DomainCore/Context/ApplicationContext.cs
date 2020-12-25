using System.Linq;
using DomainCore.Models;
using Microsoft.EntityFrameworkCore;

namespace DomainCore.Context
{
    public class ApplicationContext : DbContext
    {
        public static string ConnectionString { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<InputType> InputTypes { get; set; }
        public DbSet<Template> Templates { get; set; }
        public DbSet<RecordCard> RecordCards { get; set; }

        public ApplicationContext()
        {
            //Database.EnsureDeleted();
            Database.EnsureCreated();
            
            InitData();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        { 
            optionsBuilder.UseSqlServer(ConnectionString);
            optionsBuilder.LogTo(message => System.Diagnostics.Debug.WriteLine(message));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        private void InitData()
        {
            if (!InputTypes.Any())
            {
                InputType[] types = new InputType[]
                {
                    new InputType() { Type = "Text", Size = 0 },
                    new InputType() { Type = "Text", Size = 1 },
                    new InputType() { Type = "Text", Size = 2 }
                };
                InputTypes.AddRange(types);
                SaveChanges();
            }
        }
    }
}
