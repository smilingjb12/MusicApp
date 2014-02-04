using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Data.Domain;
using DataAccess;
using System.Data.Entity;

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

        public ActionResult Delete(int id)
        {
            Room room = db.Rooms.Find(id);
            db.Rooms.Remove(room);
            db.SaveChanges();

            return RedirectToAction("List");
        }

        public ViewResult Details(int id)
        {
            Room room = db.Rooms.Find(id);
            return View(room);
        }

    }
}
