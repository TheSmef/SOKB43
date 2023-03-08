using Models.Dto.GetModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.QuerySupporter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WEB.Data.Services;
using WEB.Data.Services.Base;
using WEB.Tests.Constants;
using WEB.Tests.Orderer;
using WEB.Tests.Tests.Stubs;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class ProfileServiceTests
    {
        private readonly IProfileService service;
        private readonly HttpClient client;
        public ProfileServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new ProfileService(client, new LocalStorageStub(TestData.TOKEN), new AuthProviderStub());
        }


        [Fact]
        [TestPriority(1)]
        public async Task getProfile_Passed()
        {
            try
            {
                User user = await service.GetProfile();
                if (user == null)
                {
                    Assert.Fail("Пользователь пуст");
                }
            }
            catch (Exception ex) 
            { 
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(1)]
        public async Task getTokens_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter{ Skip = 0, Top = 5};
                TokensGetDtoModel records = await service.GetTokens(query);
                if (records == null)
                {
                    Assert.Fail("Токены отсутствуют");
                }
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(1)]
        public async Task getTokens_Fail()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5, Filter = "22312" };
                TokensGetDtoModel records = await service.GetTokens(query);
                Assert.Fail("Должна была выйти ошибка на неверный ввод параметров");
            }
            catch {}
        }

        [Fact]
        [TestPriority(1)]
        public async Task LogOut_Passed()
        {
            try
            {
                await service.LogOut();
            }
            catch(Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(2)]
        public async Task UpdateProfile_Passed()
        {
            try
            {
                await service.UpdateProfile(new UpdateModel
                {
                    First_name = "Админ",
                    Last_name = "Админ",
                    Otch = "Админ",
                    BirthDate = DateTime.Today.AddYears(-30),
                    PassportSeries = "0000",
                    PassportNumber = "000000",
                    PhoneNumber = "88888888888",
                    Email = "admin@admin.com",
                    Login = "admin",
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(3)]
        public async Task UpdateProfile_Fail()
        {
            try
            {
                await service.UpdateProfile(new UpdateModel
                {
                    First_name = "Админ",
                    Last_name = "Админ",
                    Otch = "Админ",
                    PhoneNumber = "88888888888",
                    Email = "admin@admin.com",
                    Login = "admin",
                });
                Assert.Fail("Должна быть ошибка валидации");
            }
            catch { }
        }

        [Fact]
        [TestPriority(5)]
        public async Task DeleteToken_Fail()
        {
            try
            {
                await service.deleteToken("");
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(6)]
        public async Task DeleteToken_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                TokensGetDtoModel records = await service.GetTokens(query);
                if (records == null)
                {
                    Assert.Fail("Токены отсутствуют");
                }
                await service.deleteToken(records.Collection!.Where(x => x.TokenStr != TestData.TOKEN).First().TokenStr);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
