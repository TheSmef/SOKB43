using Models.Dto.PostPutModels.AccountModels;

namespace WEB.Data.Services.Base
{
    public interface IAuthService
    {
        public Task authUser(AuthModel model);
        public Task regUser(RegModel model);
        public Task<string> getToken();
    }
}
