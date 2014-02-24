using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Data;
using Data.Domain;
using SocialApp.Models;

namespace SocialApp.App_Start
{
    public static class AutoMapperConfig
    {
        public static void RegisterMappings()
        {
            Mapper.CreateMap<UserUpdateViewModel, User>()
                .ForMember(u => u.PictureFilePath, opt => opt.Ignore());

            Mapper.CreateMap<User, UserUpdateViewModel>();

            Mapper.CreateMap<Id3Info, Song>();
        }
    }
}