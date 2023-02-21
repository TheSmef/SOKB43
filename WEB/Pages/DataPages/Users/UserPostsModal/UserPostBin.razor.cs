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

namespace WEB.Pages.DataPages.Users.UserPostsModal
{
    public partial class UserPostBin
    {
        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<UserPost> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await RestoreUserPost(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteUserPost(args.Data);
                    break;
                }
            }
        }

        private RadzenDataGrid<UserPost>? grid;
        private UserPostsGetDtoModel? bin = new UserPostsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IUserPostService? UserPostService { get; set; }

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

        [Parameter]
        public User? user { get; set; }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(User.Id)) ==  " + "\"" + user!.Id.ToString() + "\") and ((np(Deleted)) == true)" : args.Filter + " and ((np(User.Id)) == " + "\"" + user!.Id.ToString() + "\") and ((np(Deleted)) == true)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                bin = await UserPostService!.GetUserPosts(query);
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

        private async Task RestoreUserPost(UserPost model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.RESTORE_RECORD, ConstantValues.RESTORE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    UserPostDto userPostDto = new UserPostDto();
                    UserPost userPost = await UserPostService!.GetUserPostById(model.Id);
                    userPostDto.Share = userPost.Share;
                    userPostDto.UserId = userPost.User!.Id;
                    userPostDto.PostId = userPost.Post!.Id;
                    userPostDto.Deleted = false;
                    await UserPostService!.UpdateUserPost(userPostDto, userPost.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно востановлена!", "Должность пользователя восстановленна", 4000);
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

                    await DeleteUserPost(model);
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

        private async Task DeleteUserPost(UserPost data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await UserPostService!.DeleteUserPost(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Должность сотрудника успешно удалена", 4000);
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

                    await DeleteUserPost(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа к серверу, повторно авторизируйтесь", 4000);
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