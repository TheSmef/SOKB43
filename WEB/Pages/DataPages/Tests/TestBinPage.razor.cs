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

namespace WEB.Pages.DataPages.Tests
{
    public partial class TestBinPage
    {
        protected override void OnInitialized()
        {
            if (equipment == null)
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Ошибка загрузки данных, оборудование не было задано", 4000);
                Close();
            }
        }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<TechnicalTest> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await RestoreTest(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteTest(args.Data);
                    break;
                }
            }
        }

        private RadzenDataGrid<TechnicalTest>? grid;
        private QuerySupporter query = new QuerySupporter();
        private TechnicalTestsGetDtoModel? bin = new TechnicalTestsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        [Inject]
        private ITechnicalTestsService? TechnicalTestsService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        [Parameter]
        public Equipment? equipment { get; set; }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Equipment.Id)) ==  " + "\"" + equipment!.Id.ToString() + "\") and ((np(Deleted)) == true)" : args.Filter + " and ((np(Equipment.Id)) == " + "\"" + equipment!.Id.ToString() + "\") and ((np(Deleted)) == true)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                bin = await TechnicalTestsService!.GetTests(query);
                if (bin!.Collection!.Count == 0 && bin!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(bin!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
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

        private async Task RestoreTest(TechnicalTest model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.RESTORE_RECORD, ConstantValues.RESTORE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    TechnicalTest test = await TechnicalTestsService!.GetTestById(model.Id);
                    TechnicalTestDto dto = Mapper!.Map<TechnicalTestDto>(test);
                    dto.EquipmentId = test.Equipment!.Id;
                    dto.Deleted = false;
                    await TechnicalTestsService!.UpdateTest(dto, test.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно востановлена!", "Тестирование восстановлено", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await RestoreTest(model);
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

        private async Task DeleteTest(TechnicalTest data)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await TechnicalTestsService!.DeleteTest(data.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Тестирование успешно удалено", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await DeleteTest(data);
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