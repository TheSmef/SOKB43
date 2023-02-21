using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IUserPostService
    {
        public Task<UserPostsGetDtoModel> GetUserPosts(QuerySupporter query);
        public Task<UserPost> GetUserPostById(Guid id);
        public Task<UserPost> UpdateUserPost(UserPostDto model, Guid id);
        public Task<UserPost> AddUserPost(UserPostDto model);
        public Task DeleteUserPost(Guid id);
    }
}
