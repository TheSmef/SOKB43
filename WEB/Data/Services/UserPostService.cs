using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class UserPostService : IUserPostService
    {
        private readonly HttpClient client;
        public UserPostService(HttpClient client)
        {
            this.client = client;
        }
        public async Task<UserPost> AddUserPost(UserPostDto model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("UserPosts", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserPost? userPost = await response.Content.ReadFromJsonAsync<UserPost>();
                    if (userPost == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении должности сотруднику, попробуйте позже");
                    }
                    return userPost;
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

        public async Task DeleteUserPost(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("UserPosts", query!);

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

        public async Task<UserPost> GetUserPostById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("UserPosts/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserPost? userPost = await response.Content.ReadFromJsonAsync<UserPost>();
                    if (userPost == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации о должности сотрудника, попробуйте позже");
                    }
                    return userPost;
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
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации о должности сотрудника, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при запросе информации о сотруднике, попробуйте позже");
            }
        }

        public async Task<UserPostsGetDtoModel> GetUserPosts(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("UserPosts", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserPostsGetDtoModel? userPosts = await response.Content.ReadFromJsonAsync<UserPostsGetDtoModel>();
                    if (userPosts == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе должностей сотрудника, попробуйте позже");
                    }
                    return userPosts;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе должностей сотрудника, попробуйте позже");
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

        public async Task<UserPost> UpdateUserPost(UserPostDto model, Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("UserPosts", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    UserPost? userPost = await response.Content.ReadFromJsonAsync<UserPost>();
                    if (userPost == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении должности сотрудника, попробуйте позже");
                    }
                    return userPost;
                }
                else if(response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка изменении", await response.Content.ReadAsStringAsync());
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
    }
}
