using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class PostService : IPostService
    {
        private readonly HttpClient client;
        public PostService(HttpClient client)
        {
            this.client = client;
        }

        public async Task<Post> AddPost(Post model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Posts", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Post? post = await response.Content.ReadFromJsonAsync<Post>();
                    if (post == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при добавлении должности, попробуйте позже");
                    }
                    return post;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
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

        public async Task DeletePost(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Posts", query!);

                var response = await client.DeleteAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
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

        public async Task<Post> GetPostById(Guid id)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", id.ToString() }
                };

                var uri = QueryHelpers.AddQueryString("Posts/single", query!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Post? post = await response.Content.ReadFromJsonAsync<Post>();
                    if (post == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при получении информации о должности, попробуйте позже");
                    }
                    return post;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthException();
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка запроса", await response.Content.ReadAsStringAsync());
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при получении информации о должности, попробуйте позже");
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
                throw new AppException("Ошибка запроса", "Ошибка при запросе должности, попробуйте позже");
            }
        }

        public async Task<PostsGetDtoModel> GetPosts(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("Posts", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    PostsGetDtoModel? posts = await response.Content.ReadFromJsonAsync<PostsGetDtoModel>();
                    if (posts == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе должностей, попробуйте позже");
                    }
                    return posts;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе должностей, попробуйте позже");
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

        public async Task<Post> UpdatePost(Post model)
        {
            try
            {
                var query = new Dictionary<string, string>
                {
                        { "id", model.Id.ToString() }
                };
                var uri = QueryHelpers.AddQueryString("Posts", query!);
                var response = await client.PutAsJsonAsync(uri, model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Post? post = await response.Content.ReadFromJsonAsync<Post>();
                    if (post == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении должности, попробуйте позже");
                    }
                    return post;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
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
