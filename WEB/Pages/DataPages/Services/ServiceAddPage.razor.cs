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
    public partial class ServiceAddPage
    {
        [Parameter]
        public Equipment? equipment { get; set; }

        private ServiceDto service = new ServiceDto();

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IServicesService? ServicesService { get; set; }

        protected override void OnInitialized()
        {
            if (equipment == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Оборудование не было задано", 4000);
                Close();
            }

            service.EquipmentId = equipment!.Id;
        }

        private async Task HandleAdd()
        {
            try
            {
                await ServicesService!.AddService(service);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Обслужитвание успешно добавлено", 4000);
                Close();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
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