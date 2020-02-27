using identity.Data.Models;
using identity.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identity.Services
{
    public class IdentityUserService<TUser> : IIdentityUserService<TUser> where TUser : ApplicationUser
    {
        private readonly UserManager<TUser> _userManager;

        public IdentityUserService(UserManager<TUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<TUser> FindByNameAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        public async Task<IList<string>> GetRolesAsync(TUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public PasswordVerificationResult VerifyPassword(TUser user, string password)
        {
            return _userManager.PasswordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        }
    }
}
