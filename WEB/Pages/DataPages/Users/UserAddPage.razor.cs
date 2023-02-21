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
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Users
{
    public partial class UserAddPage
    {
        [Inject]
        private IUserService? UserService { get; set; }

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

        private UserDto user = new UserDto();
        private List<Role> roles = new List<Role>();
        protected override void OnInitialized()
        {
            foreach (string value in EnumUtility.GetStringsValues(typeof(Role.NameRole)))
            {
                roles.Add(new Role()
                {Name = value});
            }
        }

        private async Task HandleAdd()
        {
            try
            {
                await UserService!.AddUser(user);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Пользователь успешно добавлен", 4000);
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
                    if (!(await AuthenticationStateTask!).User!.Claims.Where(x => (x.Value == "Администратор") || (x.Value == "Отдел кадров")).Any())
                    {
                        NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступа к данной функции", 4000);
                        return;
                    }

                    await HandleAdd();
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