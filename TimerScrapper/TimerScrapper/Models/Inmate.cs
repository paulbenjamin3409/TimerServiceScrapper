using System;
using System.Collections.Generic;
using System.Text;

namespace TimerScrapper.Models
{
    public class Inmate : Person
    {
        public string InmateNumber { get; set; }
        public string IntakeDate { get; set; }
        public string IntakeTime { get; set; }
        public string Facility { get; set; }
        public string BondAmount { get; set; }
        public string Charges { get; set; }
    }
}
