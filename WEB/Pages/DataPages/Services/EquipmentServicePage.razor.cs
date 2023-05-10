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
using WEB.Data.Services;

namespace WEB.Pages.DataPages.Services
{
    public partial class EquipmentServicePage
    {
        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<Service> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                    {
                        await EditService(args.Data);
                        break;
                    }

                case 2:
                    {
                        await DeleteService(args.Data);
                        break;
                    }

                case 3:
                    {
                        await GetServiceDocument(args.Data);
                        break;
                    }
            }
        }

        private string roles = "Администратор, Менеджер по работе с клиентами, Отдел обслуживания";

        private RadzenDataGrid<Service>? childgrid;
        private RadzenDataGrid<Equipment>? grid;
        private EquipmentDtoGetModel? records = new EquipmentDtoGetModel()
        { CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0 };
        private ServiceGetDtoModel? service = new ServiceGetDtoModel()
        { CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0 };
        private QuerySupporter query = new QuerySupporter();
        private QuerySupporter childquery = new QuerySupporter();
        [Inject]
        private IEquipmentService? EquipmentService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        [Inject]
        private IServicesService? ServicesService { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        private async Task RowExpanded(Equipment model)
        {
            service = new ServiceGetDtoModel()
            { CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0 };
            LoadDataArgs args = new LoadDataArgs()
            { Skip = 0, Top = 5 };
            await LoadChildData(args, model);
        }

        private async Task GetServiceDocument(Service model)
        {
            try
            {
                await ServicesService!.GetWordDocument(model.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное составление договора!", "Договор успешно составлен", 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await GetServiceDocument(model);
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

        private async Task ImportExcel(IBrowserFile file, Guid id)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                await file.OpenReadStream(file.Size).CopyToAsync(ms);
                await ServicesService!.ImportServices(ms.ToArray(), id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное импортирование!", "Обслуживания успешно импортированы", 4000);
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await ImportExcel(file, id);
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

        private async Task LoadChildData(LoadDataArgs args, Equipment model)
        {
            try
            {
                childquery = new QuerySupporter { Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Equipment.Id)) ==  " + "\"" + model.Id.ToString() + "\") and ((np(Deleted)) == false)" : args.Filter + " and ((np(Equipment.Id)) == " + "\"" + model.Id.ToString() + "\") and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value };
                service = await ServicesService!.GetServices(childquery);
                if (service!.Collection!.Count == 0 && service!.CurrentPageIndex != 1)
                {
                    await childgrid!.GoToPage(records!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await LoadChildData(args, model);
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

        private async Task DeleteService(Service model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                { CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE }) == true)
                {
                    Service serviceCheck = await ServicesService!.GetServiceById(model.Id);
                    ServiceDto serviceDto = Mapper!.Map<ServiceDto>(serviceCheck);
                    serviceDto.EquipmentId = serviceCheck.Equipment!.Id;
                    serviceDto.Deleted = true;
                    await ServicesService!.UpdateService(serviceDto, model.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно удалена!", "Обслуживание помещено в корзину", 4000);
                    await childgrid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await DeleteService(model);
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

        private async Task EditService(Service model)
        {
            try
            {
                Service record = await ServicesService!.GetServiceById(model.Id);
                if (record == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await childgrid!.Reload();
                    return;
                }

                await DialogService!.OpenAsync<ServiceEditPage>(ConstantValues.SERVICEEDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.RECORD, record}}, new DialogOptions()
                { CloseDialogOnOverlayClick = true });
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await EditService(model);
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

        private async Task CheckBin(Equipment model)
        {
            await DialogService!.OpenAsync<ServiceBin>(ConstantValues.SERVICE_BIN_TITLE, new Dictionary<string, object>()
            {{ConstantValues.EQ, model}}, new DialogOptions()
            { CloseDialogOnOverlayClick = true, Width = "800px", Resizable = true });
            await childgrid!.Reload();
        }

        private async Task ExportServices()
        {
            try
            {
                await ServicesService!.ExportServices(childquery);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешный экспорт обслуживания!", "Обслуживания успешно экспортированы", 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await ExportServices();
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

        private async Task AddService(Equipment model)
        {
            await DialogService!.OpenAsync<ServiceAddPage>(ConstantValues.SERVICEADD_TITLE, new Dictionary<string, object>()
            {{ConstantValues.EQ, model}}, new DialogOptions()
            { CloseDialogOnOverlayClick = true });
            await childgrid!.Reload();
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter { Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Deleted)) == false)" : args.Filter + " and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value };
                records = await EquipmentService!.GetEquipment(query);
                if (records!.Collection!.Count == 0 && records!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(records!.CurrentPageIndex - 2);
                }
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
    }
}