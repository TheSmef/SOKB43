using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using API.Data;
using Models.Entity;
using AutoMapper;
using Models.Mapper;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Models.QuerySupporter;
using API.Utility;
using Models.Dto.PostPutModels;
using Models.Dto.GetModels;
using API.Security;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Azure;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Отдел кадров")]
    public class UsersController : Controller
    {
        private readonly IMapper _mapper;

        private readonly DataContext _context;

        public UsersController(DataContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<UsersGetDtoModel>> getUsers(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Users.Include(x => x.Account).ThenInclude(x => x!.Roles).AsQueryable();
            if (query == null)
            {
                return BadRequest("Нет параметров для данных!");
            }
            if (!string.IsNullOrEmpty(query.Filter))
            {
                if (query.FilterParams != null)
                {
                    items = items.Where(query.Filter, query.FilterParams);
                }
                else
                {
                    items = items.Where(query.Filter);
                }
            }

            if (!string.IsNullOrEmpty(query.OrderBy))
            {
                items = items.OrderBy(query.OrderBy);
            }
            UsersGetDtoModel usersGetDtoModel = new UsersGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            usersGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            usersGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            usersGetDtoModel.CurrentPageIndex = usersGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            usersGetDtoModel.Collection = await items.ToListAsync();
            return Ok(usersGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<User>> getUserById(Guid id)
        {
            if (_context.Users.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Users.Where(x => x.Id == id)
                    .Include(x => x.Account).ThenInclude(x => x!.Roles).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<User>> postUser(UserDto userDto)
        {
            User user = _mapper.Map<User>(userDto);
            user.Account = _mapper.Map<Account>(userDto);
            if (_context.Accounts.Where(x => x.Email == user.Account.Email).Any())
            {
                return BadRequest("Данная электронная почта занята другим аккаунтом");
            }
            if (_context.Accounts.Where(x => x.Login == user.Account.Login).Any())
            {
                return BadRequest("Данный логин занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PhoneNumber == user.PhoneNumber).Any())
            {
                return BadRequest("Данный телефонный номер занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PassportSeries == user.PassportSeries && x.PassportNumber == user.PassportNumber).Any())
            {
                return BadRequest("Данные серия и номер паспорта заняты другим аккаунтом");
            }
            user.Account!.Password = HashProvider.MakeHash(userDto.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return Ok(user);
        }

        [HttpPut]
        public async Task<ActionResult<User>> putUser([FromQuery] Guid id, UserUpdateDto userDto)
        {
            User? user = await _context.Users.Where(x => x.Id == id).Include(x => x.Account)
                .ThenInclude(x => x!.Roles).FirstOrDefaultAsync();
            if (user == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.Accounts.Where(x => x.Email == userDto.Email).Any() && user.Account!.Email != userDto.Email)
            {
                return BadRequest("Данная электронная почта занята другим аккаунтом");
            }
            if (_context.Accounts.Where(x => x.Login == userDto.Login).Any() && user.Account!.Login != userDto.Login)
            {
                return BadRequest("Данный логин занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PhoneNumber == userDto.PhoneNumber).Any() && user.PhoneNumber != userDto.PhoneNumber)
            {
                return BadRequest("Данный телефонный номер занят другим аккаунтом");
            }
            if (_context.Users.Where(x => x.PassportSeries == userDto.PassportSeries && x.PassportNumber == userDto.PassportNumber).Any()
                && user.PassportNumber != userDto.PassportNumber && user.PassportSeries != userDto.PassportSeries)
            {
                return BadRequest("Данные серия и номер паспорта заняты другим аккаунтом");
            }
            if (userDto.Login != user.Account!.Login || userDto.Email != user.Account!.Email || (userDto.Password != string.Empty && userDto.Password != null))
            {
                _context.Tokens.RemoveRange(_context.Tokens.Where(x => x.Account!.UserId == id));
                await _context.SaveChangesAsync();
            }
            if (userDto.Login != user.Account!.Login || userDto.Email != user.Account!.Email || !string.IsNullOrEmpty(userDto.Password))
            {
                _context.Tokens.RemoveRange(_context.Tokens.Where(x => x.Account!.UserId == id));
                await _context.SaveChangesAsync();
            }

            if (userDto.Password != string.Empty && userDto.Password != null)
            {
                SafeMapper.MapUserFromUserDto(userDto, user);
                user.Account!.Password = HashProvider.MakeHash(userDto.Password);
            }
            else
            {
                SafeMapper.MapUserFromUserDto(userDto, user);
            }

            await _context.SaveChangesAsync();
            return Ok(user);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteUser([FromQuery] Guid id)
        {
            User? delete = await _context.Users.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.TechnicalTests.Where(x => x.User!.Id == delete.Id).Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Users.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }

    }
}
