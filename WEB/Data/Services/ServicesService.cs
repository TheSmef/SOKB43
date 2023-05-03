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
    public class ServicesService : IServicesService
    {
        private IBlazorDownloadFileService DownloadService;
        private readonly HttpClient client;
        public ServicesService(HttpClient client, IBlazorDownloadFileService DownloadService)
        {
            this.client = client;
            this.DownloadService = DownloadService;
        }

        public async Task<Service> AddService(ServiceDto model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Services", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Service? record = await response.Content.ReadFromJsonAsync<Service>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении обслуживания, попробуйте позже");
                    }
                    return record;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task DeleteService(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Services", query!);

                var response = await client.DeleteAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task ExportServices(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);
                var uri = QueryHelpers.AddQueryString("Services/Export", uriquery!);
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

        public async Task<Service> GetServiceById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Services/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Service? record = await response.Content.ReadFromJsonAsync<Service>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации об обслуживании, попробуйте позже");
                    }
                    return record;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации об обслуживании, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при запросе обслуживания, попробуйте позже");
            }
        }

        public async Task<ServiceGetDtoModel> GetServices(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("Services", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ServiceGetDtoModel? records = await response.Content.ReadFromJsonAsync<ServiceGetDtoModel>();
                    if (records == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе обслуживающих работ, попробуйте позже");
                    }
                    return records;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе обслуживающих работ, попробуйте позже");
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

        public async Task<Service> UpdateService(ServiceDto model, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("Services", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Service? record = await response.Content.ReadFromJsonAsync<Service>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении обслуживания, попробуйте позже");
                    }
                    return record;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task ImportServices(byte[] data, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("Services/Import", query!);
                var response = await client.PostAsJsonAsync(uri, data);

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
