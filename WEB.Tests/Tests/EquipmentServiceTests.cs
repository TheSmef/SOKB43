using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using WEB.Data.Services.Base;
using WEB.Data.Services;
using WEB.Tests.Constants;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;
using WEB.Tests.Orderer;
using Models.Utility;
using WEB.Tests.Tests.Stubs;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class EquipmentServiceTests
    {
        private readonly IContractorService contractorService;
        private readonly IOrderService orderService;
        private readonly IEquipmentService service;
        private readonly IEquipmentTypeService equipmentTypeService;
        private readonly ITechnicalTaskService technicalTaskService;
        private readonly HttpClient client;
        public EquipmentServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            orderService = new OrderService(client, new DownloadServiceStub());
            contractorService = new ContractorService(client, new DownloadServiceStub());
            service = new EquipmentService(client, new DownloadServiceStub());
            technicalTaskService = new TechnicalTaskService(client);
            equipmentTypeService = new EquipmentTypeService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddEquipment_Failed()
        {
            try
            {
                await service.AddEquipment(new EquipmentDto());
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddEquipment_Passed()
        {
            try
            {
                Contractor dataContactor = await contractorService.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "exampleeq@test.test",
                    Name = "ТестДляОборудования",
                    PhoneNumber = "86774563245",
                });
                Order dataOrder = await orderService.AddOrder(new OrderDto
                {
                    ContractorId = dataContactor.Id,
                    Date = DateTime.Today.AddDays(-5),
                    Sum = 1000
                });
                TypeEquipment dataType = await equipmentTypeService.AddType(new TypeEquipment
                {
                    Name = "ТестДляОборудования",
                });
                TechnicalTask dataTask = await technicalTaskService.AddTechnicalTask(new TechnicalTaskDto
                {
                    Content = "Контент",
                    Date = DateTime.Today.AddDays(-10),
                    NameEquipment = "ОборудованиеТест",
                    TypeEquipmentId = dataType.Id
                });
                await service.AddEquipment(new EquipmentDto
                {
                    Date = DateTime.Today,
                    Deleted = false,
                    EquipmentCode = "1111111111",
                    OrderId = dataOrder.Id,
                    Status = EnumUtility.GetStringsValues(typeof(Equipment.EquipmentStatusEnum)).ElementAt(0),
                    TechnicalTaskId = dataTask.Id
                    
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetEquipment_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetEquipment(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetEquipment_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetEquipment(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetEquipmentById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentDtoGetModel model = await service.GetEquipment(query);
                await service.GetEquipmentById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetEquipmentById_Failed()
        {
            try
            {
                await service.GetEquipmentById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateEquipment_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentDtoGetModel model = await service.GetEquipment(query);
                Equipment data = model.Collection!.Where(x => x.EquipmentCode == "1111111111").First();
                EquipmentDto dto = new EquipmentDto
                {
                    Date = DateTime.Today,
                    Deleted = false,
                    EquipmentCode = "2222222222",
                    OrderId = data.Order!.Id,
                    Status = EnumUtility.GetStringsValues(typeof(Equipment.EquipmentStatusEnum)).ElementAt(1),
                    TechnicalTaskId = data.TechnicalTask!.Id,
                };
                await service.UpdateEquipment(dto, data.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateEquipment_Failed()
        {
            try
            {
                await service.UpdateEquipment(new EquipmentDto(), Guid.Empty);
                Assert.Fail("Должна быть ошибка отсутствия id");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteEquipment_Failed()
        {
            try
            {
                await service.DeleteEquipment(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeleteEquipment_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentDtoGetModel model = await service.GetEquipment(query);
                Equipment data = model.Collection!.Where(x => x.EquipmentCode == "2222222222").First();
                await service.DeleteEquipment(data.Id);
                OrderGetDtoModel modelOrder = await orderService.GetOrders(query);
                await orderService.DeleteOrder(modelOrder.Collection!.Where(x => x.Date == DateTime.Today.AddDays(-5)).First().Id);
                ContractorsGetDtoModel modelContractor = await contractorService.GetContractors(query);
                await contractorService.DeleteContractor(modelContractor.Collection!.Where(x => x.Name == "ТестДляОборудования").First().Id);
                TechnicalTasksGetDtoModel modelTask = await technicalTaskService.GetTechnicalTasks(query);
                await technicalTaskService.DeleteTechnicalTask(modelTask.Collection!.Where(x => x.NameEquipment == "ОборудованиеТест").First().Id);
                EquipmentTypesGetDtoModel modelType = await equipmentTypeService.GetTypes(query);
                await equipmentTypeService.DeleteType(modelType.Collection!.Where(x => x.Name == "ТестДляОборудования").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
