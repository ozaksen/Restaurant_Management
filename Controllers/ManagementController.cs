using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QrMenuAgain.Models;
using QrMenuAgain.Services;
using QRMenuBackEnd.Models;

namespace QrMenuAgain.Controllers
{
    [Authorize(Roles = "Management")]
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly QrMenuContext _context;
        public ManagementController(QrMenuContext context)
        {
            _context = context;
        }



        [HttpGet("WaitStaffActionStats")]
        public async Task<ActionResult<IEnumerable<WaitStaffActionStats>>> GetWaitStaffActions()
        {
            return await _context.WaitStaffActionStats.ToListAsync();
        }
        // GET: api/Menus
        [HttpGet("Menus")]
        public async Task<ActionResult<IEnumerable<Menus>>> GetMenus()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return await _context.Menus.Where(x => x.RestaurantId == emp.RestaurantId).ToListAsync();
        }

        // GET: api/Breaks
        [HttpGet("Shifts")]
        public async Task<ActionResult<IEnumerable<Shifts>>> GetShifts()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return await _context.Shifts.Where(x => x.RestaurantId == emp.RestaurantId).ToListAsync();
        }

        [HttpGet("Breaks")]
        public async Task<ActionResult<IEnumerable<Breaks>>> GetBreaks()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return await _context.Breaks.Where(x => x.RestaurantId == emp.RestaurantId).ToListAsync();
        }
        // GET: api/Menus/5
        [HttpGet("Menus/{id}")]
        public async Task<ActionResult<Menus>> GetMenus(long id)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            
            var menus = await _context.Menus.FindAsync(id);

            if (menus == null)
            {
                return NotFound();
            }
            if (menus.RestaurantId != emp.RestaurantId) {
                return Unauthorized();
            }
            return menus;
        }







        [HttpGet("GetCategories")]
        public async Task<List<String>> GetCategories()
        {
            return await _context.FoodItem.Select(x => x.Category).Distinct().ToListAsync();
        }
        // PUT: api/Menus/5
        [HttpPut("Menus/{id}")]
        public async Task<IActionResult> PutMenus(long id, Menus menus)
        {
            if (id != menus.Id)
            {
                return BadRequest();
            }
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            menus.RestaurantId = emp.RestaurantId;
            _context.Entry(menus).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MenusExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Menus
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Menus")]
        public async Task<ActionResult<Menus>> PostMenus(Menus menus)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            menus.RestaurantId = emp.RestaurantId;
            _context.Menus.Add(menus);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMenus", new { id = menus.Id }, menus);
        }

        // DELETE: api/Menus/5
        [HttpDelete("Menus/{id}")]
        public async Task<IActionResult> DeleteMenus(long id)
        {
            var menus = await _context.Menus.FindAsync(id);
            if (menus == null)
            {
                return NotFound();
            }

            _context.Menus.Remove(menus);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        // GET: api/FoodItems
        [HttpGet("FoodItems")]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItem()
        {
            return await _context.FoodItem.ToListAsync();
        }

        // GET: api/FoodItems/5
        [HttpGet("FoodItems/{id}")]
        public async Task<ActionResult<IEnumerable<FoodItem>>> GetFoodItem(int id)
        {
            var foodItem = _context.FoodItem.Where(x => x.MenuId == id).ToList();

            if (foodItem == null)
            {
                return NotFound();
            }

            return foodItem;
        }
        [HttpPut("UpdateIsActive/{id}")]
        public async Task<IActionResult> UpdateIsActive(long id)
        {
            var FoodItem = _context.FoodItem.Where(x => x.Id == id).FirstOrDefault();

            FoodItem.IsActive = !FoodItem.IsActive;

            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("registerStaff")]
        public async Task<ActionResult<dynamic>> register([FromBody] Employees model)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            

            var check = _context.Employees.Where(x => x.Email == model.Email).FirstOrDefault();
            if (check == null)
            {
                model.RestaurantId = emp.RestaurantId;
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(model.FirstName, model);
            }
            return NotFound("There's an account linked with this email");
        }


        [HttpPost("PostRestaurantInfo")]
        public async Task<ActionResult> newRestaurant(Restaurant restaurant)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            _context.Restaurant.Add(restaurant);
            await _context.SaveChangesAsync();

            emp.RestaurantId = restaurant.Id;
            for (int i = 1; i <= restaurant.TableCount; i++)
            {
                Tables tables = new Tables();
                tables.TableNumber = i;
                tables.RestaurantId = restaurant.Id;
                _context.Table.Add(tables);
                await _context.SaveChangesAsync();
            }
            await _context.SaveChangesAsync();

            return CreatedAtAction(restaurant.RestaurantName, new { id = restaurant.Id }, restaurant);
        }
        [HttpGet("GetRestaurantInfo")]
        public async Task<ActionResult> GetRestaurant()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            
            return Ok(_context.Restaurant.Where(x => x.Id == emp.RestaurantId).FirstOrDefault());
        }
        [HttpPut("PutRestaurantInfo")]
        public async Task<ActionResult> PutRestaurant(Restaurant restaurant)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }


            _context.Entry(restaurant).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RestaurantExists(emp.RestaurantId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        [HttpGet("Themes")]
        public async Task<string> GetThemes()
        {
            return JsonConvert.SerializeObject(Base.StaticThemes.DataList);
        }
        [HttpPost]
        [Route("SetActiveTheme/{id}")]
        public async Task<string> GetActiveTheme2(long id)
        {
            var data = Base.StaticThemes.DataList.Where(x => x.isDefault).FirstOrDefault();
            data.isDefault = false;
            data = Base.StaticThemes.DataList.Where(x => x.Id == id).First();
            if (data == null)
            {
                return "Id not found";
            }
            data.isDefault = true;
            return JsonConvert.SerializeObject(data);
        }

        [HttpGet]
        [Route("ActiveTheme")]
        public async Task<string> GetActiveTheme()
        {
            return JsonConvert.SerializeObject(Base.StaticThemes.DataList.Where(x => x.isDefault).ToList()); ;
        }


        // PUT: api/FoodItems/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("FoodItems/{id}")]
        public async Task<IActionResult> PutFoodItem(long id, [FromBody] FoodItem food)
        {
            if (id != food.Id)
            {
                return BadRequest("Id does not match");
            }

            var fooditem = await _context.FoodItem.FindAsync(id);
            if (fooditem == null)
            {
                return NotFound("Item not found");
            }

            fooditem.Description = food.Description;
            fooditem.Category = food.Category;
            fooditem.Name = food.Name;
            fooditem.Price = food.Price;
            _context.Update(fooditem);
            await _context.SaveChangesAsync();

            return Ok();
        }

        // POST: api/FoodItems
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("FoodItems")]
        public async Task<ActionResult<FoodItem>> PostFoodItem(FoodItem foodItem)
        {
            _context.FoodItem.Add(foodItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFoodItem", new { id = foodItem.Id }, foodItem);
        }

        // DELETE: api/FoodItems/5
        [HttpDelete("FoodItems/{id}")]
        public async Task<IActionResult> DeleteFoodItem(long id)
        {
            var foodItem = await _context.FoodItem.FindAsync(id);
            if (foodItem == null)
            {
                return NotFound();
            }

            _context.FoodItem.Remove(foodItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MenusExists(long id)
        {
            return _context.Menus.Any(e => e.Id == id);
        }
        // GET: api/UserReviews
        [HttpGet("UserReviews")]
        public async Task<ActionResult<IEnumerable<UserReviews>>> GetUserReviews()
        {
            return await _context.UserReviews.ToListAsync();
        }

        // GET: api/UserReviews/5
        [HttpGet("UserReviews/{id}")]
        public async Task<ActionResult<UserReviews>> GetUserReviews(long id)
        {
            var userReviews = await _context.UserReviews.FindAsync(id);

            if (userReviews == null)
            {
                return NotFound();
            }

            return userReviews;
        }

        // PUT: api/UserReviews/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UserReviews/{id}")]
        public async Task<IActionResult> PutUserReviews(long id, UserReviews userReviews)
        {
            if (id != userReviews.Id)
            {
                return BadRequest();
            }

            _context.Entry(userReviews).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserReviewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }



        // DELETE: api/UserReviews/5
        [HttpDelete("UserReviews/{id}")]
        public async Task<IActionResult> DeleteUserReviews(long id)
        {
            var userReviews = await _context.UserReviews.FindAsync(id);
            if (userReviews == null)
            {
                return NotFound();
            }

            _context.UserReviews.Remove(userReviews);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost]
        [Route("TableCount/{count}")]
        public async Task<ActionResult<IEnumerable<Tables>>> CreateTablesWithCount(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                Tables tables = new Tables();
                tables.TableNumber = i;
                tables.id = i;
                _context.Table.Add(tables);
                await _context.SaveChangesAsync();
            }

            return await _context.Table.ToListAsync();
        }
        // DELETE: api/Tables/5
        [HttpDelete("Tables/{id}")]
        public async Task<IActionResult> DeleteTables(long id)
        {
            var tables = await _context.Table.FindAsync(id);
            if (tables == null)
            {
                return NotFound();
            }

            _context.Table.Remove(tables);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet]
        [Route("Stats")]
        public async Task<ActionResult<IEnumerable<TableStatistics>>> GetStats()
        {
            return await _context.TableStatistics.ToListAsync();
        }
        private bool UserReviewsExists(long id)
        {
            return _context.UserReviews.Any(e => e.Id == id);
        }
        private bool RestaurantExists(long id)
        {
            return _context.Restaurant.Any(e => e.Id == id);
        }
    }
}
