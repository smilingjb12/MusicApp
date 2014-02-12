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

        public ActionResult Details(int id)
        {
            Room room = db.Rooms.Find(id);
            if (room == null)
            {
                return RedirectToAction("List");
            }
            var model = new RoomDetailsViewModel
            {
                Room = room,
                CurrentUser = db.Users.Find(CurrentUserId),
            };
            model.CurrentUserIsHost = model.Room == model.CurrentUser.HostedRoom;
            return View(model);
        }

        public RedirectToRouteResult Destroyed(int id)
        {
            TempData["danger"] = "Room has been destroyed by its host";
            return RedirectToAction("List");
        }

    }
}
