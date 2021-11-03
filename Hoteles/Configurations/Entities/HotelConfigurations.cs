using Hoteles.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hoteles.Configurations.Entities
{
    public class HotelConfigurations : IEntityTypeConfiguration<Hotel>
    {
        public void Configure(EntityTypeBuilder<Hotel> builder)
        {
            builder.HasData(
                new Hotel
                {
                    Id = 1,
                    Name = "Sandls Resort and spa",
                    Address = "Calle 15 nro 14",
                    CountryId = 1,
                    Rating = 4.5

                },
                new Hotel
                {
                    Id = 2,
                    Name = "grancolina",
                    Address = "Calle 18 nro 10",
                    CountryId = 2,
                    Rating = 4

                },
                new Hotel
                {
                    Id = 3,
                    Name = "vuelta al mundo",
                    Address = "carrera 17 nro 12 sur",
                    CountryId = 3,
                    Rating = 5

                }

             );
        }
    }
}
