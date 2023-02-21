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
using AutoMapper;
using Blazored.LocalStorage;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Pages.Profile
{
    public partial class ChangeProfile
    {
        [Inject]
        private IProfileService? ProfileService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private ILocalStorageService? StorageService { get; set; }

        [Parameter]
        public User? profile { get; set; }

        private UpdateModel user = new UpdateModel();
        [Inject]
        private IMapper? Mapper { get; set; }

        protected override void OnInitialized()
        {
            user = Mapper!.Map<User, UpdateModel>(profile!);
            user.Email = profile!.Account!.Email;
            user.Login = profile!.Account!.Login;
        }

        private async Task HandleEdit()
        {
            try
            {
                profile = await ProfileService!.UpdateProfile(user);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное обновление!", "Профиль успешно обновлён", 4000);
                Close();
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch (UnAuthException)
            {
                if ((await AuthenticationStateTask!).User?.Identity != null)
                {
                    await StorageService!.RemoveItemAsync("jwttoken");
                    await AuthenticationStateProvider!.GetAuthenticationStateAsync();
                    await HandleEdit();
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, повторно авторизируйтесь", 4000);
                }
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        protected void Close()
        {
            DialogService!.Close(null);
        }
    }
}