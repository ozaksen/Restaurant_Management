using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QrMenuAgain.Models;
using SignalRWaitStaff.Hubs;
using QrMenuAgain.BASE;
using System.IO;
using Stripe;
using Stripe.Checkout;

namespace QrMenuAgain.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class CustomerRequestsController : ControllerBase
    {
        private readonly QrMenuContext _context;
        private readonly IHubContext<WaitStaffHub> _hubContextWaitStaff;


        public CustomerRequestsController(QrMenuContext context, IHubContext<WaitStaffHub> hubContextWaitStaff)
        {
            _context = context;
            _hubContextWaitStaff = hubContextWaitStaff;

        }

  
        // GET: api/CustomerRequests/5
        [HttpGet]
        [Route("Actions")]
        public async Task<string> GetActions()
        {

            var customerActions = new List<CustomerActions>();
            customerActions.Add(new CustomerActions("call-waiter", "Call Waiter", "bell"));
            customerActions.Add(new CustomerActions("water-refill", "Water Refill", "tint"));
            customerActions.Add(new CustomerActions("more-utensils", "More Utensils", "utensils"));
            customerActions.Add(new CustomerActions("get-bill", "Get Bill", "money-check"));
            string a = JsonConvert.SerializeObject(customerActions);
            return a;
        }
        [HttpGet("Customer/Orders/{sid}")]
        public async Task<string> GetOrders(string sid)
        {
            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();
            if (table == null)
            {
                return "Table Not Found";
            }

            return JsonConvert.SerializeObject(table.getList());
        }
        //[HttpGet]
        //[Route("GenerateData")]
        //public async Task<string> GenerateData()
        //{
        //    try
        //    {

        //        _context.Restaurant.Add(new Restaurant
        //        {
        //            RestaurantName = "Jojo",
        //            Address = "610 Purdue Mall, West Lafayette, IN 47907",
        //            Cuisine = "American",
        //            Price = "Good",
        //            Description = "Kinda good Lorem ipsum bla bla",
        //            TableCount = 10
        //        });

        //        for (int i = 1; i <= 2; i++)
        //        {
        //            Tables tables = new Tables();
        //            tables.TableNumber = i;
        //            tables.RestaurantId = 1;
        //            tables.AssignedEmployeeId = 1;
        //            _context.Table.Add(tables);
        //            await _context.SaveChangesAsync();
        //        }
        //        for (int i = 3; i <= 5; i++)
        //        {
        //            Tables tables = new Tables();
        //            tables.TableNumber = i;
        //            tables.RestaurantId = 1;
        //            tables.AssignedEmployeeId =3;
        //            _context.Table.Add(tables);
        //            await _context.SaveChangesAsync();
        //        }
        //        for (int i = 6; i <= 10; i++)
        //        {
        //            Tables tables = new Tables();
        //            tables.TableNumber = i;
        //            tables.RestaurantId = 1;
        //            _context.Table.Add(tables);
        //            await _context.SaveChangesAsync();
        //        }
        //        _context.Employees.Add(new Employees
        //        {
        //            FirstName = "Jotaro",
        //            LastName = "Kujo",
        //            Email = "manager@qr.com",
        //            Password = "Password123!",
        //            Role = "Management",
        //            RestaurantId = 1
        //        });
        //        _context.Employees.Add(new Employees
        //        {
        //            FirstName = "Dio",
        //            LastName = "Brando",
        //            Email = "waiterDio@qr.com",
        //            Password = "Password123!",
        //            Role = "Waiter",
        //            TableIds = _context.Table.Where(x => x.id == 1 || x.id == 2 ).ToList(),
        //            RestaurantId = 1
        //        });
        //        _context.Employees.Add(new Employees
        //        {
        //            FirstName = "Joseph",
        //            LastName = "Joestar",
        //            Email = "waiterJoseph@qr.com",
        //            Password = "Password123!",
        //            Role = "Waiter",
        //            TableIds = _context.Table.Where(x => x.id == 3 || x.id == 4 || x.id == 5).ToList(),
        //            RestaurantId = 1
        //        });
        //        _context.Employees.Add(new Employees
        //        {
        //            FirstName = "Jean",
        //            LastName = "Polnareff",
        //            Email = "hostJean@qr.com",
        //            Password = "Password123!",
        //            Role = "Host",
        //            RestaurantId = 1

        //        });
        //        _context.Employees.Add(new Employees
        //        {
        //            FirstName = "Noriaki",
        //            LastName = "Kakyoin",
        //            Email = "busboy@qr.com",
        //            Password = "Password123!",
        //            Role = "Busboy",
        //            RestaurantId = 1
        //        });

        //        _context.Menus.Add(new Menus("Menu 1", false, "$", 1));
        //        _context.Menus.Add(new Menus("Menu 2", false, "$", 1));
        //        _context.Menus.Add(new Menus("Menu 3", true, "$", 1));

        //        for (int i = 1; i <= 2; i++)
        //        {
        //            for (int j = 0; j < 3; j++)
        //            {
        //                _context.FoodItem.Add(new FoodItem(i, $"Category{j}", $"Food{j}", 2.99, null, true));
        //            }
        //        }

        //        _context.FoodItem.Add(new FoodItem(3, "SMALL PLATES", "Miso Soup", 4, "daily garnish", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SMALL PLATES", "Seasonal Goma-ae", 8, "umami sweet sesame soy", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SMALL PLATES", "Steamed Edamame", 7, "kosher salt", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SMALL PLATES", "Spicy Sesame Edamame", 8, "chili garlic tamari soy, crispy shallots", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SMALL PLATES", "Brussels Sprout Chips", 8, "togarashi-lemon pepper salt", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Nutrigreens Farm Tofu Salad", 18, "organic baby greens, crispy tempeh, avocado, crumbled miso tofu, pickled daikon and carrot, cucumber, cherry tomato, umami soy vinaigrette", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Chicken Nanban", 16, "lightly fried and tossed in sweet and sour soy, achara, house made tartar sauce", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Ebi Fritters", 18, "white tiger prawns in herb-beer batter, asian slaw, sweet chili lime vinaigrette, chili salt, harissa aioli, soy balsamic reduction", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Tuna and Kaiso Seaweed Tartare", 19, "avocado, red onion, celer y, cucumber, chili garlic tamari soy, wasabi crème fraîche, microgreens, edible flower, sesame rice cracker", false));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Aburi Kobayaki Glazed Tako", 19, "marinated Aburi tomato, charred negi, nutrigreens organic baby greens, pickled red onion, soy-koji cottage cheese, basil seed pico de gallo, soy-balsamic reduction, aka sal", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Hamachi Shiso Oshi Appetizer", 23, "heirloom gem tomato medley, ao nori roasted  cherr y tomatoes, yuzu wasabi chimichurri,  soy-balsamic reduction, ponzu tapioca pearls,  shallot crisps, herbed microgreens", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Kombu Cured Humboldt Squid", 18, "crispy panko breaded humboldt squid, yuzu aioli, house made tsukemono, togarashi lemon pepper salt, pickled mustard seeds", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Aburi Beef Carpaccio", 22, "AAA sterling silver, nikiri ponzu, wasabi chimichurri, shaved grana padano, 63° egg, wasabi crème fraîche, caper crisps, organic baby greens", false));
        //        _context.FoodItem.Add(new FoodItem(3, "ZENSAI | APPETIZER", "Aburi Bone Marrow Gratin", 16, "herbed wagyu panko, wasabi chimichurri, soy-balsamic reduction, microgreens, basil matcha moss, marinated bush mushrooms", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SHOKAI | CHEF'S TASTING MENU", "Yaletown", 75, "five courses", true));
        //        _context.FoodItem.Add(new FoodItem(3, "SHOKAI | CHEF'S TASTING MENU", "Blue Ocean", 90, "five courses", false));
        //        _context.FoodItem.Add(new FoodItem(3, "ENTREES", "Kaisen Soba Peperoncino", 26, "tiger prawns, squid, scallops, sweet pepper, shiitake mushrooms, gem tomatoes, baby bok choy, jalapeño, wild baby arugula, chili garlic soy", true));
        //        _context.FoodItem.Add(new FoodItem(3, "ENTREES", "Aburi Ribeye Steak", 49, "10oz AAA sterling silver, roasted market vegetables, wasabi chimichurri, suntory whisky peppercorn veal jus, shallot crisps", true));




        //        await _context.SaveChangesAsync();

        //        return "Data Generated";
        //    }
        //    catch (Exception e)
        //    {
        //        var xxx = e.Message;
        //        return e.InnerException.ToString();
        //    }
        //}

        [HttpGet]
        [Route("MenuItems")]
        public async Task<string> GetActiveMenu()
        {

            var ActiveMenu = _context.Menus.Where(x => x.IsActive).FirstOrDefault();
            if (ActiveMenu == null)
            {
                return "No Active Menu";
            }
            var menuItems = _context.FoodItem.Where(x => x.MenuId == ActiveMenu.Id).ToList();
            ResponseCustomerMenu _response = new ResponseCustomerMenu();
            CustomerMenuPayload payload = new CustomerMenuPayload();
            payload.items = menuItems;
            payload.name = ActiveMenu.Name;
            payload.currencySymbol = ActiveMenu.CurrencySymbol;
            _response.payload = payload;
            return JsonConvert.SerializeObject(_response);
        }


        [HttpPost("Orders")]
        public async Task<string> PostOrder(OrderInfo orderInfo)
        {
            var table = _context.Table.Where(x => x.TableSid == orderInfo.SessionId).FirstOrDefault();
            if (table == null)
            {
                return "Invalid Session";
            }

            foreach (var order in orderInfo.OrderList)
            {
                var food = await _context.FoodItem.FindAsync(order.FoodId);
                if (food == null)
                {
                    return $"FoodItem with id {food.Id} did not found";
                }
                food.OrderScore += order.Quantity;
                table.addOrder(order);
                _context.FoodItem.Update(food);
                table.TotalPrice += order.Quantity * food.Price;

            }

            _context.Table.Update(table);

            await _context.SaveChangesAsync();
            await _hubContextWaitStaff.Clients.All.SendAsync("ReceiveOrders", JsonConvert.SerializeObject(table));


            return "Order Submitted";
        }

        [HttpGet("Billing/{sid}/{cusId}")]
        public async Task<double> PostOrder(string cusId, string sid)
        {
            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();
            List<Orders> order = table.getList();
            double res = 0;
            foreach (var a in order)
            {
                if (a.CustomerId == cusId)
                {
                    res += a.Quantity * _context.FoodItem.Find(a.FoodId).Price;
                }
            }
            return res;

        }
       

        // POST: api/CustomerRequests
        [HttpPost]
        [Route("TriggeredAction")]
        public async Task<ActionResult<CustomerRequest>> PostCustomerRequest(CustomerRequest customerRequest)
        {
            customerRequest.nameWaiter = "Dio Brando";
            _context.CustomerRequest.Add(customerRequest);
            await _context.SaveChangesAsync();

            //List<string> aaa = new List<string> { "Kebap", "Buritto", "Spriteeee" };


            //var temp = new CustomerResult
            //{
            //    ReasonType = customerRequest.ReasonType,
            //    Parameter = JsonConvert.SerializeObject(aaa)
            //};
            Console.WriteLine(JsonConvert.SerializeObject(_context.CustomerRequest.ToList()));
            await _hubContextWaitStaff.Clients.All.SendAsync("ReceiveRequest", JsonConvert.SerializeObject(_context.CustomerRequest.ToList()));

            return CreatedAtAction(nameof(CustomerRequest), new { id = customerRequest.id }, customerRequest);
        }

    

        [HttpPost]
        [Route("startPayment/{sid}")]
        public async Task<IActionResult> setPayingFlag(string sid)
        {
            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();

            if (table == null)
            {
                return NotFound("Table not found");
            }
            else if (table.IsSomeonePaying)
            {
                return Conflict("Someone is already paying!");
            }
            else
            {
                table.IsSomeonePaying = true;
                await _context.SaveChangesAsync();

                return Ok("Table's payment flag set to true");

            }
        }
        [HttpPost]
        [Route("endPayment/{sid}")]
        public async Task<IActionResult> endPayment(string sid)
        {
            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();

            if (table == null)
            {
                return NotFound("Table not found");
            }
            else if (!table.IsSomeonePaying)
            {
                return Conflict("No one is currently paying");
            }
            else
            {
                table.IsSomeonePaying = false;
                await _context.SaveChangesAsync();

                return Ok("Table's payment flag set to false");
            }
        }


    


    // DELETE: api/CustomerRequests/5

    // POST: api/UserReviews
    [HttpPost("postUserReviews")]
        public async Task<ActionResult<UserReviews>> PostUserReviews(UserReviews userReviews)
        {
            _context.UserReviews.Add(userReviews);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserReviews", new { id = userReviews.Id }, userReviews);
        }

        [HttpGet]
        [Route("ActiveTheme")]
        public async Task<string> GetActiveTheme()
        {
            return JsonConvert.SerializeObject(Base.StaticThemes.DataList.Where(x => x.isDefault).ToList()); ;
        }



        [HttpPost("StripeCreate/{price}")]
        public ActionResult Create(double price, string sid)
        {
            var domain = "http://qr-menu.io";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                   PriceData = new SessionLineItemPriceDataOptions
                    {
                    Currency = "usd",
                    UnitAmount = (int)price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "QR-Menu",
                        Description = "Delicious meal",
                    },
                    },
                    Quantity = 1,

                  },
                },
                Metadata = new Dictionary<string, string> {
                    {"SessionId", sid },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            Console.WriteLine($"Session url {session.Url}");
            return Ok(session.Url);

        }

        [HttpPost("StripeCreate/{sid}/{price}")]
        public ActionResult Create2(double price, string sid)
        {
            var domain = "http://qr-menu.io";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                  new SessionLineItemOptions
                  {
                   PriceData = new SessionLineItemPriceDataOptions
                    {
                    Currency = "usd",
                    UnitAmount = (int)price * 100,
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "QR-Menu",
                        Description = "Delicious meal",
                    },
                    },
                    Quantity = 1,

                  },
                },
                Metadata = new Dictionary<string, string> {
                    {"SessionId", sid },
                },
                Mode = "payment",
                SuccessUrl = domain + "/success",
                CancelUrl = domain + "/cancel",
            };
            var service = new SessionService();
            Session session = service.Create(options);

            Response.Headers.Add("Location", session.Url);
            Console.WriteLine($"Session url {session.Url}");
            return Ok(session.Url);

        }

        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

            const string endpointSecret = "whsec_...";
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var signatureHeader = Request.Headers["Stripe-Signature"];

                stripeEvent = EventUtility.ConstructEvent(json,
                        signatureHeader, endpointSecret);
                if (stripeEvent.Type == Events.PaymentIntentSucceeded)
                {
                    var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                    Console.WriteLine("A successful payment for {0} was made.", paymentIntent.Amount);
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                }
                else if (stripeEvent.Type == Events.PaymentMethodAttached)
                {
                    var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                    // Then define and call a method to handle the successful attachment of a PaymentMethod.
                    // handlePaymentMethodAttached(paymentMethod);
                }
                else
                {
                    Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                Console.WriteLine("Error: {0}", e.Message);
                return BadRequest();
            }
            catch (Exception e)
            {
                return StatusCode(500);
            }

    }

        private void FulfillOrder(Session session, string sid)
        {

            var table = _context.Table.Where(x => x.TableSid == sid).FirstOrDefault();

            table.TotalPrice = (double)(table.TotalPrice - session.AmountTotal);
            _context.SaveChangesAsync();
        }


        private bool CustomerRequestExists(long id)
        {
            return _context.CustomerRequest.Any(e => e.id == id);
        }
    }
}
