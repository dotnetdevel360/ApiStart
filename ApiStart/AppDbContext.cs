using ApiStart.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace ApiStart
{

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
         
        public DbSet<Users> Users { get; set; } = null!;
    }
}
