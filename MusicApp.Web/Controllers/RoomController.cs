using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Business.Services;
using Data.Domain;
using DataAccess;
using System.Data.Entity;
using SocialApp.Models;

namespace SocialApp.Controllers
{
    [Authorize]
    public class RoomController : BaseController
    {
        private readonly IRoomService roomService;
        private readonly IUserService userService;
        private readonly IMailService mailService;

        public RoomController(IRoomService roomService, IUserService userService, IMailService mailService)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.mailService = mailService;
        }

        public ViewResult List()
        {
            var rooms = roomService.GetAllRoomsWithHosts();
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
            roomService.HostRoom(userId: CurrentUserId, room: room);
            return RedirectToAction("Details", "Room", new { room.Id });
        }

        public ActionResult Details(int id)
        {
            Room room = roomService.FindById(id);
            if (room == null)
            {
                return RedirectToAction("List");
            }
            var model = new RoomDetailsViewModel
            {
                Room = room,
                CurrentUser = userService.FindUserById(CurrentUserId)
            };
            model.CurrentUserIsHost = model.Room == model.CurrentUser.HostedRoom;
            return View(model);
        }

        public RedirectToRouteResult Destroyed(int id)
        {
            TempData["danger"] = "Room has been destroyed by its host";
            return RedirectToAction("List");
        }

        [HttpGet]
        public JsonResult InviteUserToRoom(int senderId, int receiverId, int roomId)
        {
            string text = string.Format("I invite you to join my room. Here is the {0}", 
                Url.Action("Details", new { id = roomId }));
            mailService.AddMessage(senderId, receiverId, text);
            return Json(new {success = true}, JsonRequestBehavior.AllowGet);
        }

    }
}
