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
using Models.Entity;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;
using System.Diagnostics.Contracts;

namespace WEB.Pages.DataPages.EquipmentTypes
{
    public partial class EquipmentTypesEditPage
    {
        [Inject]
        private IEquipmentTypeService? TypeService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Parameter]
        public TypeEquipment? type { get; set; }

        protected override void OnInitialized()
        {
            if (type == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "������!", "������ �������� ������, ��� ������������ �� ��� �����", 4000);
                Close();
                return;
            }
        }

        private async Task HandleEdit()
        {
            try
            {
                await TypeService!.UpdateType(type!);
                NotificationService!.Notify(NotificationSeverity.Success, "�������� ���������!", "��� ������������ ������� ������", 4000);
                Close();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"�������������", "����������� ��������"}))
                {
                    await HandleEdit();
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

        protected void Close()
        {
            DialogService!.Close(null);
        }
    }
}