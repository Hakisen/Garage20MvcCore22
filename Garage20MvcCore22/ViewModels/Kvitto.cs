using System;
using System.ComponentModel.DataAnnotations;

namespace Garage20MvcCore22.ViewModels
{
    [Serializable]
    public class Kvitto
    {
        [Display(Name = "Reg. Nr.")]
        public string RegNr
        {
            get;
            set;
        }

        [Display(Name = "Check In Time")]
        public DateTime StartTime
        {
            get;
            set;
        }

        [Display(Name = "Check Out Time")]
        public DateTime EndTime
        {
            get;
            set;
        }

        [Display(Name = "Total Park Time")]
        public String Duration
        {
            get;
            set;
        }

        [Display(Name = "Total Cost")]
        public double TotalPrice
        {
            get;
            set;
        }

    }
}
