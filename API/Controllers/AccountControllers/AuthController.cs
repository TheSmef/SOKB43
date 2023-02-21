﻿using API.Data;
using API.Security;
using API.Utility;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Models.Dto.PostPutModels;
using Models.Dto.PostPutModels.AccountModels;
using Models.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;

namespace API.Controllers.AccountsController
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        private readonly DataContext _context;

        public AuthController(DataContext context, IMapper mapper,
            IConfiguration configuration)
        {
            _context = context;
            _mapper = mapper;
            _configuration = configuration;
        }
        [HttpPost]
        public async Task<ActionResult> signUn(RegModel model)
        {
            User user = _mapper.Map<User>(model);
            user.Account = _mapper.Map<Account>(model);
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
            user.Account!.Password = HashProvider.MakeHash(model.Password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            string refresttoken = Utility.TokenHandler.GenerateRefreshToken();

            Token token = new Token();
            token.Account = user.Account;
            token.TokenStr = refresttoken;
            await _context.Tokens.AddAsync(token);
            await _context.SaveChangesAsync();

            return Ok(refresttoken);
        }

        [HttpPut]
        public async Task<ActionResult> signIn(AuthModel model)
        {
            Account? account = await _context.Accounts.Where(x => x.Email == model.Login || x.Login == model.Login).Include(x => x.User).FirstOrDefaultAsync();
            if (account == null)
            {
                return NotFound();
            }
            if (HashProvider.CheckHash(model.Password, account.Password))
            {
                if (!model.RememberMe)
                {
                    var jwt = Utility.TokenHandler.CreateJwtToken(account.User!, _configuration);
                    return Ok(jwt);
                }
                string refresttoken = Utility.TokenHandler.GenerateRefreshToken();
                Token token = new Token();
                token.Account = account;
                token.TokenStr = refresttoken;
                await _context.Tokens.AddAsync(token);
                await _context.SaveChangesAsync();


                return Ok(refresttoken);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPut("GetToken")]
        public async Task<ActionResult> getJwtToken([FromBody] string refreshtoken)
        {
            Token? token = await _context.Tokens.Where(x => x.TokenStr == refreshtoken)
                .Include(x => x.Account).ThenInclude(x => x!.User)
                .Include(x => x.Account).ThenInclude(x => x!.Roles)
                .FirstOrDefaultAsync();
            if (token == null)
            {
                return NotFound();
            }
            var jwt = Utility.TokenHandler.CreateJwtToken(token.Account!.User!, _configuration);

            return Ok(jwt);
        }

    }
}