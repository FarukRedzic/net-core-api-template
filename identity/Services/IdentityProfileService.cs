using identity.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identity.Services
{
    public class IdentityProfileService : IProfileService
    {
        /// <summary>
        /// This method is called whenever claims about the user are requested (e.g. during
        //  token creation or via the userinfo endpoint)
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var identity = (ClaimsIdentity)context.Subject.Identity;

            //Add claims to issued access token
            context.IssuedClaims.AddRange(identity.Claims);
            return Task.FromResult(0);
        }

        /// <summary>
        /// This method gets called whenever identity server needs to determine if the user
        //  is valid or active (e.g. if the user's account has been deactivated since they
        //  logged in). (e.g. during token issuance or validation).
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(0);
        }
    }
}
