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
    public class AuthServiceTests
    {
        private readonly IAuthService service;
        private readonly IUserService serviceForDelete;
        private readonly HttpClient client;
        public AuthServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            service = new AuthService(client, new LocalStorageStub(TestData.TOKEN));
            serviceForDelete = new UserService(client, new DownloadServiceStub());
        }

        [Fact]
        [TestPriority(1)]
        public async Task authUser_Admin()
        {
            try
            {
                AuthModel model = new AuthModel { Login = "admin", Password = "admin" };
                await service.authUser(model);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task getToken_Passed()
        {
            try
            {
                await service.getToken();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(3)]
        public async Task regUser_User_Pass()
        {
            try
            {
                RegModel model = new RegModel 
                { 
                    Login = "Test", 
                    Password = "Qwerty123!", 
                    PassportNumber = "888888", 
                    PassportSeries = "8888", 
                    BirthDate = DateTime.Today.AddYears(-40), 
                    Email = "example@test.test", 
                    First_name = "Тест",
                    Last_name = "Тест",
                    PasswordConfirm = "Qwerty123!", 
                    PhoneNumber = "89772341212" 
                };
                await service.regUser(model);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(4)]
        public async Task regUser_User_Fail()
        {
            try
            {
                RegModel model = new RegModel
                {
                    Login = "admin",
                    Password = "Qwerty123!",
                    PassportNumber = "888888",
                    PassportSeries = "8888",
                    BirthDate = DateTime.Today.AddYears(-40),
                    Email = "example@test.test",
                    First_name = "Тест",
                    Last_name = "Тест",
                    PasswordConfirm = "Qwerty123!",
                    PhoneNumber = "89772341212"
                };
                await service.regUser(model);
                Assert.Fail("Должна была выпасть ошибка о том, что данные заняты");
            }
            catch
            {
                
            }
        }

        [Fact]
        [TestPriority(6)]
        public async Task DeleteUser_DeleteRegUser_Passed()
        {
            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UsersGetDtoModel model = await serviceForDelete.GetUsers(query);
                User record = model.Collection!.Where(x => x.Account!.Login == "Test").First();
                await serviceForDelete.DeleteUser(record.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
