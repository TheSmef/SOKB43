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
using MudBlazor;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;

namespace WEB.Pages.Admin
{
    public partial class AdminPage
    {
        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private IBackUpService? BackUpService { get; set; }

        private async Task GetBackUpFile()
        {
            try
            {
                await BackUpService!.BackUpDatabase();
                NotificationService!.Notify(NotificationSeverity.Success, "�������� �������� ��������� ����� ���� ������!", "��������� ����� ���� ������ ���� ���������", 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>() { "�������������" }))
                {
                    await GetBackUpFile();
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

        private async Task RestoreDatabase(IBrowserFile file)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                await file.OpenReadStream(file.Size).CopyToAsync(ms);
                await BackUpService!.RestoreDatabase(ms.ToArray());
                NotificationService!.Notify(NotificationSeverity.Success, "�������� ��������������!", "���� ������ ������� �������������", 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>() { "�������������" }))
                {
                    await RestoreDatabase(file);
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
    }
}