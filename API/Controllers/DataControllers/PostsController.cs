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
using System.Collections.Generic;
using System.Linq;
using Models.QuerySupporter;
using API.Utility;
using Models.Dto.GetModels;
using Models.Dto.PostPutModels;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Microsoft.IdentityModel.Tokens;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Отдел кадров")]
    public class PostsController : Controller
    {
        private readonly DataContext _context;

        public PostsController(DataContext context)
        {
            _context = context;
        }


        [HttpGet]
        public async Task<ActionResult<PostsGetDtoModel>> getPosts(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Posts.AsQueryable();
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
            PostsGetDtoModel postsGetDtoModel = new PostsGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            postsGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            postsGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            postsGetDtoModel.CurrentPageIndex = postsGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            postsGetDtoModel.Collection = await items.ToListAsync();
            return Ok(postsGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<Post>> getPostById(Guid id)
        {
            if (_context.Posts.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Posts.Where(x => x.Id == id).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Post>> postPost(Post post)
        {
            if (_context.Posts.Where(x => x.Name == post.Name).Any())
            {
                return BadRequest("Данная должность уже существует!");
            }
            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();
            return Ok(post);
        }

        [HttpPut]
        public async Task<ActionResult<Post>> putPost([FromQuery] Guid id, Post post)
        {
            Post? postCheck = await _context.Posts.Where(x => x.Id == id).Include(x => x.UserPosts).AsNoTracking().FirstOrDefaultAsync();
            if (postCheck == null)
            {
                return BadRequest("Такой должности не существует");
            }
            if (_context.Posts.Where(x => x.Name == post.Name).Any() && post.Name != postCheck.Name)
            {
                return BadRequest("Данная должность уже существует!");
            }
            post.Id = id;
            post.UserPosts = postCheck.UserPosts;
            _context.Update(post);
            await _context.SaveChangesAsync();
            return Ok(post);
        }


        [HttpDelete]
        public async Task<ActionResult> deletePost([FromQuery] Guid id)
        {
            Post? delete = await _context.Posts.Where(x => x.Id == id).Include(x => x.UserPosts).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Записи не существует!");
            }
            if (delete.UserPosts!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Posts.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
