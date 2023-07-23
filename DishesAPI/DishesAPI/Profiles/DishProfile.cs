using AutoMapper;
using DishesAPI.Entities;
using DishesAPI.Models;

namespace DishesAPI.Profiles
{
    public class DishProfile : Profile
    {
        public DishProfile() {
            //src target
            CreateMap<Entities.Dish, DishDto>();
            CreateMap<Entities.Dish, DishForCreationDto>();
            CreateMap<DishForCreationDto, Entities.Dish>();

            CreateMap<DishForUpdateDto, Dish>();

        }

    }
}
