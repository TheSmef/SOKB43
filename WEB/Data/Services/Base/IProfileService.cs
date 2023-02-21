using Models.Dto.GetModels;
using Models.Dto.GetModels.ProfileModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.QuerySupporter;

namespace WEB.Data.Services.Base
{
    public interface IProfileService
    {
        public Task LogOut();
        public Task<User> GetProfile();
        public Task<TokensGetDtoModel> GetTokens(QuerySupporter query);
        public Task<User> UpdateProfile(UpdateModel model);
        public Task deleteToken(string token);
    }
}
