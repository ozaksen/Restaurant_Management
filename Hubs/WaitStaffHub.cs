using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalRWaitStaff.Hubs
{
    public class WaitStaffHub : Hub
    {
        public async Task SendCustomerRequest(QrMenuAgain.Models.CustomerRequest CustomerReq)
        {
            await Clients.All.SendAsync("ReceiveRequest", CustomerReq);
        }
    }
}