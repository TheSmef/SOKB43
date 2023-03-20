using Microsoft.AspNetCore.Components.Authorization;

namespace WEB.Data.UtilityServices.Base
{
    public interface IAuthInterceptor
    {
        public Task<bool> ReloadAuthState(List<string> roles);
    }
}
