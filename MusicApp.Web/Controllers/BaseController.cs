using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data;
using Data.Domain;
using DataAccess;
using Ninject;
using SocialApp.Models;
using WebMatrix.WebData;

namespace SocialApp.Controllers
{
    public class BaseController : Controller
    {
        public int CurrentUserId
        {
            get
            {
                return WebSecurity.GetUserId(User.Identity.Name);
            }
        }

        public string GenerateFileName()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
