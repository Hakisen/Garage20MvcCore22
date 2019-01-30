using System;
namespace Garage20MvcCore22.ViewModels
{
    [Serializable]
    public class Kvitto
    {
        public string RegNr
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
