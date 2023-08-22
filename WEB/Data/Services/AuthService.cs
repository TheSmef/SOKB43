using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Models.Dto.PostPutModels.AccountModels;
using System.Net;
using System.Net.Http.Headers;
using System.Reflection;
using WEB.Data.Services.Base;
using WEB.Utility;

namespace WEB.Data.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient client;
        private readonly ILocalStorageService storage;
        public AuthService(HttpClient client, ILocalStorageService storage)
        {
            this.client = client;
            this.storage = storage;
        }

        public async Task authUser(AuthModel model)
        {
            try
            {
                var response = await client.PutAsJsonAsync("Auth", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await storage.SetItemAsStringAsync("token", await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new AppException("Ошибка авторизации" ,"Неправильный логин или пароль!");
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
            catch
            {
                throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
            }
        }

        public async Task<string> getToken()
        {
            try
            {
                var refreshtoken = await storage.GetItemAsStringAsync("token");
                if (refreshtoken == null)
                {
                    throw new AppException("Ошибка авторизации", "Произошла ошибка авторизации, перезагрузите страницу или попробуйте позже!");
                }
                var response = await client.PutAsJsonAsync("Auth/GetToken", refreshtoken);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new AppException("Ошибка авторизации", "Неправильный логин или пароль!");
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
            catch
            {
                throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
            }
        }

        public async Task regUser(RegModel model)
        {
            try
            {
                var response = await client.PostAsJsonAsync("Auth", model);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    await storage.SetItemAsStringAsync("token", await response.Content.ReadAsStringAsync());
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new AppException("Ошибка регистрации", await response.Content.ReadAsStringAsync());
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
            catch
            {
                throw new AppException("Ошибка соединения", "Произошла неизвестная ошибка при запросе!");
            }
        }
    }
}
