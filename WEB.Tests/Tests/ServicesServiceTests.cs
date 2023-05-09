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
using Models.QuerySupporter;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.Utility;
using Models.Dto.GetModels;
using WEB.Tests.Tests.Stubs;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class ServicesServiceTests
    {
        private readonly IContractorService contractorService;
        private readonly IOrderService orderService;
        private readonly IEquipmentService equipmentService;
        private readonly IEquipmentTypeService equipmentTypeService;
        private readonly ITechnicalTaskService technicalTaskService;
        private readonly IServicesService service;
        private readonly HttpClient client;
        public ServicesServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            orderService = new OrderService(client, new DownloadServiceStub());
            contractorService = new ContractorService(client, new DownloadServiceStub());
            equipmentService = new EquipmentService(client, new DownloadServiceStub());
            technicalTaskService = new TechnicalTaskService(client);
            equipmentTypeService = new EquipmentTypeService(client);
            service = new ServicesService(client, new DownloadServiceStub());
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddService_Failed()
        {
            try
            {
                await service.AddService(new ServiceDto());
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddService_Passed()
        {
            try
            {
                Contractor dataContactor = await contractorService.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "exampleservice@test.test",
                    Name = "ТестДляОбслуживания",
                    PhoneNumber = "85774563245",
                });
                Order dataOrder = await orderService.AddOrder(new OrderDto
                {
                    ContractorId = dataContactor.Id,
                    Date = DateTime.Today,
                    Sum = 100000
                });
                TypeEquipment dataType = await equipmentTypeService.AddType(new TypeEquipment
                {
                    Name = "ТестДляОбслуживания",
                });
                TechnicalTask dataTask = await technicalTaskService.AddTechnicalTask(new TechnicalTaskDto
                {
                    Content = "Контент",
                    Date = DateTime.Today.AddDays(-10),
                    NameEquipment = "ТестДляОбслуживания",
                    TypeEquipmentId = dataType.Id
                });
                Equipment dataEquipment = await equipmentService.AddEquipment(new EquipmentDto
                {
                    Date = DateTime.Today,
                    Deleted = false,
                    EquipmentCode = "9999999999",
                    OrderId = dataOrder.Id,
                    Status = EnumUtility.GetStringsValues(typeof(Equipment.EquipmentStatusEnum)).ElementAt(0),
                    TechnicalTaskId = dataTask.Id
                });
                await service.AddService(new ServiceDto
                {
                    Date= DateTime.Today,
                    Deleted= false,
                    EquipmentId= dataEquipment.Id,
                    ServiceType = EnumUtility.GetStringsValues(typeof(Service.ServiceTypeEnum)).ElementAt(0),
                    Status = EnumUtility.GetStringsValues(typeof(Service.ServiceStatusEnum)).ElementAt(0),
                    Sum = 10,
                    WorkContent = "Работы"
                });
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetServices_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetServices(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetServices_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetServices(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetServiceById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                ServiceGetDtoModel model = await service.GetServices(query);
                await service.GetServiceById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetServiceById_Failed()
        {
            try
            {
                await service.GetServiceById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateService_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                ServiceGetDtoModel model = await service.GetServices(query);
                Service data = model.Collection!.Where(x => x.Date == DateTime.Today).First();
                ServiceDto dto = new ServiceDto
                {
                    Date = DateTime.Today,
                    Deleted = false,
                    EquipmentId = data.Equipment!.Id,
                    ServiceType = EnumUtility.GetStringsValues(typeof(Service.ServiceTypeEnum)).ElementAt(2),
                    Status = EnumUtility.GetStringsValues(typeof(Service.ServiceStatusEnum)).ElementAt(2),
                    Sum = 1000,
                    WorkContent = "Работы"
                };

                await service.UpdateService(dto, data.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateService_Failed()
        {
            try
            {
                await service.UpdateService(new ServiceDto(), Guid.Empty);
                Assert.Fail("Должна быть ошибка отсутствия id");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteService_Failed()
        {
            try
            {
                await service.DeleteService(Guid.Empty);
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
                ServiceGetDtoModel model = await service.GetServices(query);
                Service data = model.Collection!.Where(x => x.Date == DateTime.Today).First();
                await service.DeleteService(data.Id);
                EquipmentDtoGetModel modelEquipment = await equipmentService.GetEquipment(query);
                await equipmentService.DeleteEquipment(modelEquipment.Collection!.Where(x => x.EquipmentCode == "9999999999").First().Id);
                OrderGetDtoModel modelOrder = await orderService.GetOrders(query);
                await orderService.DeleteOrder(modelOrder.Collection!.Where(x => x.Date == DateTime.Today).First().Id);
                ContractorsGetDtoModel modelContractor = await contractorService.GetContractors(query);
                await contractorService.DeleteContractor(modelContractor.Collection!.Where(x => x.Name == "ТестДляОбслуживания").First().Id);
                TechnicalTasksGetDtoModel modelTask = await technicalTaskService.GetTechnicalTasks(query);
                await technicalTaskService.DeleteTechnicalTask(modelTask.Collection!.Where(x => x.NameEquipment == "ТестДляОбслуживания").First().Id);
                EquipmentTypesGetDtoModel modelType = await equipmentTypeService.GetTypes(query);
                await equipmentTypeService.DeleteType(modelType.Collection!.Where(x => x.Name == "ТестДляОбслуживания").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
