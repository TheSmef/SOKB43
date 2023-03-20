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
using Models.Dto.PostPutModels.AccountModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Services
{
    public partial class ServiceEditPage
    {
        [Parameter]
        public Service? record { get; set; }

        private ServiceDto service = new ServiceDto();
        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IServicesService? ServicesService { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        protected override void OnInitialized()
        {
            if (record == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Обслуживание не было задано", 4000);
                Close();
            }

            service = Mapper!.Map<ServiceDto>(record);
            service.EquipmentId = record!.Equipment!.Id;
        }

        private async Task HandleAdd()
        {
            try
            {
                await ServicesService!.UpdateService(service, record!.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное изменение!", "Обслуживание успешно изменено", 4000);
                Close();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Менеджер по работе с клиентами", "Отдел обслуживания"}))
                {
                    await HandleAdd();
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

        private void Close()
        {
            DialogService!.Close(null);
        }
    }
}