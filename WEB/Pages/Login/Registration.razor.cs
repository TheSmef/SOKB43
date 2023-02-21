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
using Models.Dto.PostPutModels.AccountModels;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Pages.Login
{
    public partial class Registration
    {
        [Inject]
        private IAuthService? AuthService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private NavigationManager? NavigationManager { get; set; }

        [Parameter]
        [SupplyParameterFromQuery]
        public string? returnUrl { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        private RegModel reg = new RegModel();
        private async Task HandleRegistration()
        {
            try
            {
                await AuthService!.regUser(reg);
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

        protected void Close(MouseEventArgs args)
        {
            DialogService!.Close(null);
        }
    }
}