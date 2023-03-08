using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
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

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class UsersServiceTests
    {
        private readonly IUserService service;
        private readonly HttpClient client;
        public UsersServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new UserService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddUser_Failed()
        {
            try
            {
                await service.AddUser(new UserDto
                {
                   BirthDate= DateTime.Today.AddYears(-49),
                   Email = "example@test.com",
                   Login = "TestLogin",
                   PassportNumber = "123456",
                   Password = "Qwerty123!",
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddUser_Passed()
        {
            try
            {
                await service.AddUser(new UserDto
                {
                    BirthDate = DateTime.Today.AddYears(-49),
                    First_name = "Тест",
                    Last_name = "Тест",
                    Email = "example@test.com",
                    Login = "TestLogin",
                    PassportNumber = "123456",
                    Password = "Qwerty123!",
                    PassportSeries = "3333",
                    PasswordConfirm = "Qwerty123!",
                    PhoneNumber = "89754432123",
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetUsers_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetUsers(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetUsers_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetUsers(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetUserById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UsersGetDtoModel model = await service.GetUsers(query);
                await service.GetUserById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetUserId_Failed()
        {
            try
            {
                await service.GetUserById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateUser_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UsersGetDtoModel model = await service.GetUsers(query);
                User record = model.Collection!.Where(x => x.Account!.Login == "TestLogin").First();
                UserUpdateDto dto = new UserUpdateDto
                {
                    BirthDate = DateTime.Today.AddYears(-49),
                    First_name = "ТестОбновлённый",
                    Last_name = "ТестОбновлённый",
                    Email = "example@test.com",
                    Login = "TestLogin",
                    PassportNumber = "123456",
                    Password = "Qwerty123!",
                    PassportSeries = "3333",
                    PasswordConfirm = "Qwerty123!",
                    PhoneNumber = "89754432123",
                };
                await service.UpdateUser(dto, record.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateUser_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UsersGetDtoModel model = await service.GetUsers(query);
                User record = model.Collection!.Where(x => x.Account!.Login == "TestLogin").First();
                UserUpdateDto dto = new UserUpdateDto
                {
                    First_name = "ТестОбновлённый",
                    Last_name = "ТестОбновлённый",
                    Email = "example@test.com",
                    Login = "TestLogin",
                };
                await service.UpdateUser(dto, record.Id);
                Assert.Fail("Должна быть ошибка отсутствия валидации");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteUser_Failed()
        {
            try
            {
                await service.DeleteUser(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeleteUser_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UsersGetDtoModel model = await service.GetUsers(query);
                User record = model.Collection!.Where(x => x.Account!.Login == "TestLogin").First();
                await service.DeleteUser(record.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
