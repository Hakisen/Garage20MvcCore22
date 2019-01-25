using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Garage20MvcCore22.Models
{
    public class Garage20MvcCore22Context : DbContext
    {
        public Garage20MvcCore22Context (DbContextOptions<Garage20MvcCore22Context> options)
            : base(options)
        {
        }

        public DbSet<Garage20MvcCore22.Models.ParkedVehicle> ParkedVehicle { get; set; }
    }
}
