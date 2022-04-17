using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QrMenuAgain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QRMenuBackEnd.Models;
using Microsoft.EntityFrameworkCore;

namespace QrMenuAgain.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostController : ControllerBase
    {
        private readonly QrMenuContext _context;
        private Dictionary<long, List<WaitList>> lineD = new Dictionary<long, List<WaitList>>();
        private List<WaitList> line = new List<WaitList>();
        public HostController(QrMenuContext context)
        {
            _context = context;
        }
        [HttpPost("WaitList")]
        public async Task<ActionResult> PostPeople2([FromBody] WaitList newCustomer)
        {
            
            newCustomer.EnterDate = DateTime.Now;
            line.Add(newCustomer);
            return Ok();
        }
        [HttpGet("Waitlist")]
        public async Task<ActionResult<IEnumerable<WaitList>>> GetPeople2()
        {
            return line;
        }
        [HttpGet("WaitStaffActionStats2")]
        public async Task<ActionResult<IEnumerable<WaitStaffActionStats>>> GetWaitStaffActions()
        {
            return await _context.WaitStaffActionStats.ToListAsync();
        }
        [HttpPost("WaitListRes")]
        public async Task<ActionResult> PostPeople([FromBody]WaitList newCustomer)
        {
            var req = Request;

            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            newCustomer.EnterDate = DateTime.Now;
            if (lineD.ContainsKey(emp.RestaurantId))
            {
                lineD[emp.RestaurantId].Add(newCustomer);
            }
            else {
                lineD.Add(emp.RestaurantId, new List<WaitList>() { newCustomer });
            }
            return Ok();
        }
        [HttpGet("WaitlistRes")]
        public async Task<ActionResult<IEnumerable<WaitList>>> GetPeople()
        {
            var req = Request;

            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            if (lineD.ContainsKey(emp.RestaurantId))
            {
                return lineD[emp.RestaurantId];
            }
            else
            {
                lineD.Add(emp.RestaurantId, new List<WaitList>() { });
                return lineD[emp.RestaurantId];
            }
        }
        [HttpGet("EndSession/{id}")]
        public async Task<ActionResult> ResetSession(long id)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            var tables = _context.Table.Where(x => x.RestaurantId == emp.RestaurantId && x.TableNumber == id).FirstOrDefault();

            if (tables == null)
            {
                return NotFound("Table Not Found");
            }
            if (tables.IsOccupied == true)
            {
                tables.TableSid = "";
                tables.IsOccupied = false;
            }
            else
            {
                return Forbid("Table has no session");
            }
            TableStatistics stat = _context.TableStatistics.Where(x => x.TableNumber == id && x.FinishTime == null && x.RestaurantId == emp.RestaurantId).FirstOrDefault();
            if (stat == null)
            {
                return NotFound("Table is not in statistics");
            }
            stat.FinishTime = DateTime.Now;
            var hours = stat.FinishTime - stat.StartTime;
            stat.timeSpent = hours.ToString();

            if (tables.getList() != null)
            {
                tables.clearList();
            }
            // stat.Server = EmpName;
            await _context.SaveChangesAsync();

            return Ok(tables);
        }
        [HttpGet("startSession/{id}")]
        public async Task<ActionResult> GetSession(int id)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            var tables = _context.Table.Where(x => x.RestaurantId == emp.RestaurantId && x.TableNumber == id).FirstOrDefault();

            if (tables == null)
            {
                return NotFound("Table Not Found");
            }
            if (tables.IsOccupied == false)
            {
                tables.TableSid = Guid.NewGuid().ToString();
                tables.IsOccupied = true;
            }
            else
            {
                return Forbid("Table is already started a session");
            }
            var newCustomer = new TableStatistics()
            {
                StartTime = DateTime.Now,
                FinishTime = null,
                TableNumber = id,
                RestaurantId = emp.RestaurantId
            };

            _context.TableStatistics.Add(newCustomer);
            await _context.SaveChangesAsync();

            //newCustomer.Server =
            return Ok(tables);
        }
        


        // GET: api/Tables
        [HttpGet("Tables")]
        public async Task<ActionResult<IEnumerable<Tables>>> GetTable()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            return await _context.Table.Where(x => x.RestaurantId == emp.RestaurantId).ToListAsync();
        }

        // GET: api/Tables/5
        [HttpGet("Tables/{id}")]
        public async Task<ActionResult<Tables>> GetTables(long id)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            var tables =  _context.Table.Where(x => x.RestaurantId == emp.RestaurantId && x.TableNumber == id).FirstOrDefault(); 

            if (tables == null)
            {
                return NotFound();
            }

            return tables;
        }

        [HttpGet]
        [Route("ActiveTheme")]
        public async Task<string> GetActiveTheme()
        {
            return JsonConvert.SerializeObject(Base.StaticThemes.DataList.Where(x => x.isDefault).ToList()); ;
        }
    }
}
