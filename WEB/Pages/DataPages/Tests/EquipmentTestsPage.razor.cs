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
using System.Security.Claims;
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Utility;
using WEB.Data.Services;

namespace WEB.Pages.DataPages.Tests
{
    public partial class EquipmentTestsPage
    {
        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<TechnicalTest> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditTest(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteTest(args.Data);
                    break;
                }
            }
        }

        private RadzenDataGrid<Equipment>? grid;
        private RadzenDataGrid<TechnicalTest>? childgrid;
        private EquipmentDtoGetModel? records = new EquipmentDtoGetModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private TechnicalTestsGetDtoModel? tests = new TechnicalTestsGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private QuerySupporter query = new QuerySupporter();
        private QuerySupporter childquery = new QuerySupporter();
        [Inject]
        private IEquipmentService? EquipmentService { get; set; }

        [Inject]
        private ITechnicalTestsService? TechnicalTestsService { get; set; }

        [Inject]
        private NotificationService? NotificationService { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        [Inject]
        private IMapper? Mapper { get; set; }

        private async Task RowExpanded(Equipment model)
        {
            tests = new TechnicalTestsGetDtoModel()
            {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
            LoadDataArgs args = new LoadDataArgs()
            {Skip = 0, Top = 5};
            await LoadChildData(args, model);
        }

        private async Task CheckBin(Equipment model)
        {
            await DialogService!.OpenAsync<TestBinPage>(ConstantValues.TEST_BIN_TITLE, new Dictionary<string, object>()
            {{ConstantValues.EQ, model}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true, Width = "800px", Resizable = true});
            await childgrid!.Reload();
        }

        private async Task ExportTests()
        {
            try
            {
                await TechnicalTestsService!.ExportTests(childquery);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешный экспорт тестирования!", "Тестирования успешно экспортированы", 4000);
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await ExportTests();
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

        private async Task ImportExcel(IBrowserFile file, Guid id)
        {
            try
            {
                MemoryStream ms = new MemoryStream();
                await file.OpenReadStream(file.Size).CopyToAsync(ms);
                await TechnicalTestsService!.ImportTests(ms.ToArray(), id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное импортирование!", "Тестирования успешно импортированы", 4000);
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>() { "Администратор", "Отдел тестирования" }))
                {
                    await ImportExcel(file, id);
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

        private async Task AddTest(Equipment model)
        {
            await DialogService!.OpenAsync<TestAddPage>(ConstantValues.TESTADD_TITLE, new Dictionary<string, object>()
            {{ConstantValues.EQ, model}}, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await childgrid!.Reload();
        }

        private async Task EditTest(TechnicalTest model)
        {
            try
            {
                TechnicalTest record = await TechnicalTestsService!.GetTestById(model.Id);
                if (record == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await childgrid!.Reload();
                    return;
                }

                await DialogService!.OpenAsync<TestEditPage>(ConstantValues.TESTEDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.TEST, record}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await childgrid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await EditTest(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task ChangeStatus(Equipment model)
        {
            try
            {
                Equipment eq = await EquipmentService!.GetEquipmentById(model.Id);
                EquipmentDto eqDto = Mapper!.Map<EquipmentDto>(eq);
                eqDto.Status = model.Status;
                eqDto.OrderId = eq.Order!.Id;
                eqDto.TechnicalTaskId = eq.TechnicalTask!.Id;
                await EquipmentService!.UpdateEquipment(eqDto, eq.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное изменение!", "Статус оборудование успешно изменено", 4000);
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await ChangeStatus(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await grid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }

        private async Task LoadChildData(LoadDataArgs args, Equipment model)
        {
            try
            {
                childquery = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Equipment.Id)) ==  " + "\"" + model.Id.ToString() + "\") and ((np(Deleted)) == false)" : args.Filter + " and ((np(Equipment.Id)) == " + "\"" + model.Id.ToString() + "\") and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                tests = await TechnicalTestsService!.GetTests(childquery);
                if (tests!.Collection!.Count == 0 && tests!.CurrentPageIndex != 1)
                {
                    await childgrid!.GoToPage(tests!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await LoadChildData(args, model);
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

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = string.IsNullOrEmpty(args.Filter) ? "((np(Deleted)) == false)" : args.Filter + " and ((np(Deleted)) == false)", OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                records = await EquipmentService!.GetEquipment(query);
                if (records!.Collection!.Count == 0 && records!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(records!.CurrentPageIndex - 2);
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

        private async Task DeleteTest(TechnicalTest model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    TechnicalTest testCheck = await TechnicalTestsService!.GetTestById(model.Id);
                    TechnicalTestDto dto = Mapper!.Map<TechnicalTestDto>(testCheck);
                    dto.EquipmentId = testCheck.Equipment!.Id;
                    dto.Deleted = true;
                    await TechnicalTestsService!.UpdateTest(dto, model.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Запись успешно удалена!", "Тестирование помещено в корзину", 4000);
                    await childgrid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(new List<string>()
                {"Администратор", "Отдел тестирования"}))
                {
                    await DeleteTest(model);
                }
                else
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка доступа, вы не имеете доступ к данной функции", 4000);
                }
            }
            catch (AppException e)
            {
                NotificationService!.Notify(NotificationSeverity.Error, e.Title, e.Message, 4000);
                await childgrid!.Reload();
            }
            catch
            {
                NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла неизвестная ошибка при запросе, попробуйте повторить запрос позже", 4000);
            }
        }
    }
}