using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using Business.Services;
using Data;
using Data.Domain;
using DataAccess;
using SocialApp.Models;
using System.Data.Entity;

namespace SocialApp.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        public ActionResult Show(int id)
        {
            User user = userService.FindUserById(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Update(UserUpdateModel model)
        {
            User currentUser = userService.FindUserById(CurrentUserId);
            if (model.Picture != null)
            {
                string extension = System.IO.Path.GetExtension(model.Picture.FileName);
                string relativePicturePath = string.Format("{0}/{1}{2}", WebConfigurationManager.AppSettings["ProfilePicturesFolderPath"], FileUtils.GenerateFileName(), extension);
                string serverPicturePath = Server.MapPath(string.Format("~/{0}", relativePicturePath));
                model.Picture.SaveAs(serverPicturePath);
                currentUser.PictureFilePath = relativePicturePath;
            }
            userService.UpdateUserInfo(currentUser.Id, model);
            return RedirectToAction("Settings");
        }

        public ViewResult Settings()
        {
            User currentUser = userService.FindUserById(CurrentUserId);
            var model = Mapper.Map<User, UserUpdateModel>(currentUser);
            return View(model);
        }

        public JsonResult UploadedSongs()
        {
            IEnumerable<Song> songs = userService.GetUploadedSongs(CurrentUserId);
            return Json(songs, JsonRequestBehavior.AllowGet);

        }

        public JsonResult Current()
        {
            return Json(userService.FindUserById(CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Library()
        {
            return View();
        }
    }
}
