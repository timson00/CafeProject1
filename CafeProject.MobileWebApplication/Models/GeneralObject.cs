using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeProject.MobileWebApplication.Models
{
    public class GeneralObject
    {
        public int ID { get; set; }
        public string Caption { get; set; }
        public string Type { get; set; }
        public string Icon { get; set; }
        public ICollection<string> Addresses { get; set; }
        public string PhoneNumber { get; set; }
        public ICollection<ObjectOption> Options { get; set; } 
    }
}