using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IUserService
    {
        public Task<UsersGetDtoModel> GetUsers(QuerySupporter query);
        public Task<User> GetUserById(Guid id);
        public Task<User> UpdateUser(UserUpdateDto model, Guid id);
        public Task<User> AddUser(UserDto model);
        public Task DeleteUser(Guid id);
    }
}
