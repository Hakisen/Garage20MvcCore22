using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage20MvcCore22.Models
{
    public class ParkedVehicle
    {
        public int Id { get; set; }
        public VehicleType VehicleType { get; set; }
        [Required]
        [Display(Name = "Reg.No")]
        public string RegNr { get; set; }
        [Display(Name = "Nr. Wheels")]
        public int NrOfWheels { get; set; }
        public string Color { get; set; }
        public string Model { get; set; }
        public string Brand { get; set; }
        [Display(Name = "Arrival")]
        [DataType(DataType.DateTime)]
        public DateTime StartTime { get; set; }
        public bool Parked { get; set; }
    }
}
