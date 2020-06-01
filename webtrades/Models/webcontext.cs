using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webtrades.Models
{
    public class webcontext: DbContext
    {
        //public webcontext() : base("DefaultConnection")//Строка подключения из app.config
        //{ }
        
        public DbSet<Person> People { get; set; }//Создаем модель таблицы для каждой таблицы из базы данных
        public DbSet<Role> Roles { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<TradeOperation> TradeOperations { get; set; }
        //public DbSet<OperationType> OperationTypes { get; set; }
       // public DbSet<Level> Levels { get; set; }
        public DbSet<ItemPersonAccount> ItemPersonAccounts { get; set; }
        public DbSet<ExchangeRateHistory>ExchangeRateHistories  { get; set; }

        public webcontext(DbContextOptions<webcontext> options)
    : base(options)
        {
            Database.EnsureCreated();//Если бд нет, создаем ее
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Write Fluent API configurations here

            //Property Configurations
            // modelBuilder.Entity<Person>().Property(p => p.Login).HasColumnType("nvarchar").HasMaxLength(20).IsRequired().IsUnicode(true);
            //modelBuilder.Entity<OperationType>().Property(p => p.Name).HasColumnType("nvarchar").HasMaxLength(20).IsRequired().IsUnicode(true);
            //modelBuilder.Entity<Item>().Property(p => p.Name).HasColumnType("nvarchar").HasMaxLength(20).IsRequired().IsUnicode(true);
            // modelBuilder.Entity<Person>().Property(p => p.PasswordHash).HasColumnType("nvarchar").HasMaxLength(64).IsRequired().IsUnicode(true);
            // modelBuilder.Entity<Person>().Property(p => p.PasswordSalt).HasColumnType("nvarchar").HasMaxLength(50).IsRequired().IsUnicode(true);
            // modelBuilder.Entity<Role>().Property(p => p.Name).HasColumnType("nvarchar").HasMaxLength(20).IsRequired().IsUnicode(true);
            //modelBuilder.Entity<Role>().HasIndex(p => p.Name).IsUnique();
            modelBuilder.Entity<Person>().Ignore(p => p.Password);
            modelBuilder.Entity<Person>(p =>
            {
                p.Property(p => p.Login).HasColumnType("nvarchar(20)").IsRequired().IsUnicode(true);
                p.Property(p => p.PasswordHash).HasColumnType("nvarchar(64)").IsRequired().IsUnicode(true);
                p.Property(p => p.PasswordSalt).HasColumnType("nvarchar(50)").IsRequired().IsUnicode(true);
            });
            modelBuilder.Entity<Role>(p =>
            {
                p.Property(p => p.Name).HasColumnType("nvarchar(20)").IsRequired().IsUnicode(true);
            });
            modelBuilder.Entity<Item>(p =>
            {
                p.Property(p => p.Name).HasColumnType("nvarchar(20)").IsRequired().IsUnicode(true);
            });
            modelBuilder.Entity<Item>(p =>
            {
                p.Property(p => p.Name).HasColumnType("nvarchar(20)").IsRequired().IsUnicode(true);
            });
            modelBuilder.Entity<TradeOperation>(p =>
            {
                p.Property(p => p.OperationType).HasColumnType("nvarchar(10)").IsRequired().IsUnicode(true);
            });
            base.OnModelCreating(modelBuilder);

        }
    }
}
