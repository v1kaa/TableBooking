﻿using Microsoft.EntityFrameworkCore;
using TableBooking.Models;
namespace TableBooking.Data
{
    public class AppDbContext:DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

       public DbSet<Table> Tables { get; set; }
       public DbSet<Reservation> Reservations { get; set; }
    }
}
