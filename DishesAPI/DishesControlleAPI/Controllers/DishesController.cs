using DishesControlleAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DishesControlleAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DishesController : ControllerBase
    {
        private readonly DishesApiContext dishesApiContext;

        public DishesController(DishesApiContext dishesApiContext)
        {
            this.dishesApiContext = dishesApiContext;
            
        }

        [HttpGet] 
        public async Task<ActionResult<List<Dish>>> Dishes()
        {
            return Ok(await dishesApiContext.Dishes.ToListAsync());
        }
        
    }
}
