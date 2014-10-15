using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CafeProject.MobileWebApplication.Models
{
    public class MenuModel
    {
        public int ObjectID { get; set; }
        public string ObjectCaption { get; set; }
        public string ObjectType { get; set; }
        public string ObjectIcon { get; set; }
        public ICollection<string> ObjectAddress { get; set; }
        public string ObjectPhoneNumber { get; set; }
        public IEnumerable<FoodType> ObjectFoodTypes { get; set; }
        public IEnumerable<Food> ObjectFoods { get; set; }
    }
}