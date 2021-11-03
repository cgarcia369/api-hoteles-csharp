using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hoteles.Configurations.Entities;
using Hoteles.Data.Models;
using Hoteles.Data.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Hoteles.Data.Context
{
    public class DatabaseContext : IdentityDbContext<ApiUser>
    {
        public DatabaseContext(DbContextOptions dbContextOptions)
        : base(dbContextOptions)
        {

        }
        public  DbSet<Country> Countries { get; set; }
        public  DbSet<Hotel> Hotels { get; set; }
        protected override void OnModelCreating(ModelBuilder model)
        {
            base.OnModelCreating(model);

            model.ApplyConfiguration(new CountryConfiguration());
            model.ApplyConfiguration(new HotelConfigurations());
            model.ApplyConfiguration(new RolConfiguration());
   
        }
    }
}
