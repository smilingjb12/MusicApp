using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Domain;
using DataAccess;
using System.Data.Entity;
using SocialApp.Models;

namespace SocialApp.Controllers
{
    [Authorize]
    public class RoomController : BaseController
    {
        private readonly SocialAppContext db;

        public RoomController(SocialAppContext db)
        {
            this.db = db;
        }

        public ViewResult List()
        {
            var rooms = db.Rooms.Include(r => r.Host).ToList();
            return View(rooms);
        }

        public ActionResult Create()
        {
            User user = db.Users
                .Include(u => u.HostedRoom)
                .FirstOrDefault(u => u.Id == CurrentUserId);
            if (user.HostedRoom != null)
            {
                TempData["danger"] = "You must destroy your currently hosted room to create a new one";
                return RedirectToAction("Details", new { user.HostedRoom.Id });
            }
            return View();
        }

        [HttpPost]
        public ActionResult Create(Room room)
        {
            if (!ModelState.IsValid)
            {
                return View(room);
            }
            User currentUser = db.Users
                .Include(u => u.HostedRoom)
                .FirstOrDefault(u => u.Id == CurrentUserId);
            currentUser.HostedRoom = room;
            db.SaveChanges();

            return RedirectToAction("Details", "Room", new { room.Id });
        }

        public ViewResult Details(int id)
        {
            var model = new RoomDetailsViewModel()
            {
                Room = db.Rooms.Find(id),
                CurrentUser = db.Users.Find(CurrentUserId),
            };
            model.CurrentUserIsHost = model.Room == model.CurrentUser.HostedRoom;
            return View(model);
        }

    }
}
