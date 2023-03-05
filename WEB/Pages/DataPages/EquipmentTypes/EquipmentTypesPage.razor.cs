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
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.EquipmentTypes
{
    public partial class EquipmentTypesPage
    {
        private RadzenDataGrid<TypeEquipment>? grid;
        private EquipmentTypesGetDtoModel? records = new EquipmentTypesGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IEquipmentTypeService? TypeService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<TypeEquipment> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditRecord(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteRecord(args.Data);
                    break;
                }
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = args.Filter, OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                records = await TypeService!.GetTypes(query);
                if (records!.Collection!.Count == 0 && records!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(records!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
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

        private async Task AddRecord()
        {
            await DialogService!.OpenAsync<EquipmentTypesAddPage>(ConstantValues.TYPEADD_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task EditRecord(TypeEquipment data)
        {
            try
            {
                TypeEquipment record = await TypeService!.GetTypeById(data.Id);
                if (record == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await grid!.Reload();
                    return;
                }
                await DialogService!.OpenAsync<EquipmentTypesEditPage>(ConstantValues.TYPEEDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.TYPE, record}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
                {
                    await EditRecord(data);
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

        private async Task DeleteRecord(TypeEquipment data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await TypeService!.DeleteType(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Тип оборудования успешно удалён", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
                {
                    await DeleteRecord(data);
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