using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
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
        private readonly SocialAppContext db;

        public UserController(SocialAppContext db)
        {
            this.db = db;
        }

        public ActionResult Show(int id)
        {
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        [HttpPost]
        public ActionResult Update(UserUpdateModel model)
        {
            User currentUser = db.Users.Find(CurrentUserId);
            if (model.Picture != null)
            {
                string extension = System.IO.Path.GetExtension(model.Picture.FileName);
                string relativePicturePath = string.Format("{0}/{1}{2}", Strings.ProfilePicturesFolder, GenerateFileName(), extension);
                string serverPicturePath = Server.MapPath(string.Format("~/{0}", relativePicturePath));
                model.Picture.SaveAs(serverPicturePath);
                currentUser.PictureFilePath = relativePicturePath;
            }
            Mapper.Map(model, currentUser);
            db.SaveChanges();

            return RedirectToAction("Settings");
        }

        public ViewResult Settings()
        {
            User currentUser = db.Users.Find(CurrentUserId);
            var model = Mapper.Map<User, UserUpdateModel>(currentUser);
            return View(model);
        }

        public JsonCamelCaseResult UploadedSongs()
        {
            IEnumerable<Song> songs = db.Songs
                .Include(song => song.Tags)
                .Where(song => song.UploaderId == CurrentUserId)
                .ToList();
            return new JsonCamelCaseResult(songs, JsonRequestBehavior.AllowGet);

        }

        public JsonCamelCaseResult Current()
        {
            return new JsonCamelCaseResult(db.Users.Find(CurrentUserId), JsonRequestBehavior.AllowGet);
        }

        public ViewResult Library()
        {
            return View();
        }
    }
}
