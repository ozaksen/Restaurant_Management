using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QrMenuAgain.Models;
using QRMenuBackEnd.Models;
using SignalRWaitStaff.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QrMenuAgain.Controllers
{
    [Route("api/[controller]")]
    public class WaiterController : Controller
    {
        private readonly QrMenuContext _context;
        private readonly IHubContext<BusboyHub> _hubContextBusboy;

        public WaiterController(QrMenuContext context, IHubContext<BusboyHub> hubContextBusboy)
        {
                _context = context;
            _hubContextBusboy = hubContextBusboy;

        }

        // GET: api/Breaks

        [HttpPost("submittedOrder/{tableIndex}/{index}")]
        public async Task<ActionResult<Orders>> SubmitOrder(int tableNumber, int index)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            emp.TableIds[index].getList()[index].isSubmitted = true;
            return Ok(emp.TableIds[index].getList()[index]);
        }
        [HttpPost("removeOrder/{tableIndex}/{index}")]
        public async Task<ActionResult<Orders>> RemoveOrder(int tableNumber, int index)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            emp.TableIds[index].getList().Remove(emp.TableIds[index].getList()[index]);
            return Ok(emp.TableIds[index].getList());
        }

        // GET: api/Breaks/5
        [HttpGet("Breaks/{id}")]
        public async Task<ActionResult<Breaks>> GetBreaks(long id)
        {
            var breaks = await _context.Breaks.FindAsync(id);

            if (breaks == null)
            {
                return NotFound();
            }

            return breaks;
        }

        // PUT: api/Breaks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Breaks/{id}")]
        public async Task<IActionResult> PutBreaks(long id, Breaks breaks)
        {
            if (id != breaks.Id)
            {
                return BadRequest();
            }

            _context.Entry(breaks).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BreaksExists(id))
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
        [HttpPost("AssignTables/{tableNumber}/{waiterId}")]
        public async Task<ActionResult> AssignTables(int tableNumber, long waiterId)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            var waiter = _context.Employees.Where(x => x.RestaurantId == emp.RestaurantId && x.Id == waiterId).FirstOrDefault();
            if (waiter == null)
            {
                return NotFound("Waiter Not Found");
            }
            var table = _context.Table.Where(x => x.TableNumber == tableNumber && x.RestaurantId == emp.RestaurantId).FirstOrDefault();
            if (table == null)
            {
                return NotFound("Table Not Found");
            }

            if (waiter.TableIds == null)
            {
                waiter.TableIds = new List<Tables>();
            }

            table.AssignedEmployeeId = waiter.Id;
            waiter.TableIds.Add(table);
            await _context.SaveChangesAsync();
            return Ok(table);
        }

        [HttpGet("AssignedTables")]
        public async Task<ActionResult<IEnumerable<Tables>>> GetAssignedTables()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return _context.Table.Where(x=> x.RestaurantId == emp.RestaurantId && emp.Id == x.AssignedEmployeeId).ToList();
        }
        [HttpGet("RestaurantTables")]
        public async Task<ActionResult<IEnumerable<Tables>>> GetTables1()
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            return _context.Table.Where(x => x.RestaurantId == emp.RestaurantId).ToList();
        }
        // POST: api/Breaks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Breaks")]
        public async Task<ActionResult<Breaks>> PostShifts([FromBody] Breaks breaks)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            breaks.RestaurantId = emp.RestaurantId;
            _context.Breaks.Add(breaks);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetShifts", new { id = breaks.Id }, breaks);
        }

        // DELETE: api/Breaks/5
        [HttpDelete("Breaks/{id}")]
        public async Task<IActionResult> DeleteBreaks(long id)
        {
            var breaks = await _context.Breaks.FindAsync(id);
            if (breaks == null)
            {
                return NotFound();
            }

            _context.Breaks.Remove(breaks);
            await _context.SaveChangesAsync();

            return NoContent();
        }




        // GET: api/Breaks/5

        // PUT: api/Breaks/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("Shifts/{id}")]
        public async Task<IActionResult> PutShifts(long id, Shifts shifts)
        {

            _context.Entry(shifts).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ShiftsExists(id))
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

        // POST: api/Breaks
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("Shifts")]
        public async Task<ActionResult<Shifts>> PostShifts([FromBody] Shifts shifts)
        {
            var req = Request;
            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            shifts.RestaurantId = emp.RestaurantId;
            _context.Shifts.Add(shifts);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Shifts), new { id = shifts.Id }, shifts);
        }

        // DELETE: api/Breaks/5
        [HttpDelete("Shifts/{id}")]
        public async Task<IActionResult> DeleteShifts(long id)
        {
            var shifts = await _context.Shifts.FindAsync(id);
            if (shifts == null)
            {
                return NotFound();
            }

            _context.Shifts.Remove(shifts);
            await _context.SaveChangesAsync();

            return NoContent();
        }



        [HttpPost]
        [Route("Busboy/{id}")]
        public async Task<ActionResult<CustomerRequest>> PostBusboy(long id)
        {
            CustomerRequest help = _context.CustomerRequest.Where(x => x.id == id).FirstOrDefault();

            if (help == null)
            {
                return NotFound();
            }
            help.isSentToBusboy = true;
            await _context.SaveChangesAsync();

            await _hubContextBusboy.Clients.All.SendAsync("ReceiveRequest", JsonConvert.SerializeObject(help));

            return CreatedAtAction(nameof(CustomerRequest), new { id = help.id }, help);
        }

        [HttpGet]
        [Route("getBusboyReqs")]
        public async Task<string> getBusboyReqs()
        {
            List<CustomerRequest> help = _context.CustomerRequest.Where(x => x.isSentToBusboy == true).ToList();

            if (help == null)
            {
                return "Not Found";
            }


            //await _hubContextBusboy.Clients.All.SendAsync("ReceiveRequest", JsonConvert.SerializeObject(_context.CustomerRequest.ToList()));

            return JsonConvert.SerializeObject(help);
        }

        // PUT: api/CustomerRequests/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomerRequest(long id, CustomerRequest customerRequest)
        {
            if (id != customerRequest.id)
            {
                return BadRequest();
            }

            _context.Entry(customerRequest).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerRequestExists(id))
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
        // GET: api/CustomerRequests
        [HttpGet]
        [Route("TriggeredAction")]
        public async Task<ActionResult<IEnumerable<CustomerRequest>>> GetCustomerRequest()
        {
            return await _context.CustomerRequest.ToListAsync();
        }

        // GET: api/CustomerRequests/5
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerRequest>> GetCustomerRequest(long id)
        {
            var customerRequest = await _context.CustomerRequest.FindAsync(id);

            if (customerRequest == null)
            {
                return NotFound();
            }

            return customerRequest;
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomerRequest(long id)
        {
            var customerRequest = await _context.CustomerRequest.FindAsync(id);
            if (customerRequest == null)
            {
                return NotFound();
            }
            var req = Request;

            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }

            WaitStaffActionStats a = new WaitStaffActionStats
            {
                tableNumber = customerRequest.tableId,
                request = customerRequest.reason,
                CompleteTime = DateTime.Now,
                EmployeeName = emp.getFullName()
            };
            _context.WaitStaffActionStats.Add(a);
            _context.CustomerRequest.Remove(customerRequest);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(WaitStaffActionStats), new { id = a.Id }, a);

        }

        [HttpGet("Orders/{tableNumber}")]
        public async Task<string> GetOrders(string sid)
        {
            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();
            if (table == null)
            {
                return "Table Not Found";
            }

            return JsonConvert.SerializeObject(table.getList());
        }

        [HttpGet("OrdersTableNum/{tbNum}")]
        public async Task<ActionResult> GetOrders(int tbNum)
        {
            var req = Request;

            string token = req.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var emp = _context.Employees.Where(x => x.Token == token).FirstOrDefault();
            if (emp == null)
            {
                return Unauthorized("Token mismatch!");
            }
            
            var table = _context.Table.Where(x => x.RestaurantId == 1 && x.TableNumber == tbNum).FirstOrDefault();
            if (table == null)
            {
                return NotFound("Table Not Found");
            }

            return Ok(table.getList());
        }


        private bool BreaksExists(long id)
        {
            return _context.Breaks.Any(e => e.Id == id);
        }
        private bool ShiftsExists(long id)
        {
            return _context.Shifts.Any(e => e.Id == id);
        }

        private bool CustomerRequestExists(long id)
        {
            return _context.CustomerRequest.Any(e => e.id == id);
        }
    }
}
