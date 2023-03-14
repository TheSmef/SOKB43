using API.Data;
using API.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Models.Dto.PostPutModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Отдел кадров")]
    public class UserPostsController : Controller
    {
        private readonly DataContext _context;

        public UserPostsController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<UserPostsGetDtoModel>> getUserPosts(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.UserPosts.Include(x => x.User)
                .Include(x => x.Post).AsQueryable();
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
            UserPostsGetDtoModel userPostsGetDtoModel = new UserPostsGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            userPostsGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            userPostsGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            userPostsGetDtoModel.CurrentPageIndex = userPostsGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            userPostsGetDtoModel.Collection = await items.ToListAsync();
            return Ok(userPostsGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<UserPost>> getUserPostById(Guid id)
        {
            if (_context.UserPosts.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.UserPosts.Include(x => x.User)
                .Include(x => x.Post).Where(x => x.Id == id).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<UserPost>> postUserPost(UserPostDto userPost)
        {
            if (_context.UserPosts.Where(x => x.User!.Id == userPost.UserId
                && x.Post!.Id == userPost.PostId).Any())
            {
                return BadRequest("Данная должность уже существует у этого сотрудника!");
            }
            if (!_context.Users.Where(x => x.Id == userPost.UserId).Any())
            {
                return BadRequest("Данного пользователя не существует!");
            }
            if (!_context.Posts.Where(x => x.Id == userPost.PostId).Any())
            {
                return BadRequest("Данной должности не существует!");
            }
            UserPost userPostRecord = new UserPost();
            userPostRecord.Share = userPost.Share;
            userPostRecord.User = _context.Users.Where(x => x.Id == userPost.UserId).First();
            userPostRecord.Post = _context.Posts.Where(x => x.Id == userPost.PostId).First();
            userPostRecord.Deleted = userPost.Deleted;
            await _context.UserPosts.AddAsync(userPostRecord);
            await _context.SaveChangesAsync();
            return Ok(userPostRecord);
        }

        [HttpPut]
        public async Task<ActionResult<UserPost>> putUserPost([FromQuery] Guid id, UserPostDto userPostDto)
        {
            UserPost? userPostCheck = await _context.UserPosts.Where(x => x.Id == id).Include(x => x.User)
                .Include(x => x.Post).FirstOrDefaultAsync();
            if (userPostCheck == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.UserPosts.Where(x => x.User!.Id == userPostDto.UserId
                && x.Post!.Id == userPostDto.PostId).Any() && userPostCheck.User!.Id == userPostDto.UserId
                && userPostCheck.Post!.Id != userPostDto.PostId)
            {
                return BadRequest("Данная должность уже существует у этого сотрудника!");
            }
            if (_context.UserPosts.Where(x => x.User!.Id == userPostDto.UserId
                && x.Post!.Id == userPostDto.PostId).Any() && userPostCheck.User!.Id != userPostDto.UserId
                && userPostCheck.Post!.Id == userPostDto.PostId)
            {
                return BadRequest("Данная должность уже существует у этого сотрудника!");
            }
            if (!_context.Users.Where(x => x.Id == userPostDto.UserId).Any())
            {
                return BadRequest("Данного пользователя не существует!");
            }
            if (!_context.Posts.Where(x => x.Id == userPostDto.PostId).Any())
            {
                return BadRequest("Данной должности не существует!");
            }
            userPostCheck.Share = userPostDto.Share;
            userPostCheck.User = _context.Users.Where(x => x.Id == userPostDto.UserId).First();
            userPostCheck.Post = _context.Posts.Where(x => x.Id == userPostDto.PostId).First();
            userPostCheck.Deleted = userPostDto.Deleted;
            await _context.SaveChangesAsync();
            return Ok(userPostCheck);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteUserPost([FromQuery] Guid id)
        {
            UserPost? delete = await _context.UserPosts.Where(x => x.Id == id).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            _context.UserPosts.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
