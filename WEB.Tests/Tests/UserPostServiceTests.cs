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
    public class UserPostServiceTests
    {
        private readonly IUserPostService service;
        private readonly IPostService postService;
        private readonly IUserService userService;
        private readonly HttpClient client;
        public UserPostServiceTests()
        {
            client = new HttpClient { BaseAddress = new Uri(TestData.URI) };
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestData.JWTTOKEN);
            service = new UserPostService(client);
            postService= new PostService(client);
            userService= new UserService(client);
        }

        [Fact]
        [TestPriority(1)]
        public async Task AddUserPost_Failed()
        {
            try
            {
                await service.AddUserPost(new UserPostDto
                {
                    Deleted= false,
                    PostId= Guid.NewGuid(),
                    Share = 1,
                    UserId = Guid.NewGuid(),
                });
                Assert.Fail("Введены некоректные данные");
            }
            catch { }
        }

        [Fact]
        [TestPriority(2)]
        public async Task AddUserPost_Passed()
        {
            try
            {
                Guid postId = (await postService.AddPost(new Post
                {
                    Name = "ДолжностьСотрудника",
                    Salary = 1,
                })).Id;
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                await service.AddUserPost(new UserPostDto
                {
                    Deleted = false,
                    PostId = postId,
                    Share = 1,
                    UserId = (await userService.GetUsers(query)).Collection!.Where(x => x.Account!.Login == "admin").First().Id,
                });

            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [Fact]
        [TestPriority(4)]
        public async Task GetUserPosts_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 5 };
                await service.GetUserPosts(query);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }


        [Fact]
        [TestPriority(5)]
        public async Task GetUserPosts_Failed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = -1, Top = 5 };
                await service.GetUserPosts(query);
                Assert.Fail("Должен быть неправильный формат строки");
            }
            catch { }
        }


        [Fact]
        [TestPriority(6)]
        public async Task GetUserPostById_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UserPostsGetDtoModel model = await service.GetUserPosts(query);
                await service.GetUserPostById(model.Collection!.First().Id);
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
                await service.GetUserPostById(Guid.Empty);
                Assert.Fail("Должен быть пустой Id");
            }
            catch { }
        }

        [Fact]
        [TestPriority(8)]
        public async Task UpdateUserPost_Passed()
        {
            try
            {
                QuerySupporter query = new QuerySupporter { Skip = 0, Top = 10 };
                UserPostsGetDtoModel model = await service.GetUserPosts(query);
                UserPost record = model.Collection!.First();
                UserPostDto dto = new UserPostDto
                {
                    Deleted = true,
                    PostId = record.Post!.Id,
                    Share = 1,
                    UserId = record.User!.Id,
                };
                
                await service.UpdateUserPost(dto, record.Id);
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
                UserPostsGetDtoModel model = await service.GetUserPosts(query);
                UserPost record = model.Collection!.First();
                UserPostDto dto = new UserPostDto
                {
                    Deleted = true,
                    PostId = Guid.Empty,
                    Share = 10,
                    UserId = record.Id,
                };

                await service.UpdateUserPost(dto, record.Id);
                Assert.Fail("Должна быть ошибка отсутствия валидации");
            }
            catch { }
        }


        [Fact]
        [TestPriority(10)]
        public async Task DeletePost_Failed()
        {
            try
            {
                await service.DeleteUserPost(Guid.Empty);
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
                UserPostsGetDtoModel model = await service.GetUserPosts(query);
                UserPost record = model.Collection!.First();
                await service.DeleteUserPost(record.Id);
                await postService.DeletePost(record.Post!.Id);
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }
    }
}

