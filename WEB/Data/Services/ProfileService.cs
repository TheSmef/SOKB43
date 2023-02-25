using Blazored.LocalStorage;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;
using Models.Dto.GetModels;
using Models.Dto.GetModels.ProfileModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Collections.Specialized;
using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Reflection;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    [Authorize]
    public class ProfileService : IProfileService
    {

        private readonly HttpClient client;
        private readonly ILocalStorageService storage;
        private readonly AuthenticationStateProvider authenticationStateProvider;
        public ProfileService(HttpClient client, ILocalStorageService storage
            , AuthenticationStateProvider authenticationStateProvider)
        {
            this.client = client;
            this.storage = storage;
            this.authenticationStateProvider = authenticationStateProvider;
        }

        public async Task deleteToken(string token)
        {
            try
            {
                var query = new Dictionary<string, string>
                    {
                        { "token", token }
                    };

                var uri = QueryHelpers.AddQueryString("Profile/Tokens", query!);

                var response = await client.DeleteAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                {
                    if ((await storage.GetItemAsStringAsync("token")).Equals(token))
                    {
                        await storage.RemoveItemAsync("token");
                        await storage.RemoveItemAsync("jwttoken");
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                    }
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
                    throw new AppException("Ошибка удаления токена", "Ошибка при удалении токена, попробуйте позже");
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
                throw new AppException("Ошибка выхода из аккаунта", "Ошибка при выходе из аккаунта, попробуйте позже");
            }
        }

        public async Task<User> GetProfile()
        {
            try
            {

                var response = await client.GetAsync("Profile");

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    User? user = await response.Content.ReadFromJsonAsync<User>();
                    if (user == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе профиля, попробуйте позже");
                    }
                    return user;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе профиля, попробуйте позже");
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

        public async Task<TokensGetDtoModel> GetTokens(QuerySupporter query)
        {
            try
            {
                var uriquery = QueryMapper.MapToQuery(query);


                var uri = QueryHelpers.AddQueryString("Profile/Tokens", uriquery!);

                var response = await client.GetAsync(uri);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    TokensGetDtoModel? tokens = await response.Content.ReadFromJsonAsync<TokensGetDtoModel>();
                    if (tokens == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при запросе токенов доступа, попробуйте позже");
                    }
                    return tokens;
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                {
                    throw new UnAuthException();
                }
                else
                {
                    throw new AppException("Ошибка запроса", "Ошибка при запросе токенов доступа, попробуйте позже");
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

        public async Task LogOut()
        {
            try
            {
                if (await storage.ContainKeyAsync("token"))
                {
                    var query = new Dictionary<string, string>
                    {
                        { "token", await storage.GetItemAsStringAsync("token") }
                    };

                    var uri = QueryHelpers.AddQueryString("Profile/Tokens", query!);

                    var response = await client.DeleteAsync(uri);

                    if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.BadRequest)
                    {
                        await storage.RemoveItemAsync("token");
                        await storage.RemoveItemAsync("jwttoken");
                        await authenticationStateProvider.GetAuthenticationStateAsync();
                    }
                    else if (response.StatusCode == HttpStatusCode.Unauthorized || response.StatusCode == HttpStatusCode.Forbidden)
                    {
                        throw new UnAuthException();
                    }
                    else
                    {
                        throw new AppException("Ошибка выхода из аккаунта", "Ошибка при выходе из аккаунта, попробуйте позже");
                    }
                }
                else
                {
                    await storage.RemoveItemAsync("jwttoken");
                    await authenticationStateProvider.GetAuthenticationStateAsync();
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
                throw new AppException("Ошибка выхода из аккаунта", "Ошибка при выходе из аккаунта, попробуйте позже");
            }
        }

        public async Task<User> UpdateProfile(UpdateModel model)
        {
            try
            {

                var response = await client.PutAsJsonAsync("Profile", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    ProfileGetModel? user = await response.Content.ReadFromJsonAsync<ProfileGetModel>();
                    if (user == null)
                    {
                        throw new AppException("Ошибка запроса", "Ошибка при изменении профиля, попробуйте позже");
                    }
                    if (user.RefreshToken != null)
                        await storage.SetItemAsStringAsync("token", user.RefreshToken);
                    return user.User!;
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
                    throw new AppException("Ошибка запроса", "Ошибка при изменении профиля, попробуйте позже");
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
