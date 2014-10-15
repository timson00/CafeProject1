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

using System.Security.Cryptography;

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
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(CafeProject.MobileWebApplication.Models.User user)
        {
            if (user.Surname.Length < 2 && user.Surname.Length > 255)
            {
                ModelState.AddModelError("Surname", "Длина строки должна быть от 2 до 255 символов");
            }
            if (user.Name.Length < 2 && user.Name.Length > 255)
            {
                ModelState.AddModelError("Name", "Длина строки должна быть от 2 до 255 символов");
            }
            if (user.Login.Length < 5 && user.Login.Length > 100)
            {
                ModelState.AddModelError("Login", "Длина строки должна быть от 5 до 100 символов");
            }
            if (ModelState.IsValid)
            {
                byte b = 0;
                var context = DatabaseContext.Create();
                switch (user.Gender)
                {
                    case "Муж.":
                        b = 0;
                        break;
                    case "Жен.":
                        b = 1;
                        break;
                    default:
                        break;
                }
                byte[] ps = Sha(user.Password);
                CafeProject.MobileDataLevel.Entities.User us = new CafeProject.MobileDataLevel.Entities.User()
                {
                    ID = Guid.NewGuid(),
                    FirstName = user.Name,
                    MiddleName = user.Surname,
                    Login = user.Login,
                    Gender = b,
                    Email = user.Email,
                    ConfirmEmail = false
                };
                us.Password = new Password() { PasswordHash = ps };
                context.Users.Add(us);
                context.SaveChangesAsync();
                //bool a = true;
                //if (a)
                //{
                    MailAddress from = new MailAddress("timson7@yandex.ru", "Регистрация");
                    MailAddress to = new MailAddress(user.Email);
                    MailMessage mess = new MailMessage(from, to);
                    mess.Subject = "Подтверждение регистрации";
                    mess.Body = string.Format("Для завершения регистрации перейдите по ссылке:" +
                            "<a href=\"{0}\" title=\"Подтвердить регистрацию\">{0}</a>",
                            Url.Action("ConfirmEmail", "HomeController",
                            new { Token = us.ID, Email = user.Email }, Request.Url.Scheme));
                    mess.IsBodyHtml = true;

                    SmtpClient smpt = new System.Net.Mail.SmtpClient("smtp.yandex.ru", 25);

                    smpt.Credentials = new System.Net.NetworkCredential("timson7@yandex.ru", "2240800");
                    smpt.EnableSsl = true;
                    smpt.Send(mess);
                    return RedirectToAction("Confirm", "HomeController", new { Email = user.Email });
                //}
                //ViewBag.Message = "Регистрация прошла успешно";
            }
            return View(user);
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
                return PartialView("~/Views/Home/_NearestList.cshtml", model);

            return View("~/Views/Home/NearestList.cshtml", model);
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
                        Price = f.FoodPrice
                    }).ToList()
                }).SingleOrDefault();

            if (obj == null)
                return HttpNotFound();
            return obj;
        }

        //Хэш пароля
        public byte[] Sha(string password)
        {
            HashAlgorithm sha = new SHA512Managed();
            byte[] pass = System.Text.Encoding.UTF8.GetBytes(password);
            byte[] result = sha.ComputeHash(pass);
            sha.Dispose();
            return result;
        }

        //Для эл.адреса
        public ActionResult Confirm(string Email)
        {
            ViewBag.Message = "На ваш почтовый адрес: " + Email + "Вам высланы дальнейщие инструкции по завершению регистрации";
            return View();
        }

        //Подтверждение эл.адреса
        public ActionResult ConfirmEmail(string ID, string Email)
        {
            CafeProject.MobileDataLevel.Entities.User us = new CafeProject.MobileDataLevel.Entities.User();
            var context = DatabaseContext.Create();
            var u = context.Users.Where(i => i.ID == new Guid(ID)).SingleOrDefault();
            if (u != null)
            {
                var e = context.Users.Where(i => i.Email == Email).SingleOrDefault();
                if (e != null)
                {
                    us.ConfirmEmail = true;
                    context.SaveChanges();
                }
                else
                {
                    ViewBag.Message = "Регистрация не пройдена в связи не верного Email адреса";
                }
            }
            else
            {
                ViewBag.Message = "Пользователь не найден";
            }
            return Content("Подтвержение прошло успешно!");
        }

    }
}
