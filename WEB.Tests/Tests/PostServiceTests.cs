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
    public class PostServiceTests
    {
        private readonly IPostService service;
        private readonly HttpClient client;
        public PostServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new PostService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddPost_Failed()
        {
            try
            {
                await service.AddPost(new Post
                {
                    Name = "test",
                    Salary = 1,
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddPost_Passed()
        {
            try
            {
                await service.AddPost(new Post
                {
                    Name = "Тест",
                    Salary = 1,
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetPosts_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetPosts(query);
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
                await service.GetPosts(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetPostById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                PostsGetDtoModel model = await service.GetPosts(query);
                await service.GetPostById(model.Collection!.First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(7)]
        public async Task GetPostId_Failed()
        {
            try
            {
                await service.GetPostById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdatePost_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                PostsGetDtoModel model = await service.GetPosts(query);
                Post record = model.Collection!.Where(x => x.Name == "Тест").First();
                Guid id = record.Id;
                record = new Post
                {
                    Id = id,
                    Name = "Менеджер",
                    Salary = 10,
                };
                await service.UpdatePost(record);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(9)]
        public async Task UpdatePost_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                PostsGetDtoModel model = await service.GetPosts(query);
                Post record = model.Collection!.First();
                Guid id = record.Id;
                record = new Post
                {
                    Name = "Qwerty",
                    Salary = -10,
                };
                await service.UpdatePost(record);
                Assert.Fail("Должна быть ошибка отсутствия id или валидации");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeletePost_Failed()
        {
            try
            {
                await service.DeletePost(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(11)]
        public async Task DeletePost_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                PostsGetDtoModel model = await service.GetPosts(query);
                await service.DeletePost(model.Collection!.Where(x => x.Name == "Менеджер").First().Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}
