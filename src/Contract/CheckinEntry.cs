using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllCheckin.Contract
{
    public enum Gendre
    {
        Unknown = 0,
        Male,
        Female,
        Other,
    }

    public class CheckinEntry
    {
        public string Name { get; set; }
        public string Id { get; set; }
        public Gendre Gendre { get; set; }
        public Nullable<DateTime> Birthdate { get; set; }
        public string Address { get; set; }
        public string CellPhoneNumber { get; set; }
        public string TelephoneNumber { get; set; }
        public string Mailbox { get; set; }
        public DateTime? CheckinTime { get; set; }
    }

}
