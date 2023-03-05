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
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Services
{
    public partial class ServiceBin
    {
        protected override void OnInitialized()
        {
            if (equipment == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Ошибка загрузки данных, оборудование не было задано", 4000);
                Close();
            }
        }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<Service> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await RestoreRecord(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteRecord(args.Data);
                    break;
                }
            }
        }

        private RadzenDataGrid<Service>? grid;
        private ServiceGetDtoModel? bin = new ServiceGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IServicesService? ServicesService { get; set; }

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
        private IMapper? Mapper { get; set; }

        [Parameter]
        public Equipment? equipment { get; set; }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Equipment.Id)) ==  " + "\"" + equipment!.Id.ToString() + "\") and ((np(Deleted)) == true)" : args.Filter + " and ((np(Equipment.Id)) == " + "\"" + equipment!.Id.ToString() + "\") and ((np(Deleted)) == true)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                bin = await ServicesService!.GetServices(query);
                if (bin!.Collection!.Count == 0 && bin!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(bin!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами", "Отдел обслуживания"}))
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

        private async Task RestoreRecord(Service model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.RESTORE_RECORD, ConstantValues.RESTORE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    ServiceDto serviceDto = Mapper!.Map<ServiceDto>(model);
                    serviceDto.EquipmentId = model!.Equipment!.Id;
                    serviceDto.Deleted = false;
                    await ServicesService!.UpdateService(serviceDto, model.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно востановлена!", "Обслуживание восстановленно", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами", "Отдел обслуживания"}))
                {
                    await RestoreRecord(model);
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

        private async Task DeleteRecord(Service data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await ServicesService!.DeleteService(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Обслуживание успешно удалено", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Менеджер по работе с клиентами", "Отдел обслуживания"}))
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