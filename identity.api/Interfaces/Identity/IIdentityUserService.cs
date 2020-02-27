using identity.api.Models.Identity;
using identity.api.Models.oAuth;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace identity.api.Interfaces.Identity
{
    public interface IIdentityUserService
    {
        Task<IdentityResult> Register(ApplicationUserDTO user);
        Task<TokenResponse> Token(TokenRequestDTO request);
        Task<TokenResponse> RefreshToken(TokenRequestDTO request);
        Task<TokenRevocationResponse> RevokeToken(TokenRequestDTO request);
    }
}
