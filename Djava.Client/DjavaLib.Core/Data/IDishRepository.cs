using DjavaLib.Models;
using System.Collections.Generic;

namespace DjavaLib.Data
{
    public interface IDishRepository
    {
        List<Dish> GetAllAvailableDishes();
        List<Category> GetAllCategories();
        List<Dish> GetDishesByCategory(int categoryId);
        Dish GetDishById(int id);
    }
}