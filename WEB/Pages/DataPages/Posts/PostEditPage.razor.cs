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

namespace WEB.Pages.DataPages.Posts
{
    public partial class PostEditPage
    {
        [Inject]
        private IPostService? PostService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private ILocalStorageService? StorageService { get; set; }

        [Inject]
        private AuthenticationStateProvider? AuthenticationStateProvider { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Parameter]
        public Post? record { get; set; }

        private async Task HandleEdit()
        {
            try
            {
                await PostService!.UpdatePost(record!);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное изменение!", "Должность успешно изменена", 4000);
                Close();
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
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
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