using QrMenuAgain.BASE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using QrMenuAgain.Models;

namespace QrMenuAgain.Controllers.IdentityBase
{
    public interface ICurrentUser
    {
        //public Task<Employees> Detail();
        public Task<Result<IdentityTokenResult>> SignIn(Employees data, string P, string Role);
        public Task<Result> Update(Employees data);
        public bool IsLogin();
        public Task<Result> SignOut();
    }
}
