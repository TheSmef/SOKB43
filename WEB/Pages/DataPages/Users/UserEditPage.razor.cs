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
    public partial class UserEditPage
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

        [Inject]
        private IMapper? Mapper { get; set; }

        [Parameter]
        public User? userEdit { get; set; }

        private UserUpdateDto user = new UserUpdateDto();
        private List<Role> roles = new List<Role>();
        protected override void OnInitialized()
        {
            user = Mapper!.Map<User, UserUpdateDto>(userEdit!);
            user.Email = userEdit!.Account!.Email;
            user.Login = userEdit!.Account!.Login;
            user.Roles = userEdit!.Account!.Roles == null ? new List<Role>() : (List<Role>)userEdit!.Account!.Roles;
            foreach (string value in EnumUtility.GetStringsValues(typeof(Role.NameRole)))
            {
                if (user.Roles!.Where(x => x.Name == value).Any())
                {
                    roles.Add(user.Roles!.Find(x => x.Name == value)!);
                }
                else
                {
                    roles.Add(new Role()
                    {Name = value});
                }
            }
        }

        private async Task HandleEdit()
        {
            try
            {
                await UserService!.UpdateUser(user, userEdit!.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное обновление!", "Пользователь успешно обновлён", 4000);
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