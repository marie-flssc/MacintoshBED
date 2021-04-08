using AutoMapper;
using MacintoshBED.DTO;
using MacintoshBED.Models;

namespace MacintoshBED.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserModel>();
            CreateMap<RegisterModel, User>();
            CreateMap<UpdateModel, User>();
        }
    }
}
