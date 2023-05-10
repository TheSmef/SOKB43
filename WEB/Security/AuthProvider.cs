using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Security
{
    public class AuthProvider : AuthenticationStateProvider
    {
        private readonly ILocalStorageService storage;
        private readonly IAuthService authService;
        private readonly HttpClient client;
        public AuthProvider(ILocalStorageService localStorage, IAuthService authService, HttpClient client)
        {
            this.storage = localStorage;
            this.authService = authService;
            this.client = client;
        }



        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var state = new AuthenticationState(new ClaimsPrincipal());
            try
            {
                var refreshtoken = await storage.GetItemAsStringAsync("token");
                if (refreshtoken != null)
                {
                    await authService.getToken();
                }
                var token = await storage.GetItemAsStringAsync("jwttoken");
                if (token != null)
                {
                    var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
                    if (jwt.Claims.Any())
                    {
                        var identity = new ClaimsIdentity(ConstantValues.USER_AUTH_TYPE);
                        identity.AddClaims(jwt.Claims);
                        state = new AuthenticationState(new ClaimsPrincipal(identity));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("\n", ""));
                    }
                }
                else
                {
                    await storage.RemoveItemAsync("token");
                    client.DefaultRequestHeaders.Authorization = null;
                }
            }
            catch
            {
                state = new AuthenticationState(new ClaimsPrincipal());
                client.DefaultRequestHeaders.Authorization = null;
            }
            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }

    }
}
