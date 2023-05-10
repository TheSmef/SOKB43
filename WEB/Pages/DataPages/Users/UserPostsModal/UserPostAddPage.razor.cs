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
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using WEB.Data.Services.Base;
using WEB.Utility;
using WEB.Data.UtilityServices.Base;

namespace WEB.Pages.DataPages.Users.UserPostsModal
{
    public partial class UserPostAddPage
    {
        private PostsGetDtoModel? posts = new PostsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IPostService? PostService { get; set; }

        [Inject]
        private IUserPostService? UserPostService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Parameter]
        public User? user { get; set; }

        private UserPostDto userpost = new UserPostDto();

        private string roles = "Администратор, Отдел кадров";
        protected override void OnInitialized()
        {
            if (user == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Сотрудник для должности не был выбран", 4000);
                Close();
            }    
            userpost.UserId = user!.Id;
            userpost.Deleted = false;
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? args.Filter : "(np(Name).Contains(" + "\"" + args.Filter + "\"))", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                posts = await PostService!.GetPosts(query);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {              
                    await LoadData(args);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
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

        private async Task HandleAdd()
        {
            try
            {
                await UserPostService!.AddUserPost(userpost);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Должность сотрудника успешно добавлена", 4000);
                Close();
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await HandleAdd();
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

        protected void Close()
        {
            DialogService!.Close(null);
        }
    }
}