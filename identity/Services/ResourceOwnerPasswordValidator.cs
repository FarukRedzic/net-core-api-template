using identity.Data.Models;
using identity.Interfaces;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identity.Services
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly IIdentityUserService<ApplicationUser> _identityUserService;

        public ResourceOwnerPasswordValidator(IIdentityUserService<ApplicationUser> identityUserService)
        {
            _identityUserService = identityUserService;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            try
            {
                var user = await _identityUserService.FindByNameAsync(context.UserName);
                if (user != null)
                {
                    var verificationResult = _identityUserService.VerifyPassword(user, context.Password);
                    if (verificationResult == PasswordVerificationResult.Success)
                    {
                        List<Claim> userClaims = (await _identityUserService.GetClaimsAsync(user)).ToList();
                        //Add role claims for role authorization
                        foreach (var role in await _identityUserService.GetRolesAsync(user))
                            userClaims.Add(new Claim(ClaimTypes.Role, role));

                        context.Result = new GrantValidationResult(
                            subject: user.Id,
                            authenticationMethod: "custom",
                            claims: userClaims);
                        return;
                    }

                    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Incorrect password.");
                    return;
                }
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "User does not exist.");
                return;
            }
            catch (Exception)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "Invalid username or password");
            }
        }
    }
}
