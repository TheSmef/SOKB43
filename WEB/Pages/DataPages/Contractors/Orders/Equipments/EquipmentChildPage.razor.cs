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
using System.Security.Claims;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Contractors.Orders.Equipments
{
    public partial class EquipmentChildPage
    {
        [Parameter]
        public Order? order { get; set; }

        private RadzenDataGrid<Equipment>? grid;
        private EquipmentDtoGetModel? records = new EquipmentDtoGetModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IEquipmentService? EquipmentService { get; set; }

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

        [Inject]
        private IMapper? Mapper { get; set; }

        protected override void OnInitialized()
        {
            if (order == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Ошибка загрузки данных, заказ не был задан", 4000);
                Close();
                return;
            }
        }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<Equipment> args, MenuItemEventArgs value)
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

        private async Task CheckRecyclerBin()
        {
            await DialogService!.OpenAsync<EquipmentBin>(ConstantValues.EQ_BIN_TITLE, new Dictionary<string, object>()
            {{ConstantValues.ORDER, order!}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true, Width = "800px", Resizable = true });
            await grid!.Reload();
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Order.Id)) ==  " + "\"" + order!.Id.ToString() + "\") and ((np(Deleted)) == false)" : args.Filter + " and ((np(Order.Id)) == " + "\"" + order!.Id.ToString() + "\") and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                records = await EquipmentService!.GetEquipment(query);
                if (records!.Collection!.Count == 0 && records!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(records!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами"}))
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
            await DialogService!.OpenAsync<EquipmentAddPage>(ConstantValues.EQADD_TITLE, new Dictionary<string, object>()
            {{ConstantValues.ORDER, order!}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task EditRecord(Equipment data)
        {
            try
            {
                Equipment equipment = await EquipmentService!.GetEquipmentById(data.Id);
                if (equipment == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await grid!.Reload();
                    return;
                }

                await DialogService!.OpenAsync<EquipmentEditPage>(ConstantValues.EQEDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.EQ, equipment}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами"}))
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

        private async Task DeleteRecord(Equipment model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    Equipment equipment = await EquipmentService!.GetEquipmentById(model.Id);
                    EquipmentDto dto = Mapper!.Map<EquipmentDto>(equipment);
                    dto.OrderId = equipment.Order!.Id;
                    dto.TechnicalTaskId = equipment.TechnicalTask!.Id;
                    dto.Deleted = true;
                    await EquipmentService!.UpdateEquipment(dto, model.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно удалена!", "Оборудование помещено в корзину", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами"}))
                {
                    await DeleteRecord(model);
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

        protected void Close()
        {
            DialogService!.Close(null);
        }
    }
}