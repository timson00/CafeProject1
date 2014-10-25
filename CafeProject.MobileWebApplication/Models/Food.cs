using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.ComponentModel.DataAnnotations;

namespace CafeProject.MobileWebApplication.Models
{
    public class Food
    {
        public int ID { get; set; }
        public string Type { get; set; }
        public string Photo { get; set; }
        public string Name { get; set; }
        public string Consist { get; set; }
        [DataType(DataType.Currency)]
        public int Price { get; set; }
        public int PriceCoins { get; set; }
        public int ObjectID { get; set; }
        public string Object { get; set; }
        public ICollection<string> ObjectAddress { get; set; }
    }
}