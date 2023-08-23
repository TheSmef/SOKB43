using API.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Models.Dto.FileModels;

namespace API.Controllers.UtilityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Администратор")]
    public class BackUpController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly string _backupFolderFullPath = AppDomain.CurrentDomain.BaseDirectory + "Backup\\backup.bak";
        private readonly string _directory = AppDomain.CurrentDomain.BaseDirectory + "Backup";
        private readonly DataContext _context;
        private readonly string _dataBaseName;


        public BackUpController(IConfiguration configuration, DataContext _context)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DatabaseConnection")!;
            _dataBaseName = _configuration.GetConnectionString("DatabaseConnection")!.Split(";")
                .Where(x => x.Contains("Database")).First().Trim().Replace("Database=", "");
            if (!Directory.Exists(_directory))
            {
                Directory.CreateDirectory(_directory);
            }
            this._context = _context;
        }

        [HttpGet]
        public async Task<ActionResult<FileModel>> BackupDatabase(CancellationToken ct)
        {
            try
            {
                if (!System.IO.File.Exists(_backupFolderFullPath))
                {
                    System.IO.File.Create(_backupFolderFullPath).Close();
                }
                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = String.Format($"BACKUP DATABASE [{_dataBaseName}] TO DISK='{_backupFolderFullPath}' WITH INIT, FORMAT");

                    using (var command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync(ct);
                    }
                    using (FileStream reader = new FileStream(_backupFolderFullPath, FileMode.Open))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            await reader.CopyToAsync(ms);
                            return new FileModel() { Name = $"BACKUP_{DateTime.Today.ToShortDateString()}.bak", Data = ms.ToArray() };
                        }
                    }

                }
            }
            catch
            {
                return BadRequest("Ошибка при запросе к базе данных, попробуйте позже");
            }

        }

        [HttpPut]
        public async Task<ActionResult> RestoreBatabase(byte[] data, CancellationToken ct)
        {
            try
            {
                if (!System.IO.File.Exists(_backupFolderFullPath))
                {
                    System.IO.File.Create(_backupFolderFullPath).Close();
                }
                using (MemoryStream ms = new MemoryStream(data))
                {
                    await System.IO.File.WriteAllBytesAsync(_backupFolderFullPath, ms.ToArray());
                }

                using (var connection = new SqlConnection(_connectionString))
                {
                    var query = String.Format($"USE MASTER RESTORE DATABASE [{_dataBaseName}] FROM DISK='{_backupFolderFullPath}' WITH REPLACE");
                    using (var command = new SqlCommand(query, connection))
                    {
                        await connection.OpenAsync();
                        await command.ExecuteNonQueryAsync(ct);
                    }
                }
                var entitiesList = _context.ChangeTracker.Entries().ToList();
                foreach (var entity in entitiesList)
                {
                    entity.State = EntityState.Detached;
                }

                return Ok("База данных успешно восстановлена");

            }
            catch
            {
                return BadRequest("Произошла ошибка при восстановлении базы данных, проверьте корректность файла или попробуйте позже");
            }
        }
    }
}
