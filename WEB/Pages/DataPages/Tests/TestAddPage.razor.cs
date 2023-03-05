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
using System.Security.Claims;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Tests
{
    public partial class TestAddPage
    {
        [Parameter]
        public Equipment? equipment { get; set; }

        private TechnicalTestDto test = new TechnicalTestDto();
        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private ITechnicalTestsService? TestService { get; set; }

        protected override void OnInitialized()
        {
            try
            {
                if (equipment == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Оборудование не было задано", 4000);
                    Close();
                }

                test.EquipmentId = equipment!.Id;
                test.Deleted = false;
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при подготовке запроса", 4000);
                Close();
            }
        }

        private async Task HandleAdd()
        {
            try
            {
                await TestService!.AddTest(test);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Тестирование успешно добавлено", 4000);
                Close();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Отдел тестирования"}))
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