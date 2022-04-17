using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using QrMenuAgain.BASE;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using QrMenuAgain.Models;

namespace QrMenuAgain.Controllers.IdentityBase
{
    public class CurrentUser : ICurrentUser
    {
        private readonly UserManager<Employees> _userManager;
        private readonly SignInManager<Employees> _signInManager;
        private IHttpContextAccessor _httpContextAccessor;

        public CurrentUser(UserManager<Employees> userManager, SignInManager<Employees> signInManager, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _httpContextAccessor = httpContextAccessor;
        }

        public bool IsLogin()
        {
            return _signInManager.IsSignedIn(_httpContextAccessor.HttpContext.User);
        }

     

        public async Task<Result<Models.IdentityTokenResult>> SignIn(Models.Employees data, string P, string UserType) //true ise admin
        {
            var _result = new Result<Models.IdentityTokenResult>();

            try
            {
                var user = new Employees
                {
                    Id = data.Id,
                    FirstName = data.FirstName,
                    LastName = data.LastName,
                    Role = UserType,
                    Email = data.Email,
                    UserName = data.Email
                    
                };

                var _idenResult = await _userManager.CreateAsync(user, P);

                if (_idenResult.Succeeded)
                {
                    var _c = await _userManager.AddToRoleAsync(user, UserType);

                    var authClaims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Email, data.Email),
                            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        };

                    authClaims.Add(new Claim(ClaimTypes.Role, UserType));

                    var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("cGBZ8Rmg0MVAXlAU7ndx9mSoW6GchqQR"));

                    var token = new JwtSecurityToken(
                        expires: DateTime.Now.AddHours(3),
                        claims: authClaims,
                        signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                        );

                    var _ac = _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Email, data.Email));

                    var signInResult = await _signInManager.PasswordSignInAsync(user, P, true, false);

                    _result.Success = signInResult.Succeeded;

                    if (_result.Success)
                    {
                        _result.ResultObj = new Models.IdentityTokenResult
                        {
                            Token = new JwtSecurityTokenHandler().WriteToken(token),
                            ValidDate = token.ValidTo
                        };
                    }
                }
            }
            catch (Exception ex)
            {
            }

            return _result;
        }

        public async Task<Result> Update(QrMenuAgain.Models.Employees data)
        {
            var _result = new Result();

            var current = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);

            if (current != null)
            {
                current.FirstName = data.FirstName;
                current.LastName = data.LastName;

                var process = await _userManager.UpdateAsync(current);

                _result.Success = process.Succeeded;
            }

            return _result;
        }


        public async Task<Result> DeleteConfirmed(string id)
        {
            var _result = new Result();

            var user = await _userManager.FindByIdAsync(id);
            var logins = await _userManager.GetLoginsAsync(user);
            var rolesForUser = await _userManager.GetRolesAsync(user);


            foreach (var login in logins.ToList())
            {
                await _userManager.RemoveLoginAsync(user, login.LoginProvider, login.ProviderKey);
            }

            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                    var result = await _userManager.RemoveFromRoleAsync(user, item);
                }
            }

            //Delete User
            await _userManager.DeleteAsync(user);

            _result.SetSuccess();

            return _result;

        }

        public async Task<Result> SignOut()
        {
            var _result = new Result();

            await _signInManager.SignOutAsync();

            _result.SetSuccess();

            return _result;
        }

 
    }
}
