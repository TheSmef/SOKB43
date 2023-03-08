using Models.Dto.GetModels;
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
    public class EquipmentTypeServiceTests
    {
        private readonly IEquipmentTypeService service;
        private readonly HttpClient client;
        public EquipmentTypeServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new EquipmentTypeService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddType_Failed()
        {
            try
            {
                await service.AddType(new TypeEquipment
                {
                    Name = "test",
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddType_Passed()
        {
            try
            {
                await service.AddType(new TypeEquipment
                {
                    Name = "Тест",
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetPostss_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetTypes(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetPosts_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetTypes(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetTypeById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentTypesGetDtoModel model = await service.GetTypes(query);
                await service.GetTypeById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetTypeId_Failed()
        {
            try
            {
                await service.GetTypeById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateType_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentTypesGetDtoModel model = await service.GetTypes(query);
                TypeEquipment record = model.Collection!.Where(x => x.Name == "Тест").First();
                Guid id = record.Id;
                record = new TypeEquipment
                {
                    Id = id,
                    Name = "Тест",
                };
                await service.UpdateType(record);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateType_Failed()
        {
            try
            {
                TypeEquipment record = new TypeEquipment();
                await service.UpdateType(record);
                Assert.Fail("Должна быть ошибка отсутствия id или валидации");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeleteType_Failed()
        {
            try
            {
                await service.DeleteType(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeleteType_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                EquipmentTypesGetDtoModel model = await service.GetTypes(query);
                TypeEquipment record = model.Collection!.Where(x => x.Name == "Тест").First();
                await service.DeleteType(record.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
