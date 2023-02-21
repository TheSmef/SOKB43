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
using Models.Entity;
using Models.QuerySupporter;
using System.Security.Claims;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Posts
{
    public partial class PostsPage
    {

        private RadzenDataGrid<Post>? grid;
        private PostsGetDtoModel? posts = new PostsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
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

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }



        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<Post> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditPost(args.Data);
                    break;
                }

                case 2:
                {
                    await DeletePost(args.Data);
                    break;
                }
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = args.Filter, OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                posts = await PostService!.GetPosts(query);
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

                    await LoadData(args);
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

        private async Task AddPost()
        {
            await DialogService!.OpenAsync<PostAddPage>(ConstantValues.POSTADD_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task EditPost(Post data)
        {
            await DialogService!.OpenAsync<PostEditPage>(ConstantValues.POSTEDIT_TITLE, new Dictionary<string, object>()
            {{ConstantValues.RECORD, data}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task DeletePost(Post data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await PostService!.DeletePost(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Должность успешно удалена", 4000);
                    await grid!.Reload();
                }
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

                    await DeletePost(data);
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
    }
}