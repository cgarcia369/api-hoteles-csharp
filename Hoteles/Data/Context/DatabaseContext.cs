using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoteles.Data.Models;

namespace Hoteles.Data.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public  DbSet<Country> Countries { get; set; }
        public  DbSet<Hotel> Hotels { get; set; }

    }
}
