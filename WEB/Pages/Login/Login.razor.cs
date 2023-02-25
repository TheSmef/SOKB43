using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Web.Virtualization;
using Microsoft.JSInterop;
using WEB;
using WEB.Shared;
using Radzen;
using Radzen.Blazor;
using Blazored.LocalStorage;
using Models.Dto.PostPutModels.AccountModels;
using WEB.Data.Services.Base;
using WEB.Utility;
using Microsoft.IdentityModel.Tokens;

namespace WEB.Pages.Login
{
    public partial class Login
    {
        [Inject]
        private IAuthService? AuthService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? returnUrl { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        private AuthModel auth = new AuthModel();
        protected override async Task OnInitializedAsync()
        {
            var authenticationState = await AuthenticationStateTask!;
            if (authenticationState != null)
            {
                if (authenticationState.User.Identity != null)
                {
                    if (returnUrl.IsNullOrEmpty())
                    {
                        NavigationManager!.NavigateTo("profile");
                    }
                    else
                    {
                        NavigationManager?.NavigateTo($"/{returnUrl}");
                    }
                }
            }
        }

        private async void HandleLogin()
        {
            try
            {
                if (auth.RememberMe)
                {
                    await AuthService!.authUser(auth);
                    await AuthService!.getToken();
                }
                else
                {
                    await AuthService!.authUser(auth);
                }

                await AuthenticationStateProvider!.GetAuthenticationStateAsync();
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    NavigationManager?.NavigateTo($"/{returnUrl}");
                }
                else
                {
                    NavigationManager?.NavigateTo("profile");
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе к серверу, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task ToRegistration()
        {
            await DialogService!.OpenAsync<Registration>(ConstantValues.REGISTRATION_TITLE, new Dictionary<string, object>()
            {{ConstantValues.RETURN_URL, returnUrl!}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
        }
    }
}