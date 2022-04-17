using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QrMenuAgain.BASE;
using QrMenuAgain.Controllers.IdentityBase;
using QrMenuAgain.Models;
using QrMenuAgain.Services;

namespace QrMenuAgain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly QrMenuContext _context;
        private readonly List<Employees> EmployeeList = new List<Employees>();
        private ICurrentUser currentUser;

        public EmployeesController(QrMenuContext context, ICurrentUser _currentUser)
        {
            _context = context;
    

            currentUser = _currentUser;
        }

        // GET: api/Employees
        [Authorize(Roles = "Management")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Employees>>> GetEmployees()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return _context.Employees.Where(x => x.RestaurantId == emp.RestaurantId).ToList();
        }

        // GET: api/Employees/5
        [Authorize(Roles = "Management")]
        [HttpGet("{id}")]
        public async Task<ActionResult<Employees>> GetEmployees(long id)
        {
            var employees = await _context.Employees.FindAsync(id);

            if (employees == null)
            {
                return NotFound();
            }

            return employees;
        }

        // PUT: api/Employees/5
        // [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmployees(long id, Employees employees)
        {
            if (id != employees.Id)
            {
                return BadRequest();
            }

            _context.Entry(employees).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmployeesExists(id))
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

        [HttpPost("register")]
        public async Task<ActionResult<dynamic>> register([FromBody] Employees model)
        {
            var check = _context.Employees.Where(x => x.Email == model.Email).FirstOrDefault();
            //var check = EmployeeList.Where(x => x.Email == model.Email).FirstOrDefault();
            if (check == null) {
                _context.Employees.Add(model);
                await _context.SaveChangesAsync();
                return CreatedAtAction(model.FirstName, model);
            }
            return NotFound("There's an account linked with this email");
        }

        [HttpPost("signout")]
        public async Task<ActionResult<dynamic>> signout([FromBody] Employees model)
        {
            await currentUser.SignOut();

            return NotFound("There's an account linked with this email");
        }
        [HttpPost("login")]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] Employees model)
        {
            var employee = _context.Employees.Where(x => x.Email == model.Email && x.Password == model.Password).FirstOrDefault();
            //var employee = EmployeeList.Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password) && model.Role.Equals(x.Role)).FirstOrDefault();
            //var employee = EmployeeList.Where(x => x.Email.Equals(model.Email) && x.Password.Equals(model.Password)).FirstOrDefault();
            if (employee == null)
                return NotFound(new { message = "User or password invalid" });




            var _login = currentUser.SignIn(employee, employee.Password, employee.Role).Result;
            employee.Token = _login.ResultObj != null ? _login.ResultObj.Token : employee.Token;
            await _context.SaveChangesAsync();

            return new
            {
                email = employee.Email,
                Name = employee.FirstName + " " + employee.LastName,
                token = _login.ResultObj != null ? _login.ResultObj.Token : employee.Token,
                Role = employee.Role,
                Tables = employee.Role.Equals("Waiter") || employee.Role.Equals("Busboy") ? employee.TableIds : null
            };
        }
        [HttpPost]
        [Route("loginGoogleAuth")]
        public async Task<ActionResult<dynamic>> AuthenticateGoogleAuth([FromBody] Employees model)
        {
            //var employee = _context.Employees.Where(x => x.Email == model.Email && x.Password == model.Password).First();
            var employee = EmployeeList.Where(x => x.Email == model.Email).FirstOrDefault();
            if (employee == null)
                return NotFound(new { message = "User or password invalid" });

            var token = TokenService.CreateToken(employee);
            employee.Token = token;
            employee.Password = "";
            return Ok("Email Found");
        }
        // POST: api/Employees
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost("Register")]
        //[AllowAnonymous]
        //public async Task<ActionResult<Employees>> PostEmployees(Employees employees)
        //{
        //    _context.Employees.Add(employees);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetEmployees", new { id = employees.Id }, employees);
        //}

        // DELETE: api/Employees/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployees(long id)
        {
            var employees = await _context.Employees.FindAsync(id);
            if (employees == null)
            {
                return NotFound();
            }

            _context.Employees.Remove(employees);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmployeesExists(long id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
    }
}
