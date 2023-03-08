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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class OrderServiceTests
    {
        private readonly IContractorService contractorService;
        private readonly IOrderService service;
        private readonly HttpClient client;
        public OrderServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new OrderService(client);
            contractorService = new ContractorService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddOrder_Failed()
        {
            try
            {
                await service.AddOrder(new OrderDto
                {
                    ContractorId = Guid.Empty,
                    Date= DateTime.Today.AddDays(-2),
                    Sum = 100
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddOrder_Passed()
        {
            try
            {
                Contractor data = await contractorService.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "exampleorder@test.test",
                    Name = "ТестДляЗаказа",
                    PhoneNumber = "87774563245",
                });
                await service.AddOrder(new OrderDto
                {
                    ContractorId = data.Id,
                    Date = DateTime.Today.AddDays(-10),
                    Sum = 100
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetOrders_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetOrders(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetOrders_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetOrders(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetOrderById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                OrderGetDtoModel model = await service.GetOrders(query);
                await service.GetOrderById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetOrderById_Failed()
        {
            try
            {
                await service.GetOrderById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateOrder_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                OrderGetDtoModel model = await service.GetOrders(query);
                Order data = model.Collection!.Where(x => x.Date == DateTime.Today.AddDays(-10)).First();
                OrderDto dto = new OrderDto
                {
                    ContractorId = data.Contractor!.Id,
                    Date = DateTime.Today.AddDays(-10),
                    Sum = 100
                };
                await service.UpdateOrder(dto, data.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateOrder_Failed()
        {
            try
            {
                OrderDto dto = new OrderDto
                {
                    ContractorId = Guid.Empty,
                    Date = DateTime.Today.AddDays(-10),
                    Sum = 100
                };
                await service.UpdateOrder(dto, Guid.Empty);
                Assert.Fail("Должна быть ошибка отсутствия id");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteContractor_Failed()
        {
            try
            {
                await service.DeleteOrder(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeleteContractor_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                OrderGetDtoModel model = await service.GetOrders(query);
                Order data = model.Collection!.Where(x => x.Date == DateTime.Today.AddDays(-10)).First();
                await service.DeleteOrder(data.Id);
                ContractorsGetDtoModel modelContractors = await contractorService.GetContractors(query);
                await contractorService.DeleteContractor(modelContractors.Collection!.Where(x => x.Name == "ТестДляЗаказа").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
