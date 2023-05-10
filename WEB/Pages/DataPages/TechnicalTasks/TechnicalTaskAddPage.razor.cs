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
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.TechnicalTasks
{
    public partial class TechnicalTaskAddPage
    {
        private EquipmentTypesGetDtoModel? types = new EquipmentTypesGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private IEquipmentTypeService? TypeService { get; set; }

        [Inject]
        private ITechnicalTaskService? TaskService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        private TechnicalTaskDto task = new TechnicalTaskDto();

        private string roles = "Администратор, Технический писатель";
        protected override void OnInitialized()
        {
            task.Content = "Содержание технического задания";
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? args.Filter : "(np(Name).Contains(" + "\"" + args.Filter + "\"))", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                types = await TypeService!.GetTypes(query);
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

        private async Task HandleAdd()
        {
            try
            {
                await TaskService!.AddTechnicalTask(task);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Техническое задание успешно добавлено", 4000);
                Close();
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(roles.Split(", ").ToList()))
                {
                    await HandleAdd();
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
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