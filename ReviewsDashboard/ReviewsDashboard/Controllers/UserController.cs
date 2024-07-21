using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReviewsDashboard.Context;
using ReviewsDashboard.Entities;
using ReviewsDashboard.Models;
using ReviewsDashboard.Privilage;
using ReviewsDashboard.Repos;

namespace ReviewsDashboard.Controllers
{
    [EnableCors("allow")]
    [ApiController]
    [AllowAnonymous]
    public class UserController : ControllerBase
    {
        private readonly UserManager<ExtendIdentityUser> userManager;
        private readonly SignInManager<ExtendIdentityUser> signInManager;
        private readonly DbContainer db;
        public UserController(UserManager<ExtendIdentityUser> userManager, SignInManager<ExtendIdentityUser> signInManager, DbContainer db)
        {
            this.db = db;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public async Task<IActionResult> UpdateNotificationSetting(NotificationSettingModel obj)
        {

            Properties p = db.properties.Find(1);
            p.Value = obj.IsOn;
            db.SaveChanges();
            await db.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [email]");

            foreach (var item in obj.Emails)
            {
                Emails e = new Emails();
                e.Sending = true;
                e.Email = item;
                db.email.Add(e);
                db.SaveChanges(true);
            }

            return Ok(true);
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> GetNotificationSetting()
        {
            NotificationSettingModel obj = new NotificationSettingModel();
            var n = db.properties.Find(1);
            switch (n.Value)
            {
                case "true":
                    obj.IsOn = "true";
                    break;
                case "false":
                    obj.IsOn = "false";
                    break;
                default:
                    n.Value = "true";
                    db.SaveChanges();
                    obj.IsOn = "true";
                    break;
            }

            obj.Emails = new List<string>();
            List<string> e = db.email.Select(a=>a.Email).ToList();
            obj.Emails.AddRange(e);
            return Ok(obj);
        }



        [Route("[controller]/[Action]")]
        [HttpGet]
        public async Task<IActionResult> GetUserInfo()
        {
            ExtendIdentityUser user = userManager.Users.First();
            GetUserInfo obj = new GetUserInfo();
            obj.Email = user.Email;
            obj.Name = user.FullName;
            return Ok(obj);
        }

        [Route("[controller]/[Action]")]
        [HttpPost]
        public async Task<IActionResult> UpdateUserInfo(GetUserInfo obj)
        {
            ExtendIdentityUser user = userManager.Users.First();

            var x = await userManager.SetEmailAsync(user, obj.Email);
            var y = await userManager.SetUserNameAsync(user, obj.Email);

            ExtendIdentityUser user2 = db.Users.First();
            user2.FullName = obj.Name;
            db.SaveChanges();
            if (x.Succeeded && y.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }


        [Route("[controller]/[Action]")]
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(ChangePasswordModel obj)
        {
            ExtendIdentityUser user = userManager.Users.First();
            var x = await userManager.ChangePasswordAsync(user, obj.OldPassword, obj.NewPassword);

            if (x.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }

        }



        [Route("[controller]/[Action]")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel obj)
        {
            var res = await signInManager.PasswordSignInAsync(obj.Email, obj.Password, true, false);

            if (res.Succeeded)
            {
                ExtendIdentityUser user = userManager.FindByEmailAsync(obj.Email).Result;
                LoginViewModel LoginModel = new LoginViewModel();
                LoginModel.Id = user.Id;
                LoginModel.FullName = user.FullName;
                LoginModel.Email = user.Email;
                LoginModel.PhoneNumber = user.PhoneNumber;
                return Ok(LoginModel);
            }
            else
            {
                return Ok(false);
            }
        }

        [Route("[controller]/[Action]")]
        [HttpGet]
        public IActionResult Register()
        {
            ExtendIdentityUser user = new ExtendIdentityUser();
            user.Email = "businesstracker2024@gmail.com";
            user.UserName = "businesstracker2024@gmail.com";
            user.PhoneNumber = "9999999999999";
            user.FullName = "Chris";
            user.EmailConfirmed = true;
            user.PhoneNumberConfirmed = true;

            var create = userManager.CreateAsync(user, "Adminpass@1").Result;
            if (create.Succeeded)
            {
                return Ok(true);
            }
            else
            {
                return Ok(false);
            }
        }

    }
}
