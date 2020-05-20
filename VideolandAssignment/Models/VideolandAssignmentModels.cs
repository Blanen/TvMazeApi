using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace VideolandAssignment.Models
{
    public class VideolandAssignmentContext : DbContext
    {
        public DbSet<Show> Shows { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<ShowPerson> ShowPersons { get; set; }
        public DbSet<UnixTimeStamp> UnixTimeStamps { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
           optionsBuilder.UseSqlite("Data source=Shows.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShowPerson>()
                .HasKey(sp => new {sp.PersonId, sp.ShowId});

            modelBuilder.Entity<ShowPerson>()
                .HasOne(sp => sp.Show)
                .WithMany(s => s.ShowPeople)
                .HasForeignKey(sp => sp.ShowId);

            modelBuilder.Entity<ShowPerson>()
                .HasOne(sp => sp.Person)
                .WithMany(p => p.ShowPeople)
                .HasForeignKey(sp => sp.PersonId);

        }
    }
    public class Show 
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public ICollection<ShowPerson> ShowPeople { get; set; }

    }

    public class ShowPerson
    {
        public long ShowId { get; set; }
        public Show Show { get; set; }
        public long PersonId { get; set; }
        public Person Person { get; set; }
    }

    public class Person
    {
        public long Id { get; set; }

        public string Name { get; set; }

        public DateTime? Birthday { get; set; }
        public ICollection<ShowPerson> ShowPeople { get; set; }
    }

    public class UnixTimeStamp
    {
        public string Id { get; set; }
        public long TimeStamp { get; set; }
    }
}
