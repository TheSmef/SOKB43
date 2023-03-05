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
using WEB.Data.Services.Base;
using WEB.Utility;
using Radzen.Blazor.Rendering;
using WEB.Data.UtilityServices;
using WEB.Data.UtilityServices.Base;

namespace WEB.Pages.Profile
{
    public partial class Tokens
    {
        private RadzenDataGrid<Token>? grid;
        private TokensGetDtoModel? tokens = new TokensGetDtoModel() { CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0 };

        private QuerySupporter query = new QuerySupporter();

        [Inject]
        private IProfileService? ProfileService { get; set; }
        [Inject]
        private NotificationService? NotificationService { get; set; }
        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }
        [Inject]
        private ILocalStorageService? StorageService { get; set; }
        [Inject]
        private DialogService? DialogService { get; set; }
        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }
        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        private string token = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            token = await StorageService!.GetItemAsStringAsync("token");
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter { Filter = args.Filter, OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value };
                tokens = await ProfileService!.GetTokens(query);
                if (tokens!.Collection!.Count == 0 && tokens!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(tokens!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()))
                {
                    await LoadData(args);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!"
                        , "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!"
                    , "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }


        }


        private async Task DeleteToken(Token data)
        {
            try
            {

                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {
                    CloseDialogOnOverlayClick = true,
                    CancelButtonText = ConstantValues.CANCEL,
                    OkButtonText = ConstantValues.OK_DELETE
                }) == true)
                {
                    await ProfileService!.deleteToken(data.TokenStr);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Токен успешно удалён", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()))
                {
                    await DeleteToken(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!"
                        , "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!"
                    , "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }
    }
}