using AutoMapper;
using ListWatch.Models;
using ListWatch.DTOs;
namespace ListWatch.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            // CreateMap<User, UserDto>();
            //CreateMap<CreateUserDto, User>();

            // WatchListItem mappings
            CreateMap<WatchListItems, WatchListItemDto>()
                .ForMember(dest => dest.CompletedEpisodes, opt => opt.MapFrom(src => (int?)src.WatchProgress!.CompletedEpisodes))
                .ForMember(dest => dest.TotalEpisodes, opt => opt.MapFrom(src => (int?)src.WatchProgress!.TotalEpisodes))
                .ReverseMap();



            CreateMap<CreateWatchListItemDto, WatchListItems>();
            CreateMap<UpdateWatchListItemDto, WatchListItems>();
        }
    }
}
