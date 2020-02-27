using AutoMapper;
using identity.api.Interfaces.Identity;
using identity.api.Models.Identity;
using identity.api.Models.oAuth;
using identity.Data.Models;
using IdentityModel.Client;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace identity.api.Services.Identity
{
    public class IdentityApplicationUserService : IIdentityUserService
    {

        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpClientFactory _clientFactory;

        public IdentityApplicationUserService(
                UserManager<ApplicationUser> userManager,
                IMapper mapper,
                IConfiguration config,
                IHttpClientFactory clientFactory)
        {
            _userManager = userManager;
            _mapper = mapper;
            _config = config;
            _clientFactory = clientFactory;
        }

        public async Task<IdentityResult> Register(ApplicationUserDTO user)
        {
            try
            {
                ApplicationUser applicationUser = _mapper.Map<ApplicationUserDTO, ApplicationUser>(user);
                var identityResult = await _userManager.CreateAsync(applicationUser, user.Password);

                if (identityResult.Succeeded)
                {
                    await _userManager.AddClaimsAsync(applicationUser, new List<Claim>() {
                        new Claim("user_id", applicationUser.Id),
                        new Claim("email", applicationUser.Email)
                    });
                    await _userManager.AddToRolesAsync(applicationUser, user.Roles);
                }

                return identityResult;
            }
            catch (Exception ex)
            {
                //add logger
                throw ex;
            }
        }

        public async Task<TokenResponse> Token(TokenRequestDTO tokenRequest)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var disco = await client.GetDiscoveryDocumentAsync(_config.GetSection("AUTHORITY").Value);
                switch (tokenRequest.GrantType)
                {
                    case "password":
                        var passwordFlow = await client.RequestPasswordTokenAsync(new PasswordTokenRequest()
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = tokenRequest.ClientId,
                            ClientSecret = tokenRequest.ClientSecret,
                            Scope = tokenRequest.Scope,
                            UserName = tokenRequest.Username,
                            Password = tokenRequest.Password
                        });
                        return passwordFlow;
                    case "client_credentials":
                        var clientCredentialsFlow = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest()
                        {
                            Address = disco.TokenEndpoint,
                            ClientId = tokenRequest.ClientId,
                            ClientSecret = tokenRequest.ClientSecret,
                            Scope = tokenRequest.Scope,
                        });
                        return clientCredentialsFlow;
                    default:
                        return new TokenResponse(HttpStatusCode.BadRequest, "grant_type is not supported", null);
                }
            }

            catch (Exception ex)
            {
                //add logger
                throw ex;
            }
        }

        public async Task<TokenResponse> RefreshToken(TokenRequestDTO tokenRequest)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                //TODO: use cache for discovery document
                var disco = await client.GetDiscoveryDocumentAsync(_config.GetSection("AUTHORITY").Value);
                var refreshToken = await client.RequestRefreshTokenAsync(new RefreshTokenRequest()
                {
                    Address = disco.TokenEndpoint,
                    ClientId = tokenRequest.ClientId,
                    ClientSecret = tokenRequest.ClientSecret,
                    RefreshToken = tokenRequest.RefreshToken
                });
                return refreshToken;
            }
            catch (Exception ex)
            {
                //add logger
                throw ex;
            }
        }

        public async Task<TokenRevocationResponse> RevokeToken(TokenRequestDTO tokenRequest)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var disco = await client.GetDiscoveryDocumentAsync(_config.GetSection("AUTHORITY").Value);

                var revokeResult = await client.RevokeTokenAsync(new TokenRevocationRequest
                {
                    Address = disco.RevocationEndpoint,
                    ClientId = tokenRequest.ClientId,
                    ClientSecret = tokenRequest.ClientSecret,
                    Token = tokenRequest.RefreshToken
                });
                return revokeResult;
            }
            catch (Exception ex)
            {
                //add logger
                throw ex;
            }
        }
    }
}
