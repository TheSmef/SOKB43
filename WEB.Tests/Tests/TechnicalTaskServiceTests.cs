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
    public class TechnicalTaskServiceTests
    {
        private readonly IEquipmentTypeService typeService;
        private readonly ITechnicalTaskService service;
        private readonly HttpClient client;
        public TechnicalTaskServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new TechnicalTaskService(client);
            typeService = new EquipmentTypeService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddTechnicalTaskt_Failed()
        {
            try
            {
                await service.AddTechnicalTask(new TechnicalTaskDto());
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddTechnicalTask_Passed()
        {
            try
            {
                TypeEquipment data = await typeService.AddType(new TypeEquipment
                {
                    Name = "ТестДляТЗ",
                });
                await service.AddTechnicalTask(new TechnicalTaskDto
                {
                    Content = "Контент",
                    Date= DateTime.Today.AddDays(-10),
                    NameEquipment = "Имя",
                    TypeEquipmentId = data.Id
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetTechnicalTasks_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetTechnicalTasks(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
        

        [Fact]
        [TestPriority(5)]
        public async Task GetTechnicalTasks_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetTechnicalTasks(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetTechnicalTaskById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                TechnicalTasksGetDtoModel model = await service.GetTechnicalTasks(query);
                await service.GetTechnicalTaskById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetTechnicalTaskById_Failed()
        {
            try
            {
                await service.GetTechnicalTaskById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateTechnicalTask_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                TechnicalTasksGetDtoModel model = await service.GetTechnicalTasks(query);
                TechnicalTask data = model.Collection!.Where(x => x.NameEquipment == "Имя").First();
                Guid id = data.Id;
                TechnicalTaskDto dto = new TechnicalTaskDto
                {
                    Content = "Контент ТЗ",
                    Date = DateTime.Today.AddDays(-1),
                    NameEquipment = "Оборудование",
                    TypeEquipmentId = data.TypeEquipment!.Id
                };
                await service.UpdateTechnicalTask(dto, data.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateTechnicalTask_Failed()
        {
            try
            {
                TechnicalTaskDto data = new TechnicalTaskDto();
                await service.UpdateTechnicalTask(data, Guid.Empty);
                Assert.Fail("Должна быть ошибка отсутствия id или пустого объекта");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteContractor_Failed()
        {
            try
            {
                await service.DeleteTechnicalTask(Guid.Empty);
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
                TechnicalTasksGetDtoModel model = await service.GetTechnicalTasks(query);
                TechnicalTask data = model.Collection!.Where(x => x.NameEquipment == "Оборудование").First();
                await service.DeleteTechnicalTask(data.Id);
                await typeService.DeleteType(data.TypeEquipment!.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
