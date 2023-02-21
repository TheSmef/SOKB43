using Models.Dto.GetModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IPostService
    {
        public Task<PostsGetDtoModel> GetPosts(QuerySupporter query);
        public Task<Post> GetPostById(Guid id);
        public Task<Post> UpdatePost(Post model);
        public Task<Post> AddPost(Post model);
        public Task DeletePost(Guid id);
    }
}
