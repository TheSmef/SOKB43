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
using Models.Dto.StatsModels.GetModels;
using Models.Dto.StatsModels.ParamModels;
using Models.Entity;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.DataPages.Statistic
{
    public partial class StatsPage
    {
        private List<EquipmentTypesStatsModel> typestats = new List<EquipmentTypesStatsModel>();
        private List<IncomeStatsModel> incomestats = new List<IncomeStatsModel>();
        private DateQuery query = new DateQuery();
        [Parameter]
        public Contractor? Contractor { get; set; }

        [Inject]
        private IOrderService? OrderService { get; set; }

        [Inject]
        private IEquipmentService? EquipmentService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await Load();
        }

        private async Task Load()
        {
            try
            {
                if (Contractor == null)
                {
                    typestats = await EquipmentService!.GetTypesStats(query);
                    incomestats = await OrderService!.GetIncomeStats(query);
                }
                else
                {
                    typestats = await EquipmentService!.GetTypesStats(query, Contractor.Id);
                    incomestats = await OrderService!.GetIncomeStats(query, Contractor.Id);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>() { "Администратор", "Менеджер по работе с клиентами" }))
                {
                    await Load();
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