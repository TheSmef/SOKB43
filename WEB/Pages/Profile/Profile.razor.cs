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
using Models.Entity;
using WEB.Data.Services.Base;
using WEB.Utility;
using WEB.Data.UtilityServices.Base;

namespace WEB.Pages.Profile
{
    public partial class Profile
    {
        [Inject]
        private IProfileService? ProfileService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }
        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        private User user = new User()
        {Account = new Account()};
        protected override async Task OnInitializedAsync()
        {
            try
            {
                user = await ProfileService!.GetProfile();
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()))
                {
                    await OnInitializedAsync();
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        protected async Task ProfileChange()
        {
            await DialogService!.OpenAsync<ChangeProfile>(ConstantValues.PROFILE_TITLE, new Dictionary<string, object>()
            {{ConstantValues.PROFILE, user}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await OnInitializedAsync();
        }

        protected async Task GetTokens()
        {
            await DialogService!.OpenAsync<Tokens>(ConstantValues.TOKENS_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true, Width="700px"});
            await OnInitializedAsync();
        }
    }
}