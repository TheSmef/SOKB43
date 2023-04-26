using BlazorDownloadFile;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.FileModels;
using Models.Dto.GetModels;
using Radzen;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class BackUpService : IBackUpService
    {
        private readonly HttpClient client;
        private IBlazorDownloadFileService DownloadService;
        public BackUpService(HttpClient client, IBlazorDownloadFileService DownloadService)
        {
            this.client = client;
            this.DownloadService = DownloadService;
        }
        public async Task BackUpDatabase()
        {
            try
            {
                var response = await client.GetAsync("BackUp");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FileModel? record = await response.Content.ReadFromJsonAsync<FileModel>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе резервной копии базы данных, попробуйте позже");
                    }
                    await DownloadService.DownloadFile(record.Name, record.Data, "application/ostet-stream");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
                }

            }
            catch (AppException)
            {
                throw;
            }
            catch (UnAuthException)
            {
                throw;
            }
            catch
            {
                throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
            }
        }

        public async Task RestoreDatabase(byte[] data)
        {
            try
            {
                var response = await client.PutAsJsonAsync("BackUp", data);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе контрагентов, попробуйте позже");
                }

            }
            catch (AppException)
            {
                throw;
            }
            catch (UnAuthException)
            {
                throw;
            }
            catch
            {
                throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
            }
        }
    }
}
