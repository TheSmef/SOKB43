using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Radzen;
using System.Reflection;
using WEB.Data.UtilityServices.Base;

namespace WEB.Data.UtilityServices
{
    public class AuthInterceptor : IAuthInterceptor
    {
        private ILocalStorageService storageService { get; set; }

        private AuthenticationStateProvider authenticationStateProvider;
        public AuthInterceptor(ILocalStorageService storageService, AuthenticationStateProvider authenticationStateProvider)
        {
            this.authenticationStateProvider = authenticationStateProvider;
            this.storageService = storageService;
        }

        public async Task<bool> ReloadAuthState(AuthenticationState? state ,List<string> roles)
        {
            try
            {
                if (state == null)
                {
                    return false;
                }
                await storageService!.RemoveItemAsync("jwttoken");
                await authenticationStateProvider!.GetAuthenticationStateAsync();
                if (state.User?.Identity != null)
                {
                    if (roles.Count != 0)
                    {
                        foreach (string value in roles)
                        {
                            if (state!.User!.Claims.Where(x => x.Value == value).Any())
                            {
                                return true;
                            }
                        }
                    }
                    else
                    {
                        return true;
                    }
                    return false;
                }
                return false;
            }
            catch
            {
                return false;
            }

        }
    }
}
