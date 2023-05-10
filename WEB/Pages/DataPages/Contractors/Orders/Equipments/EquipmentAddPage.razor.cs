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
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Contractors.Orders.Equipments
{
    public partial class EquipmentAddPage
    {
        [Inject]
        private IEquipmentService? EquipmentService { get; set; }


        [Inject]
        private ITechnicalTaskService? TaskService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        [Parameter]
        public Order? order { get; set; }

        private string roles = "Администратор, Менеджер по работе с клиентами";

        private TechnicalTasksGetDtoModel tasks = new TechnicalTasksGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        private List<string> statuses = new List<string>();
        private EquipmentDto equipment = new EquipmentDto();
        protected override void OnInitialized()
        {
            if (order != null)
            {
                equipment.OrderId = order.Id;
                equipment.Deleted = false;
            }
            else
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Не был задан заказ для оборудования", 4000);
                Close();
            }

        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? args.Filter : "(np(NameEquipment).Contains(" + "\"" + args.Filter + "\"))", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                tasks = await TaskService!.GetTechnicalTasks(query);
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
                await EquipmentService!.AddEquipment(equipment);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Оборудование успешно добавлено", 4000);
                Close();
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