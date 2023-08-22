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
using WEB.Data.Services.Base;
using WEB.Utility;
using WEB.Data.UtilityServices;
using WEB.Data.UtilityServices.Base;

namespace WEB.Shared
{
    public partial class MainLayout
    {
        private RadzenSidebar? sidebar;
        [Inject]
        private IProfileService? ProfileService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private ILocalStorageService? StorageService { get; set; }

        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        protected async Task ProfileMenuClick(dynamic args)
        {
            if (args.Value == "Logout")
            {
                try
                {
                    await ProfileService!.LogOut();
                }
                catch (AppException e)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                }
                catch (UnAuthException)
                {
                    if (await AuthInterceptor!.ReloadAuthState(new List<string>()))
                    {
                        await AuthenticationStateProvider!.GetAuthenticationStateAsync();
                        await ProfileMenuClick(args);
                    }
                    else
                    {
                        NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа к серверу, повторно авторизируйтесь", 4000);
                    }
                }
                catch
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе к серверу, попробуйте повторить запрос позже", 4000);
                }
            }
        }

        protected async Task SidebarToggle()
        {
            await InvokeAsync(() =>
            {
                sidebar!.Toggle();
            });
        }
    }
}