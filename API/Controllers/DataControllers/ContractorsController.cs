using API.Data;
using API.Utility;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Models.Dto.GetModels;
using Models.Entity;
using Models.QuerySupporter;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.AspNetCore.Authorization;
using ClosedXML.Excel;
using Models.Dto.FileModels;
using AutoMapper.Configuration;
using System.ComponentModel.DataAnnotations;

namespace API.Controllers.DataControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор, Менеджер по работе с клиентами")]
    public class ContractorsController : Controller
    {
        private readonly DataContext _context;
        public ContractorsController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("Import")]
        public async Task<ActionResult> importContractors(byte[] data)
        {
            List<Contractor> contractors = new List<Contractor>();
            try
            {
                contractors = ExcelExporter.getImportModel<Contractor>(data, "Контрагенты");
            }
            catch
            {
                return BadRequest("Файл импортирования некорректен");
            }

            if (contractors.Count == 0)
            {
                return BadRequest("Файл импортирования не содержит данных для импортирования");
            }
            foreach (var contractor in contractors)
            {
                System.ComponentModel.DataAnnotations.ValidationContext validationContext 
                    = new System.ComponentModel.DataAnnotations.ValidationContext(contractor);
                if (!Validator.TryValidateObject(contractor, validationContext, null, true)
                    || _context.Contractors.Where(x => x.Name == contractor.Name).Any()
                    || _context.Contractors.Where(x => x.Email == contractor.Email).Any()
                    || _context.Contractors.Where(x => x.PhoneNumber == contractor.PhoneNumber).Any())
                {
                    return BadRequest($"Ошибка валидации внутри файла импортирования, исправьте ошибку валидации и повторите попытку (Ошибка на строке {contractors.IndexOf(contractor) + 2})");
                }
            }
            await _context.Contractors.AddRangeAsync(contractors);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Export")]
        public async Task<ActionResult<FileModel>> exportContractors(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Contractors.AsQueryable();
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
            using (MemoryStream ms = new MemoryStream())
            {
                XLWorkbook wb = ExcelExporter.getExcelReport(await items.ToListAsync(), "Контрагенты");
                wb.SaveAs(ms);
                FileModel response = new FileModel() { Name = $"Контрагенты_{DateTime.Today.ToShortDateString()}.xlsx", Data = ms.ToArray()};
                return Ok(response);
            }
        }

        [HttpGet]
        public async Task<ActionResult<ContractorsGetDtoModel>> getContractors(
            [FromQuery] QuerySupporter query)
        {
            var items = _context.Contractors.AsQueryable();
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
            ContractorsGetDtoModel contractorsGetDtoModel = new ContractorsGetDtoModel();
            if (query.Skip <= -1 || query.Top <= 0)
            {
                return BadRequest("Неправильный формат строки запроса!");
            }
            contractorsGetDtoModel.TotalPages = PageCounter.CountPages(items.Count(), query.Top);
            contractorsGetDtoModel.ElementsCount = items.Count();
            items = items.Skip(query.Skip);
            contractorsGetDtoModel.CurrentPageIndex = contractorsGetDtoModel.TotalPages + 1 - PageCounter.CountPages(items.Count(), query.Top);
            items = items.Take(query.Top);
            contractorsGetDtoModel.Collection = await items.ToListAsync();
            
            return Ok(contractorsGetDtoModel);
        }

        [HttpGet("single")]
        public async Task<ActionResult<Contractor>> getContractorById(Guid id)
        {
            if (_context.Contractors.Where(x => x.Id == id).Any())
            {
                return Ok(await _context.Contractors.Where(x => x.Id == id).FirstAsync());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<ActionResult<Contractor>> postContractor(Contractor conctractor)
        {
            if (_context.Contractors.Where(x => x.Name == conctractor.Name).Any())
            {
                return BadRequest("Контрагент с данным названием уже существует!");
            }
            if (_context.Contractors.Where(x => x.Email == conctractor.Email).Any())
            {
                return BadRequest("Контрагент с данной электронной почтой уже существует!");
            }
            if (_context.Contractors.Where(x => x.PhoneNumber == conctractor.PhoneNumber).Any())
            {
                return BadRequest("Контрагент с данным номером телефона уже существует!");
            }
            await _context.Contractors.AddAsync(conctractor);
            await _context.SaveChangesAsync();
            return Ok(conctractor);
        }

        [HttpPut]
        public async Task<ActionResult<Contractor>> putContractor([FromQuery] Guid id, Contractor conctractor)
        {
            Contractor? contractorCheck = await _context.Contractors.Where(x => x.Id == id).AsNoTracking().FirstOrDefaultAsync();
            if (contractorCheck == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (_context.Contractors.Where(x => x.Name == conctractor.Name).Any() && conctractor.Name != contractorCheck.Name)
            {
                return BadRequest("Контрагент с данным названием уже существует!");
            }
            if (_context.Contractors.Where(x => x.Email == conctractor.Email).Any() && conctractor.Email != contractorCheck.Email)
            {
                return BadRequest("Контрагент с данной электронной почтой уже существует!");
            }
            if (_context.Contractors.Where(x => x.PhoneNumber == conctractor.PhoneNumber).Any() && conctractor.PhoneNumber != contractorCheck.PhoneNumber)
            {
                return BadRequest("Контрагент с данным номером телефона уже существует!");
            }
            conctractor.Id = id;
            conctractor.Orders = contractorCheck.Orders;
            _context.Update(conctractor);
            await _context.SaveChangesAsync();
            return Ok(conctractor);
        }


        [HttpDelete]
        public async Task<ActionResult> deleteContractor([FromQuery] Guid id)
        {
            Contractor? delete = await _context.Contractors.Where(x => x.Id == id).Include(x => x.Orders).FirstOrDefaultAsync();
            if (delete == null)
            {
                return BadRequest("Запись не существует!");
            }
            if (delete.Orders!.Any())
            {
                return BadRequest("Невозможно удалить запись, у неё присутствуют дочерние записи!");
            }
            _context.Contractors.Remove(delete);
            await _context.SaveChangesAsync();
            return Ok("Запись удалена");
        }
    }
}
