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
using Models.Dto.PostPutModels;
using Models.Entity;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Contractors.Orders
{
    public partial class OrdersAddPage
    {
        [Inject]
        private IOrderService? OrderService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Parameter]
        public Contractor? contractor { get; set; }

        private OrderDto order = new OrderDto();

        private string roles = "Администратор, Менеджер по работе с клиентами";
        protected override void OnInitialized()
        {
            if (contractor != null)
            {
                order.ContractorId = contractor!.Id;
            }
            else
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Не был задан контрагент для заказа", 4000);
                Close();
            }

        }

        private async Task HandleAdd()
        {
            try
            {
                await OrderService!.AddOrder(order);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное добавление!", "Заказ успешно добавлен", 4000);
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