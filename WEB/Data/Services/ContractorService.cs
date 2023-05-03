using BlazorDownloadFile;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.FileModels;
using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class ContractorService : IContractorService
    {
        private IBlazorDownloadFileService DownloadService;
        private readonly HttpClient client;
        public ContractorService(HttpClient client, IBlazorDownloadFileService DownloadService)
        {
            this.client = client;
            this.DownloadService = DownloadService;
        }
        public async Task<Contractor> AddContractor(Contractor model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Contractors", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Contractor? record = await response.Content.ReadFromJsonAsync<Contractor>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении контрагента, попробуйте позже");
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

        public async Task DeleteContractor(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Contractors", query!);

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

        public async Task ExportContractors(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);
                var uri = QueryHelpers.AddQueryString("Contractors/Export", uriquery!);
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

        public async Task<Contractor> GetContractorById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Contractors/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Contractor? record = await response.Content.ReadFromJsonAsync<Contractor>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации о контрагента, попробуйте позже");
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
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации о контрагенте, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при запросе контрагента, попробуйте позже");
            }
        }

        public async Task<ContractorsGetDtoModel> GetContractors(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("Contractors", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ContractorsGetDtoModel? records = await response.Content.ReadFromJsonAsync<ContractorsGetDtoModel>();
                    if (records == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе контрагентов, попробуйте позже");
                    }
                    return records;
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

        public async Task<Contractor> UpdateContractor(Contractor model)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", model.Id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("Contractors", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Contractor? record = await response.Content.ReadFromJsonAsync<Contractor>();
                    if (record == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении контрагента, попробуйте позже");
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

        public async Task ImportContractors(byte[] data)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Contractors/Import", data);

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
