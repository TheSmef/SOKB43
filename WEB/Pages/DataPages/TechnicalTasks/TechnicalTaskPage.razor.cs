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
using Models.Utility;
using WEB.Data.Services.Base;
using WEB.Data.UtilityServices.Base;
using WEB.Pages.DataPages.Users.UserPostsModal;
using WEB.Utility;

namespace WEB.Pages.DataPages.TechnicalTasks
{
    public partial class TechnicalTaskPage
    {
        private TechnicalTasksGetDtoModel tasks = new TechnicalTasksGetDtoModel()
        {CurrentPageIndex = 0, ElementsCount = 0, TotalPages = 0};
        private RadzenDataGrid<TechnicalTask>? grid;
        private QuerySupporter query = new QuerySupporter();
        [Inject]
        private NotificationService? NotificationService { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState>? AuthenticationStateTask { get; set; }

        [Inject]
        private IAuthInterceptor? AuthInterceptor { get; set; }

        [Inject]
        private DialogService? DialogService { get; set; }

        [Inject]
        private ContextMenuService? ContextMenuService { get; set; }

        [Inject]
        private ITechnicalTaskService? TaskService { get; set; }

        [Inject]
        private IUserPostService? UserPostService { get; set; }

        private async Task ContextMenuSelector(DataGridCellMouseEventArgs<TechnicalTask> args, MenuItemEventArgs value)
        {
            switch (value.Value)
            {
                case 1:
                {
                    await EditTask(args.Data);
                    break;
                }

                case 2:
                {
                    await DeleteTask(args.Data);
                    break;
                }
            }
        }

        private async Task LoadData(LoadDataArgs args)
        {
            try
            {
                query = new QuerySupporter{Filter = args.Filter, OrderBy = args.OrderBy, Skip = args.Skip!.Value, Top = args.Top!.Value};
                tasks = await TaskService!.GetTechnicalTasks(query);
                if (tasks!.Collection!.Count == 0 && tasks!.CurrentPageIndex != 1)
                {
                    await grid!.GoToPage(tasks!.CurrentPageIndex - 2);
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
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

        private async Task AddTask()
        {
            await DialogService!.OpenAsync<TechnicalTaskAddPage>(ConstantValues.TASKADD_TITLE, null, new DialogOptions()
            {CloseDialogOnOverlayClick = true});
            await grid!.Reload();
        }

        private async Task EditTask(TechnicalTask model)
        {
            try
            {
                TechnicalTask task = await TaskService!.GetTechnicalTaskById(model.Id);
                if (task == null)
                {
                    NotificationService!.Notify(NotificationSeverity.Error, "Ошибка!", "Произошла ошибка при запросе, данные отсутствуют", 4000);
                    await grid!.Reload();
                    return;
                }
                await DialogService!.OpenAsync<TechnicalTaskEditPage>(ConstantValues.TASKEDIT_TITLE, new Dictionary<string, object>()
                {{ConstantValues.TASK, task}}, new DialogOptions()
                {CloseDialogOnOverlayClick = true});
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
                {
                    await EditTask(model);
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

        private async Task DeleteTask(TechnicalTask model)
        {
            try
            {
                if (await DialogService!.Confirm(ConstantValues.DELETE_RECORD, ConstantValues.DELETE_RECORD_TITLE, new ConfirmOptions()
                {CloseDialogOnOverlayClick = true, CancelButtonText = ConstantValues.CANCEL, OkButtonText = ConstantValues.OK_DELETE}) == true)
                {
                    await TaskService!.DeleteTechnicalTask(model.Id);
                    NotificationService!.Notify(NotificationSeverity.Success, "Успешное удаление!", "Техническое задание успешно удалено", 4000);
                    await grid!.Reload();
                }
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
                {
                    await DeleteTask(model);
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

        private async Task SaveTaskContent(TechnicalTask model)
        {
            try
            {
                TechnicalTask task = await TaskService!.GetTechnicalTaskById(model.Id);
                TechnicalTaskDto taskDto = new TechnicalTaskDto();
                taskDto.TypeEquipmentId = task.TypeEquipment!.Id;
                taskDto.Date = task.Date;
                taskDto.NameEquipment = task.NameEquipment;
                taskDto.Content = model.Content;
                await TaskService!.UpdateTechnicalTask(taskDto, model.Id);
                NotificationService!.Notify(NotificationSeverity.Success, "Успешное изменение!", "Содержание технического задания успешно изменено", 4000);
                await grid!.Reload();
            }
            catch (UnAuthException)
            {
                if (await AuthInterceptor!.ReloadAuthState(await AuthenticationStateTask!, new List<string>()
                {"Администратор", "Технический писатель"}))
                {
                    await SaveTaskContent(model);
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
    }
}