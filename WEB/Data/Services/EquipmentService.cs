using BlazorDownloadFile;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.FileModels;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Dto.StatsModels.GetModels;
using Models.Dto.StatsModels.ParamModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class EquipmentService : IEquipmentService
    {
        private IBlazorDownloadFileService DownloadService;
        private readonly HttpClient client;
        public EquipmentService(HttpClient client, IBlazorDownloadFileService DownloadService)
        {
            this.client = client;
            this.DownloadService = DownloadService;
        }
        public async Task<Equipment> AddEquipment(EquipmentDto model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Equipment", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Equipment? record = await response.Content.ReadFromJsonAsync<Equipment>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении оборудования, попробуйте позже");
                    }
                    return record;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task DeleteEquipment(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Equipment", query!);

                var response = await client.DeleteAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task<Equipment> GetEquipmentById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Equipment/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Equipment? record = await response.Content.ReadFromJsonAsync<Equipment>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации об оборудовании, попробуйте позже");
                    }
                    return record;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации об оборудовании, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при получении информации об оборудовании, попробуйте позже");
            }
        }

        public async Task<EquipmentDtoGetModel> GetEquipment(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("Equipment", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    EquipmentDtoGetModel? records = await response.Content.ReadFromJsonAsync<EquipmentDtoGetModel>();
                    if (records == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе оборудования, попробуйте позже");
                    }
                    return records;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе оборудования, попробуйте позже");
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

        public async Task<Equipment> UpdateEquipment(EquipmentDto model, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("Equipment", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Equipment? record = await response.Content.ReadFromJsonAsync<Equipment>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении оборудования, попробуйте позже");
                    }
                    return record;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
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

        public async Task<List<EquipmentTypesStatsModel>> GetTypesStats(DateQuery query, Guid? id = null)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);

                if (id != null)
                {
                    uriquery.Add("id", id.ToString()!);
                }


                var uri = QueryHelpers.AddQueryString("Equipment/Stats", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    List<EquipmentTypesStatsModel>? stats = await response.Content.ReadFromJsonAsync<List<EquipmentTypesStatsModel>>();
                    if (stats == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе статистики, попробуйте позже");
                    }
                    return stats;
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
                    throw new AppException("Ошибка запроса", "Ошибка при запросе статистики, попробуйте позже");
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

        public async Task ExportEquipment(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);
                var uri = QueryHelpers.AddQueryString("Equipment/Export", uriquery!);
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
    }
}
