using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Data;
using Data.Domain;
using DataAccess;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using SocialApp.Models;

namespace SocialApp.Controllers
{
    [Authorize]
    public class AccountController : BaseController
    {
        public const string DefaultProfilePicturePath = "/Content/images/default_profile_2.png";

        private readonly SocialAppContext db;
        private readonly IEmailSender emailSender;

        public AccountController(SocialAppContext db, IEmailSender emailSender)
        {
            this.db = db;
            this.emailSender = emailSender;
        }

        [AllowAnonymous]
        public ActionResult Activate(string code)
        {
            User user = db.Users.FirstOrDefault(u => u.ActivationCode == code);
            if (user == null)
            {
                TempData["danger"] = "No user wtih given activation code was found";
                return RedirectToAction("Index", "Home");
            }
            user.IsActivated = true;
            db.SaveChanges();
            LoginModel model = new LoginModel { Email = user.Email, Password = user.Password };
            return Login(model, returnUrl: null);
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            User user = db.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null)
            {
                TempData["danger"] = "Invalid email or password";
                return View(model);
            }
            if (!user.IsActivated)
            {
                TempData["danger"] = "Your account hasn't been activated yet";
                return View(model);
            }
            if (WebSecurity.Login(model.Email, model.Password, persistCookie: model.RememberMe))
            {
                if (returnUrl != null)
                {
                    return RedirectToLocal(returnUrl);
                }
                return RedirectToAction("Show", "User", new { id = user.Id });
            }

            ModelState.AddModelError("", "Invalid email or password");
            return View(model);
        }

        public ActionResult LogOff()
        {
            WebSecurity.Logout();

            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(User user)
        {
            if (!ModelState.IsValid)
            {
                return View(user);
            }
            User existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "Email is already taken. Choose another");
                return View(user);
            }
            string activationCode = Guid.NewGuid().ToString();
            try
            {
                emailSender.SendActivationCode(user.Email, activationCode);
            }
            catch (FormatException)
            {
                ModelState.AddModelError("Email", "Email format is incorrect");
            }
            // Attempt to register the user
            try
            {
                WebSecurity.CreateUserAndAccount(user.Email, user.Password, propertyValues: new
                {
                    Password = user.Password,
                    ActivationCode = activationCode,
                    IsActivated = false,
                    Role = Role.User,
                    FullName = "FullName",
                    Country = "Country",
                    City = "City",
                    About = "About",
                    PictureFilePath = "/Content/images/default-profile-picture-b&w.png"
                });
            }
            catch (MembershipCreateUserException e)
            {
                ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                return View(user);
            }

            TempData["success"] = string.Format("An email with activation link has been sent to {0}. Check it out", user.Email);
            return RedirectToAction("Index", "Home");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public enum ManageMessageId
        {
            ChangePasswordSuccess,
            SetPasswordSuccess,
            RemoveLoginSuccess,
        }

        internal class ExternalLoginResult : ActionResult
        {
            public ExternalLoginResult(string provider, string returnUrl)
            {
                Provider = provider;
                ReturnUrl = returnUrl;
            }

            public string Provider { get; private set; }
            public string ReturnUrl { get; private set; }

            public override void ExecuteResult(ControllerContext context)
            {
                OAuthWebSecurity.RequestAuthentication(Provider, ReturnUrl);
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
