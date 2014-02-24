using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using Business;
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
            var profile = userService.GetProfileInfoFor(
                userId: id,
                currentUserId: CurrentUserId
            );
            if (profile == null)
            {
                return HttpNotFound();
            }
            return View(profile);
        }

        [HttpPost]
        public ActionResult Update(UserUpdateViewModel viewModel)
        {
            User currentUser = userService.FindUserById(CurrentUserId);
            if (viewModel.Picture != null)
            {
                string extension = System.IO.Path.GetExtension(viewModel.Picture.FileName);
                string relativePicturePath = string.Format("{0}/{1}{2}", WebConfigurationManager.AppSettings["ProfilePicturesFolderPath"], FileUtils.GenerateFileName(), extension);
                string serverPicturePath = Server.MapPath(string.Format("~/{0}", relativePicturePath));
                viewModel.Picture.SaveAs(serverPicturePath);
                currentUser.PictureFilePath = relativePicturePath;
            }
            userService.UpdateUserInfo(currentUser.Id, viewModel);
            return RedirectToAction("Settings");
        }

        public ActionResult SendFriendRequest(int senderId, int receiverId)
        {
            userService.SendFriendRequest(senderId, receiverId);
            return RedirectToAction("Show", new { id = CurrentUserId });
        }

        public ViewResult Settings()
        {
            User currentUser = userService.FindUserById(CurrentUserId);
            var model = Mapper.Map<User, UserUpdateViewModel>(currentUser);
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
