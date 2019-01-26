using System;
namespace Garage20MvcCore22.ViewModels
{
    public class Kvitto
    {
        public int RegNr
        {
            get;
            set;
        }

        public DateTime StartTime
        {
            get;
            set;
        }

        public DateTime EndTime
        {
            get;
            set;
        }


        public String Duration
        {
            get;
            set;
        }

        public double TotalPrice
        {
            get;
            set;
        }

    }
}
