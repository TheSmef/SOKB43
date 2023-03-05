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
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Pages.DataPages.Users.UserPostsModal;
using WEB.Utility;
using WEB.Data.UtilityServices.Base;

namespace WEB.Pages.DataPages.Users
{
    public partial class UsersPage
    {
        private RadzenDataGrid<User>? grid;
        private RadzenDataGrid<UserPost>? childgrid;
        private UsersGetDtoModel? users = new UsersGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private UserPostsGetDtoModel? userPosts = new UserPostsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        [Inject]
        private IUserService? UserService { get; set; }

        [Inject]
        private IUserPostService? UserPostService { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        private List<Role> roles = new List<Role>();
        private async Task RowExpended(User user)
        {
            roles.Clear();
            foreach (string value in EnumUtility.GetStringsValues(typeof(Role.NameRole)))
            {
                if (user.Account!.Roles!.Where(x => x.Name == value).Any())
                {
                    roles.Add(user.Account!.Roles!.Where(x => x.Name == value).First());
                }
                else
                {
                    roles.Add(new Role()
                    {Name = value});
                }
            }

            userPosts = new UserPostsGetDtoModel()
            {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
            LoadDataArgs args = new LoadDataArgs()
            {Skip = 0, Top = 5};
            await LoadChildData(args, user);
        }

        private async Task ChangeRoles(User model)
        {
            try
            {
                User userEdit = await UserService!.GetUserById(model.Id);
                if (userEdit == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные о пользователе отсутствуют", 4000);
                    await grid!.Reload();
                    return;
                }
                UserUpdateDto user = new UserUpdateDto();
                user = Mapper!.Map<User, UserUpdateDto>(userEdit!);
                user.Email = userEdit!.Account!.Email;
                user.Login = userEdit!.Account!.Login;
                user.Roles = model.Account!.Roles;
                model = await UserService!.UpdateUser(user, model.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное изменение ролей!", "Роли пользователя успешно изменены", 4000);
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await ChangeRoles(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<User> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditUser(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteUser(args.Data);
                    break;
                }
            }
        }

        private async Task ChildContextMenuSelector(DataGridCellMouseEventArgs<UserPost> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditUserPost(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteUserPost(args.Data);
                    break;
                }
            }
        }

        private async Task LoadChildData(LoadDataArgs args, User data)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(User.Id)) ==  " + "\"" + data.Id.ToString() + "\") and ((np(Deleted)) == false)"
                    : args.Filter + " and ((np(User.Id)) == " + "\"" + data.Id.ToString() + "\") and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                userPosts = await UserPostService!.GetUserPosts(query);
                if (userPosts!.Collection!.Count == 0 && userPosts!.CurrentPageIndex != 1)
                {
                    await childgrid!.GoToPage(userPosts!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await LoadChildData(args, data);
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

        private async Task AddUserPost(User data)
        {
            await DialogService!.OpenAsync<UserPostAddPage>(ConstantValues.USERPOSTADD_TITLE, new Dictionary<string, object>()
            {{ConstantValues.USER, data}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await childgrid!.Reload();
        }

        private async Task DeleteUserPost(UserPost model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    UserPostDto userPostDto = new UserPostDto();
                    UserPost userPost = await UserPostService!.GetUserPostById(model.Id);
                    userPostDto.Share = userPost.Share;
                    userPostDto.UserId = userPost.User!.Id;
                    userPostDto.PostId = userPost.Post!.Id;
                    userPostDto.Deleted = true;
                    await UserPostService!.UpdateUserPost(userPostDto, userPost.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно удалена!", "Должность сотрудника помещена в корзину", 4000);
                    await childgrid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await DeleteUserPost(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task EditUserPost(UserPost model)
        {
            try
            {
                UserPost userPost = await UserPostService!.GetUserPostById(model.Id);
                if (userPost == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await childgrid!.Reload();
                    return;
                }
                await DialogService!.OpenAsync<UserPostEditPage>(ConstantValues.USEREDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.USERPOST, userPost}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await EditUserPost(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = args.Filter, OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                users = await UserService!.GetUsers(query);
                if (users!.Collection!.Count == 0 && users!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(users!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
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

        private async Task AddUser()
        {
            await DialogService!.OpenAsync<UserAddPage>(ConstantValues.USERADD_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task CheckRecyclerBin(User data)
        {
            await DialogService!.OpenAsync<UserPostBin>(ConstantValues.USERPOST_BIN_TITLE, new Dictionary<string, object>()
            {{ConstantValues.USER, data}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true, Width = "800px", Resizable = true });
            await childgrid!.Reload();
        }

        private async Task EditUser(User data)
        {
            try
            {
                User user = await UserService!.GetUserById(data.Id);
                if (user == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await grid!.Reload();
                    return;
                }
                await DialogService!.OpenAsync<UserEditPage>(ConstantValues.USEREDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.USEREDIT, user}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await EditUser(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task DeleteUser(User data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await UserService!.DeleteUser(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Пользователь успешно удалён", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>() { "Администратор", "Отдел кадров" }))
                {
                    await DeleteUser(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }
    }
}