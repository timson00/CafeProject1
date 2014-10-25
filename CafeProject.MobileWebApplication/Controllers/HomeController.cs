using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using System.Data.Entity;
using System.Data.Sql;

using CafeProject.MobileDataLevel.Contexts;
using CafeProject.MobileWebApplication;
using CafeProject.MobileWebApplication.Models;
using CafeProject.MobileDataLevel.Entities;

using System.Net.Mail;
using System.Threading.Tasks;

namespace CafeProject.MobileWebApplication.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Nearest()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Menu(int? id)
        {
            var context = DatabaseContext.Create();
            int? i = context.ObjectMenuItems.Where(a => a.ObjectID == id).Select(a => a.FoodTypeID).FirstOrDefault();
            var obj = ShowMenuList("Show", id, i);
            return View("~/Views/Home/Menu.cshtml", obj);
        }

        [HttpPost]
        public ActionResult Menu(string command, int? objID, int? ftID)
        {
            var obj = ShowMenuList(command, objID, ftID);
            return View("~/Views/Home/Menu.cshtml", obj);
        }

        [HttpGet]
        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SignIn(SignIn model)
        {
            var context = DatabaseContext.Create();
            if (ModelState.IsValid)
            {
                var user = context.Users.Find(model.Login, model.Password);
                if (user != null)
                {
                    if (user.ConfirmEmail == true)
                    {
                    }
                    else
                    {
                        ModelState.AddModelError("", "Email не подтвержден.");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult Details(int? id)
        {
            if (!id.HasValue)
                return HttpNotFound();

            var context = DatabaseContext.Create();

            var model = context
                .Objects
                .Where(obj => obj.ID == id.Value && obj.IsWork && !obj.Deleted)
                .Select(obj => new DetailsModel()
                {
                    ObjectID = obj.ID,
                    ObjectCaption = obj.Caption,
                    ObjectType = obj.Type.Title,
                    ObjectIcon = obj.Icon,
                    ObjectAddresses = obj
                                        .Addresses
                                        .Select(address => "ул. " + address.Street.Title + " дом " + address.HouseNumber)
                                        .ToList(),
                    ObjectPhoneNumber = obj.PhoneNumber,
                    ObjectOptions = obj
                                        .Options
                                        .Select(option => new CafeProject.MobileWebApplication.Models.ObjectOption()
                                        {
                                            ID = option.ID,
                                            Title = option.Option,
                                            Icon = option.Icon
                                        }).ToList()
                }).SingleOrDefault();

            if (model == null)
                return HttpNotFound();

            return View("~/Views/Home/Details.cshtml", model);
        }

        [HttpGet]
        public ActionResult GetNearest(float? latitude, float? longitude)
        {
            if (!latitude.HasValue || !longitude.HasValue)
                return this.HttpBadRequest();

            var context = DatabaseContext.Create();
            var model = new NearestModel();

            model.Objects = context.Objects
                .Where(obj => obj.IsWork && !obj.Deleted)
                .Select(obj => new CafeProject.MobileWebApplication.Models.GeneralObject()
                {
                    ID = obj.ID,
                    Caption = obj.Caption,
                    Type = obj.Type.Title,
                    Icon = obj.Icon
                }).ToArray();

            if (Request.IsAjaxRequest())
                return PartialView("/Home/_NearestList.cshtml", model);

            return View("/Home/NearestList.cshtml", model);
        }

        //Функция для Меню
        public Object ShowMenuList(string command, int? objID, int? ftID)
        {
            if (!objID.HasValue)
                return HttpNotFound();
            var context = DatabaseContext.Create();
            var obj = context.Objects.
                Where(b => b.ID == objID).
                Select(m => new MenuModel
                {
                    ObjectID = m.ID,
                    ObjectCaption = m.Caption,
                    ObjectType = m.Type.Title,
                    ObjectIcon = m.Icon,
                    ObjectAddress = m.Addresses
                                        .Select(a => "ул." + a.Street.Title + " дом " + a.HouseNumber)
                                        .ToList(),
                    ObjectPhoneNumber = m.PhoneNumber,

                    ObjectFoodTypes = m.MenuItems
                    .Select(i => new CafeProject.MobileWebApplication.Models.FoodType() { ID = i.FoodType.ID, Type = i.FoodType.Title })
                    .Distinct()
                    .ToList(),

                    ObjectFoods = m.MenuItems.Where(f => f.FoodTypeID == ftID).Select(f => new Food()
                    {
                        ID = f.ID,
                        Name = f.FoodName,
                        Photo = f.FoodIcon,
                        Consist = f.FoodConsist,
                        Price = f.FoodPrice,
                        PriceCoins = f.FoodPriceCoins
                    }).ToList()
                }).SingleOrDefault();

            if (obj == null)
                return HttpNotFound();
            return obj;
        }

        [HttpGet]
        public ActionResult FoodSearch()
        {
            string fname = "Cake";
            var context = DatabaseContext.Create();

            var model = context.ObjectMenuItems.Where(f => f.FoodName == fname || f.FoodType.Title == fname)
    .Select(m => new Food()
    {
        Name = m.FoodName,
        Photo = m.FoodIcon,
        Consist = m.FoodConsist,
        Price = m.FoodPrice,
        PriceCoins = m.FoodPriceCoins,
        ObjectID = m.ObjectID,
        Object = m.Object.Caption,
        ObjectAddress = m.Object.Addresses.Select(a => "ул. " + a.Street.Title + " дом " + a.HouseNumber).ToList()
    });

            if (model == null)
                return Content("По вашему запросу ничего не найдено.");
            return View("~/Views/Home/FoodSearch.cshtml", model);
        }

        [HttpPost]
        public ActionResult FoodSearch(string fname)
        {

            if (string.IsNullOrEmpty(fname))
                return Content("Введите название блюда!");

            var context = DatabaseContext.Create();

            var model = context.ObjectMenuItems.Where(f => f.FoodName == fname || f.FoodType.Title == fname)
                .Select(m => new Food()
                {
                    Name = m.FoodName,
                    Photo = m.FoodIcon,
                    Consist = m.FoodConsist,
                    Price = m.FoodPrice,
                    PriceCoins = m.FoodPriceCoins,
                    Object = m.Object.Caption,
                    ObjectAddress = m.Object.Addresses.Select(a => "ул." + a.Street.Title + "дом" + a.HouseNumber)
                               .ToList()
                });
            //var model = context.ObjectMenuItems.Where(f => f.FoodName == fname || f.FoodType.Title == fname)
            //    .Select(f => new MenuModel()
            //    {
            //        ObjectCaption = f.Object.Caption,
            //        ObjectAddress = f.Object.Addresses
            //                    .Select(a => "ул." + a.Street.Title + "дом" + a.HouseNumber)
            //                    .ToList(),
            //        ObjectFoods = f.Object.MenuItems.Select(m => new Food()
            //        {
            //            Name = m.FoodName,
            //            Photo = m.FoodIcon,
            //            Consist = m.FoodConsist,
            //            Price = m.FoodPrice,
            //            PriceCoins = m.FoodPriceCoins
            //        }).ToList()
            //    }).ToArray();

            if (model == null)
                return Content("По вашему запросу ничего не найдено.");
            return View("~/Views/Home/FoodSearch.cshtml", model);
        }


    }
}
