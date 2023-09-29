using AutoMapper;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dto);
        DishDto GetById(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _context = dbContext;
            _mapper = mapper;
        }
        public int Create(int restaurantId, CreateDishDto dto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dishEntity = _mapper.Map<Dish>(dto);
            dishEntity.RestaurantId = restaurantId;
            _context.Dishes.Add(dishEntity);

            _context.SaveChanges();

            return dishEntity.Id;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            //var restaurant = _context.Restaurants.FirstOrDefault(r => r.Id == restaurantId);
            //if (restaurant == null)
            //    throw new NotFoundException($"Restaurant with id: {restaurantId} was not found");

            //return _mapper.Map<List<DishDto>>(restaurant.Dishes);
            var restaurant = GetRestaurantById(restaurantId);
                
            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);
            //var dish = _context.Dishes.FirstOrDefault(d => d.Id == dishId);
            //if (dish.RestaurantId != restaurantId)
            //    throw new NotFoundException($"Dish with id: {restaurantId} was not found");

            if (dish is null)
                throw new NotFoundException($"Dish with id: {restaurantId} was not found");

            var dishDto = _mapper.Map<DishDto>(dish);
            return dishDto;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == restaurantId);
            if (restaurant == null)
                throw new NotFoundException($"Restaurant with id: {restaurantId} was not found");

            return restaurant;
        }
    }

}
