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
using WEB.Pages.DataPages.Contractors.Orders;
using WEB.Pages.DataPages.Contractors.Orders.Equipments;
using WEB.Utility;

namespace WEB.Pages.DataPages.Contractors
{
    public partial class ContractorsPage
    {
        private RadzenDataGrid<Contractor>? grid;
        private RadzenDataGrid<Order>? childgrid;
        private OrderGetDtoModel? orders = new OrderGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private ContractorsGetDtoModel? records = new ContractorsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IContractorService? ContractorService { get; set; }

        [Inject]
        private IOrderService? OrderService { get; set; }

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

        private async Task RowExpended(Contractor record)
        {
            orders = new OrderGetDtoModel()
            {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
            LoadDataArgs args = new LoadDataArgs()
            {Skip = 0, Top = 5};
            await LoadChildData(args, record);
        }

        private async Task EditOrder(Order model)
        {
            try
            {
                Order order = await OrderService!.GetOrderById(model.Id);
                if (order == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ ��� �������, ������ �����������", 4000);
                    await childgrid!.Reload();
                    return;
                }

                await DialogService!.OpenAsync<OrdersEditPage>(ConstantValues.ORDEREDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.ORDEREDIT, order}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await EditOrder(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task AddOrder(Contractor contractor)
        {
            await DialogService!.OpenAsync<OrdersAddPage>(ConstantValues.ORDERADD_TITLE, new Dictionary<string, object>()
            {{ConstantValues.CONTRACTOR, contractor}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await childgrid!.Reload();
        }

        private async Task DeleteOrder(Order order)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await OrderService!.DeleteOrder(order.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "�������� ��������!", "����� ������� �����", 4000);
                    await childgrid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await DeleteOrder(order);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task LoadChildData(LoadDataArgs args, Contractor contractor)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Contractor.Id)) ==  " + "\"" + contractor.Id.ToString() + "\")" : args.Filter + " and ((np(Contractor.Id)) == " + "\"" + contractor.Id.ToString() + "\")", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                orders = await OrderService!.GetOrders(query);
                if (orders!.Collection!.Count == 0 && orders!.CurrentPageIndex != 1)
                {
                    await childgrid!.GoToPage(orders!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "����� ������"}))
                {
                    await LoadChildData(args, contractor);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task ChildContextMenuSelector(DataGridCellMouseEventArgs<Order> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditOrder(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteOrder(args.Data);
                    break;
                }

                case 3:
                {
                    await CheckEquipment(args.Data);
                    break;
                }
            }
        }

        private async Task CheckEquipment(Order model)
        {
            try
            {
                await DialogService!.OpenAsync<EquipmentChildPage>(ConstantValues.EQ_TITLE, new Dictionary<string, object>()
                {{ConstantValues.ORDER, model}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true, Width = "800px", Resizable = true });
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await EditOrder(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<Contractor> args, MenuItemEventArgs value)
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
                records = await ContractorService!.GetContractors(query);
                if (records!.Collection!.Count == 0 && records!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(records!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await LoadData(args);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task AddRecord()
        {
            await DialogService!.OpenAsync<ContractorAddPage>(ConstantValues.CONTRACTORADD_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task EditRecord(Contractor data)
        {
            try
            {
                Contractor record = await ContractorService!.GetContractorById(data.Id);
                if (record == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ ��� �������, ������ �����������", 4000);
                    await grid!.Reload();
                    return;
                }

                await DialogService!.OpenAsync<ContractorEditPage>(ConstantValues.CONTRACTOREDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.CONTRACTOR, record}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await grid!.Reload();
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await EditRecord(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }

        private async Task DeleteRecord(Contractor data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await ContractorService!.DeleteContractor(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "�������� ��������!", "���������� ������� �����", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "�������� �� ������ � ���������"}))
                {
                    await DeleteRecord(data);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ������ �������, �� �� ������ ������ � ������ �������", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "��������� ����������� ������ ��� �������, ���������� ��������� ������ �����", 4000);
            }
        }
    }
}