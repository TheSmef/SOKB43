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
using WEB.Tests.Tests.Stubs;

namespace WEB.Tests.Tests
{
    [TestCaseOrderer("WEB.Tests.Orderer.PriorityOrderer", "WEB.Tests")]
    public class ContractorServiceTests
    {
        private readonly IContractorService service;
        private readonly HttpClient client;
        public ContractorServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new ContractorService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddContactor_Failed()
        {
            try
            {
                await service.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "example",
                    Name= "Test",
                    PhoneNumber = "89774563245",
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddContactor_Passed()
        {
            try
            {
                await service.AddContractor(new Contractor
                {
                    Description = "Описание",
                    Email = "example@test.test",
                    Name = "Тест",
                    PhoneNumber = "89774563245",
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetContractors_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetContractors(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetContractors_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetContractors(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetContractorById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                ContractorsGetDtoModel model = await service.GetContractors(query);
                await service.GetContractorById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetContractorById_Failed()
        {
            try
            {
                await service.GetContractorById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateContractor_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                ContractorsGetDtoModel model = await service.GetContractors(query);
                Contractor contractor = model.Collection!.Where(x => x.Name == "Тест").First();
                Guid id = contractor.Id;
                contractor = new Contractor
                {
                    Id = id,
                    Description = "Описание контрагента",
                    Email = "example123@test.test",
                    Name = "Тест3",
                    PhoneNumber = "89778563255",
                };
                await service.UpdateContractor(contractor);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdateContractor_Failed()
        {
            try
            {
                Contractor contractor = new Contractor
                {
                    Description = "Описание контрагента",
                    Email = "example123@test.test",
                    Name = "Тест",
                    PhoneNumber = "8977456325",
                };
                await service.UpdateContractor(contractor);
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
                await service.DeleteContractor(Guid.Empty);
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
                ContractorsGetDtoModel model = await service.GetContractors(query);
                await service.DeleteContractor(model.Collection!.Where(x => x.Name == "Тест3").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
