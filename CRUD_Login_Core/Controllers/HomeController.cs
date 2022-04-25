using CRUD_Login_Core.DB_Connection;
using CRUD_Login_Core.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CRUD_Login_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            TemplateContext tc = new TemplateContext();

            var re = tc.UseTemps.ToList();

            List<DataModel> dm = new List<DataModel>();

            foreach (var item in re)
            {
                dm.Add(new DataModel
                {
                    Id = item.Id,
                    Name = item.Name,
                    Moblie=item.Moblie,
                    Depart=item.Depart,
                    Email=item.Email
                }) ;
            }



            return View(dm);
        }

        [HttpGet]
        public IActionResult AddData()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddData(DataModel dm)
        {
            TemplateContext tc = new TemplateContext();

            UseTemp ut = new UseTemp();

            ut.Id = dm.Id;
            ut.Name = dm.Name;
            ut.Moblie = dm.Moblie;
            ut.Depart = dm.Depart;
            ut.Email = dm.Email;

            if (dm.Id == 0)
            {
                tc.UseTemps.Add(ut);
                tc.SaveChanges();

                
            }
            else
            {
                tc.Entry(ut).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                tc.SaveChanges();
            }

            

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            TemplateContext tc = new TemplateContext();

            var re = tc.UseTemps.Where(m => m.Id == id).First();

            DataModel dm = new DataModel();

            dm.Id = re.Id;
            dm.Name = re.Name;
            dm.Moblie = re.Moblie;
            dm.Depart = re.Depart;
            dm.Email = re.Email;


            return View("AddData",dm);
        }

        public IActionResult Delete(int id)
        {
            TemplateContext tc = new TemplateContext();
            var re = tc.UseTemps.Where(n => n.Id == id).First();

            tc.UseTemps.Remove(re);

            tc.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Login()
        {


            return View();
        }

        [HttpPost]
        public IActionResult Login(UserModel ud)
        {
            TemplateContext tc = new TemplateContext();

            var res = tc.Logiuses.Where(m => m.Email == ud.Email).FirstOrDefault();

            if (res == null)
            {
                TempData["invalid"] = "Email is not foumd";
            }
            else
            {
                if (res.Email == ud.Email && res.Password == ud.Password)
                {

                    var claims = new[] { new Claim(ClaimTypes.Name,res.Name),new Claim(ClaimTypes.Email, res.Email) };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true
                    };

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity), authProperties);
                    HttpContext.Session.SetString("Email", ud.Email);
                    HttpContext.Session.SetString("Name", res.Name);

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.inv = "Wrong Email Id or password";
                    return View();
                }
            }

            return View();
        }


        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


            return View("Login");
 
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
