using API.Data;
using API.Security;
using API.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto.GetModels;
using Models.Dto.GetModels.ProfileModels;
using Models.Dto.PostPutModels;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using Models.Mapper;
using Models.QuerySupporter;
using System.Linq;
using System.Security.Claims;

namespace API.Controllers.AccountsController
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProfileController : ControllerBase
    {

        private readonly DataContext _context;

        public ProfileController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<User>> getProfile(CancellationToken ct)
        {
            Guid id = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            User? user = await _context.Users.Where(x => x.Id == id)
                .Include(x => x.Account).ThenInclude(x => x!.Roles)
                .AsNoTracking()
                .FirstOrDefaultAsync(ct);
            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(user);
        }

        [HttpGet("Tokens")]
        public async Task<ActionResult<TokensGetDtoModel>> getCurrentTokens([FromQuery] QuerySupporter query,
            CancellationToken ct)
        {
            Guid id = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            var items = _context.Tokens.AsNoTracking().Where(x => x.Account!.UserId == id);
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            items =  QueryParamHelper.SetParams(items, query);
            TokensGetDtoModel tokensGetDtoModel = new TokensGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            tokensGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            tokensGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            tokensGetDtoModel.CurrentPageIndex = tokensGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            tokensGetDtoModel.Collection = await items.ToListAsync(ct);
            return Ok(tokensGetDtoModel);
        }

        [HttpDelete("Tokens")]
        public async Task<ActionResult> deleteToken([FromQuery] string token)
        {
            Guid id = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            Token? delete = await _context.Tokens.Where(x => x.TokenStr == token).Include(x => x.Account).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Токена не существует!");
            }
            if (delete.Account!.UserId != id)
            {
                return Unauthorized();
            }
            _context.Tokens.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }

        [HttpPut]
        public async Task<ActionResult<ProfileGetModel>> updateProfile(UpdateModel model)
        {
            Guid id = Guid.Parse(User.Claims.Where(x => x.Type == ClaimTypes.NameIdentifier).First().Value);
            User? user = await _context.Users.Where(x => x.Id == id)
                .Include(x => x.Account).ThenInclude(x => x!.Roles).FirstOrDefaultAsync();
            if (user == null)
            {
                return Unauthorized();
            }
            if (_context.Accounts.Where(x => x.Email == model.Email).Any() && user.Account!.Email != model.Email)
            {
                return BadRequest("Данная электронная почта занята другим аккаунтом");
            }
            if (_context.Accounts.Where(x => x.Login == model.Login).Any() && user.Account!.Login != model.Login)
            {
                return BadRequest("Данный логин занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PhoneNumber == model.PhoneNumber).Any() && user.PhoneNumber != model.PhoneNumber)
            {
                return BadRequest("Данный телефонный номер занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PassportSeries == model.PassportSeries && x.PassportNumber == model.PassportNumber).Any()
                && user.PassportNumber != model.PassportNumber && user.PassportSeries != model.PassportSeries)
            {
                return BadRequest("Данные серия и номер паспорта заняты другим аккаунтом");
            }
            ProfileGetModel responce = new ProfileGetModel();
            if (model.Login != user.Account!.Login || model.Email != user.Account!.Email || !string.IsNullOrEmpty(model.Password))
            {
                if (string.IsNullOrEmpty(model.OldPassword))
                {
                    return BadRequest("При изменении логина, электронной почты или пароля, ввод старого пароля необходим!");
                }

                if (!HashProvider.CheckHash(model.OldPassword, user.Account!.Password))
                {
                    return BadRequest("Старый пароль аккаунта - неправильный!");
                }

                _context.Tokens.RemoveRange(_context.Tokens.Where(x => x.Account!.UserId == id));
                string refresttoken = TokenHandler.GenerateRefreshToken();
                Token token = new Token();
                token.Account = user.Account;
                token.TokenStr = refresttoken;
                responce.RefreshToken = refresttoken;
                await _context.Tokens.AddAsync(token);
                await _context.SaveChangesAsync();
            }
            SafeMapper.MapUserFromUserDto(model, user);
            if (model.Password != string.Empty && model.Password != null)
            {
                user.Account!.Password = HashProvider.MakeHash(model.Password);
            }

            
            await _context.SaveChangesAsync();
            responce.User = user;
            return Ok(responce);
        }

    }
}
