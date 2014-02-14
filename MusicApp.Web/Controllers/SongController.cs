using System;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using AutoMapper;
using Business;
using Business.Services;
using Data;
using Data.Domain;
using DataAccess;
using Ninject;
using SocialApp.Models;
using TagLib;
using System.Data.Entity;
using Tag = Data.Domain.Tag;

namespace SocialApp.Controllers
{
    public class SongController : BaseController
    {
        private const string SongDirectory = "/Content/Uploads/Songs/";
        private const string AlbumCoverDirectory = "/Content/Uploads/AlbumCovers/";

        private readonly SocialAppContext db;
        private readonly ITagService tagService;

        public SongController(ITagService tagService, SocialAppContext db)
        {
            this.tagService = tagService;
            this.db = db;
        }

        [HttpPost]
        public JsonResult Update(Song song)
        {
            Song existingSong = db.Songs
                .Include(s => s.Tags)
                .FirstOrDefault(s => s.Id == song.Id);
            if (existingSong == null) return null;

            existingSong.Title = song.Title;
            existingSong.Artist = song.Artist;
            existingSong.Album = song.Album;
            existingSong.Tags = tagService
                .GetOrCreateTags(song.Tags.Select(t => t.Name))
                .ToList();

            db.SaveChanges();
            return Json(string.Empty);
        }

        public JsonResult Search(string term)
        {
            term = term.ToLower();
            var songs = db.Songs.Where(s => s.Artist.ToLower().Contains(term) || s.Title.ToLower().Contains(term));
            return Json(songs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            Song song = db.Songs.Find(id);
            db.Songs.Remove(song);
            db.SaveChanges();
            return Json(string.Empty);
        }

        public FileResult Download(int id)
        {
            Song song = db.Songs.Find(id);
            if (song == null) return null;
            string path = Server.MapPath("~" + song.FilePath);
            var fileStream = new FileStream(path, FileMode.Open);
            return File(fileStream, "audio/mp3");
        }

        [HttpPost]
        public JsonResult Upload()
        {
            byte[] songBytes = FileUtils.ReadBytesFromStream(Request.InputStream);
            string fileName = string.Format("{0}.mp3", FileUtils.GenerateFileName());
            string songServerDirectory = Server.MapPath("~" + SongDirectory);
            if (!Directory.Exists(songServerDirectory))
            {
                Directory.CreateDirectory(songServerDirectory);
            }
            string path = string.Format("{0}{1}", SongDirectory, fileName);
            System.IO.File.WriteAllBytes(Server.MapPath("~" + path), songBytes);

            Song song = new Song
            {
                UploaderId = CurrentUserId,
                FilePath = path,
                FileSizeInMegaBytes = Math.Round(Request.ContentLength / 1024d / 1024d, 2)
            };

            string albumCoverDirectory = Server.MapPath("~" + AlbumCoverDirectory);
            if (!Directory.Exists(albumCoverDirectory))
            {
                Directory.CreateDirectory(albumCoverDirectory);
            }

            IMp3Parser mp3Parser = new Mp3Parser();
            Id3Info id3Info = mp3Parser.ExtractId3Info(Server.MapPath("~" + path));
            Mapper.Map(id3Info, song);
            IPicture albumCover = id3Info.Picture;
            if (albumCover != null)
            {
                string extension = albumCover.MimeType.Substring(albumCover.MimeType.LastIndexOf('/') + 1);
                string picFileName = string.Format("{0}.{1}", FileUtils.GenerateFileName(), extension);
                string picPath = string.Format("{0}{1}", AlbumCoverDirectory, picFileName);
                System.IO.File.WriteAllBytes(Server.MapPath("~" + picPath), albumCover.Data.ToArray());
                song.AlbumCoverPicturePath = picPath;
            }
            else
            {
                song.AlbumCoverPicturePath = WebConfigurationManager.AppSettings["DefaultAlbumCoverPicturePath"];
            }

            db.Songs.Add(song);
            db.SaveChanges();

            return Json(song, JsonRequestBehavior.DenyGet);
        }

    }
}
