using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WEB.Data.Services.Base;
using WEB.Data.Services;
using WEB.Tests.Constants;
using WEB.Tests.Orderer;
using WEB.Tests.Tests.Stubs;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class TechnicalTestsServiceTests
    {
        private readonly IContractorService contractorService;
        private readonly IOrderService orderService;
        private readonly IEquipmentService equipmentService;
        private readonly IEquipmentTypeService equipmentTypeService;
        private readonly ITechnicalTaskService technicalTaskService;
        private readonly ITechnicalTestsService service;
        private readonly HttpClient client;
        public TechnicalTestsServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            orderService = new OrderService(client, new DownloadServiceStub());
            contractorService = new ContractorService(client, new DownloadServiceStub());
            equipmentService = new EquipmentService(client, new DownloadServiceStub());
            technicalTaskService = new TechnicalTaskService(client);
            equipmentTypeService = new EquipmentTypeService(client);
            service = new TechnicalTestsService(client, new DownloadServiceStub());
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddTest_Failed()
        {
            try
            {
                await service.AddTest(new TechnicalTestDto());
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddTest_Passed()
        {
            try
            {
                Contractor dataContactor = await contractorService.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "exampletest@test.test",
                    Name = "ТестДляТестирования",
                    PhoneNumber = "84774563245",
                });
                Order dataOrder = await orderService.AddOrder(new OrderDto
                {
                    ContractorId = dataContactor.Id,
                    Date = DateTime.Today.AddDays(-2),
                    Sum = 10000
                });
                TypeEquipment dataType = await equipmentTypeService.AddType(new TypeEquipment
                {
                    Name = "ТестДляТестирования",
                });
                TechnicalTask dataTask = await technicalTaskService.AddTechnicalTask(new TechnicalTaskDto
                {
                    Content = "Контент",
                    Date = DateTime.Today.AddDays(-10),
                    NameEquipment = "ТестДляТестирования",
                    TypeEquipmentId = dataType.Id
                });
                Equipment dataEquipment =  await equipmentService.AddEquipment(new EquipmentDto
                {
                    Date = DateTime.Today,
                    Deleted = false,
                    EquipmentCode = "0000000000",
                    OrderId = dataOrder.Id,
                    Status = EnumUtility.GetStringsValues(typeof(Equipment.EquipmentStatusEnum)).ElementAt(0),
                    TechnicalTaskId = dataTask.Id
                });
                await service.AddTest(new TechnicalTestDto
                {
                    Comment = "Тест",
                    Date= DateTime.Today,
                    Deleted= false,
                    Description = "Описание",
                    EquipmentId = dataEquipment.Id,
                    ExpectedConclusion = "Ожидаемый результат",
                    FactConclusion = "Фактический результат",
                    Passed= true,
                    TestData = "Тестовые данные",
                    TestPriority = EnumUtility.GetStringsValues(typeof(TechnicalTest.TestPriorityEnum)).ElementAt(1),
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetTests_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetTests(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetTests_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetTests(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetTestById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                TechnicalTestsGetDtoModel model = await service.GetTests(query);
                await service.GetTestById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetTestById_Failed()
        {
            try
            {
                await service.GetTestById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateTest_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                TechnicalTestsGetDtoModel model = await service.GetTests(query);
                TechnicalTest data = model.Collection!.Where(x => x.Date == DateTime.Today).First();
                TechnicalTestDto dto = new TechnicalTestDto
                {
                    Comment = "Тест",
                    Date = DateTime.Today,
                    Deleted = false,
                    Description = "Описание",
                    EquipmentId = data.Equipment!.Id,
                    ExpectedConclusion = "Ожидаемый результат",
                    FactConclusion = "Фактический результат",
                    Passed = true,
                    TestData = "Тестовые данные",
                    TestPriority = EnumUtility.GetStringsValues(typeof(TechnicalTest.TestPriorityEnum)).ElementAt(1),
                };

                await service.UpdateTest(dto, data.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateTest_Failed()
        {
            try
            {
                await service.UpdateTest(new TechnicalTestDto(), Guid.Empty);
                Assert.Fail("Должна быть ошибка отсутствия id");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteTest_Failed()
        {
            try
            {
                await service.DeleteTest(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeleteTest_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                TechnicalTestsGetDtoModel model = await service.GetTests(query);
                TechnicalTest data = model.Collection!.Where(x => x.Date == DateTime.Today).First();
                await service.DeleteTest(data.Id);
                EquipmentDtoGetModel modelEquipment = await equipmentService.GetEquipment(query);
                await equipmentService.DeleteEquipment(modelEquipment.Collection!.Where(x => x.EquipmentCode == "0000000000").First().Id);
                OrderGetDtoModel modelOrder = await orderService.GetOrders(query);
                await orderService.DeleteOrder(modelOrder.Collection!.Where(x => x.Date == DateTime.Today.AddDays(-2)).First().Id);
                ContractorsGetDtoModel modelContractor = await contractorService.GetContractors(query);
                await contractorService.DeleteContractor(modelContractor.Collection!.Where(x => x.Name == "ТестДляТестирования").First().Id);
                TechnicalTasksGetDtoModel modelTask = await technicalTaskService.GetTechnicalTasks(query);
                await technicalTaskService.DeleteTechnicalTask(modelTask.Collection!.Where(x => x.NameEquipment == "ТестДляТестирования").First().Id);
                EquipmentTypesGetDtoModel modelType = await equipmentTypeService.GetTypes(query);
                await equipmentTypeService.DeleteType(modelType.Collection!.Where(x => x.Name == "ТестДляТестирования").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
