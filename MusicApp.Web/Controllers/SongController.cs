using System;
using System.IO;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
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
        public JsonCamelCaseResult Upload()
        {
            byte[] songBytes = FileUtils.ReadBytesFromStream(Request.InputStream);
            string fileName = string.Format("{0}.mp3", GenerateFileName());
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

            using (TagLib.File tagFile = TagLib.File.Create(Server.MapPath("~" + path)))
            {
                song.Bitrate = tagFile.Properties.AudioBitrate;
                song.Duration = tagFile.Properties.Duration;
                song.Title = tagFile.Tag.Title ?? "Song Title";
                song.Artist = tagFile.Tag.FirstAlbumArtist ?? tagFile.Tag.FirstArtist ?? "Song Artist";
                song.Album = tagFile.Tag.Album ?? "Song Album Title";
                if (tagFile.Tag.Pictures.FirstOrDefault() != null) // has album cover
                {
                    IPicture pic = tagFile.Tag.Pictures.First();
                    string extension = pic.MimeType.Substring(pic.MimeType.LastIndexOf('/') + 1);
                    string picFileName = string.Format("{0}.{1}", GenerateFileName(), extension);
                    string picPath = string.Format("{0}{1}", AlbumCoverDirectory, picFileName);
                    System.IO.File.WriteAllBytes(Server.MapPath("~" + picPath), pic.Data.ToArray());
                    song.AlbumCoverPicturePath = picPath;
                }
                else // use default album cover picture
                {
                    song.AlbumCoverPicturePath = WebConfigurationManager.AppSettings["DefaultAlbumCoverPicturePath"];
                }
            }

            db.Songs.Add(song);
            db.SaveChanges();

            return new JsonCamelCaseResult(song, JsonRequestBehavior.DenyGet);
        }

    }
}
