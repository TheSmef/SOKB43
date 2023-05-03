using BlazorDownloadFile;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.FileModels;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class TechnicalTestsService : ITechnicalTestsService
    {
        private IBlazorDownloadFileService DownloadService;
        private readonly HttpClient client;
        public TechnicalTestsService(HttpClient client, IBlazorDownloadFileService DownloadService)
        {
            this.client = client;
            this.DownloadService = DownloadService;
        }
        public async Task<TechnicalTest> AddTest(TechnicalTestDto model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("TechnicalTests", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TechnicalTest? record = await response.Content.ReadFromJsonAsync<TechnicalTest>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении тестирования, попробуйте позже");
                    }
                    return record;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка добавления", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка добавления", "Произошла неизвестная ошибка при добавлении, попробуйте позже!");
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

        public async Task DeleteTest(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("TechnicalTests", query!);

                var response = await client.DeleteAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
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

        public async Task ExportTests(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);
                var uri = QueryHelpers.AddQueryString("TechnicalTests/Export", uriquery!);
                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    FileModel? record = await response.Content.ReadFromJsonAsync<FileModel>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при экспорте, попробуйте позже");
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

        public async Task<TechnicalTest> GetTestById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("TechnicalTests/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TechnicalTest? record = await response.Content.ReadFromJsonAsync<TechnicalTest>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации о тестировании, попробуйте позже");
                    }
                    return record;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации о тестировании, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при запросе тестирования, попробуйте позже");
            }
        }

        public async Task<TechnicalTestsGetDtoModel> GetTests(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("TechnicalTests", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TechnicalTestsGetDtoModel? records = await response.Content.ReadFromJsonAsync<TechnicalTestsGetDtoModel>();
                    if (records == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе тестирований, попробуйте позже");
                    }
                    return records;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе заказов, попробуйте позже");
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

        public async Task<TechnicalTest> UpdateTest(TechnicalTestDto model, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("TechnicalTests", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TechnicalTest? record = await response.Content.ReadFromJsonAsync<TechnicalTest>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении тестирования, попробуйте позже");
                    }
                    return record;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка изменения", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка изменения", "Произошла неизвестная ошибка при изменении, попробуйте позже!");
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

        public async Task ImportTests(byte[] data, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("TechnicalTests/Import", query!);
                var response = await client.PostAsJsonAsync(uri, data);
                var a = response.Content.ReadAsStringAsync();

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
                    throw new AppException("Ошибка запроса", "Ошибка при импортировании, попробуйте позже");
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
