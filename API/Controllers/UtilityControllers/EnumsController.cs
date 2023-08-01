using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Models.Entity;
using Models.Utility;
using NuGet.Protocol;
using System.Data;
using static Models.Entity.Equipment;
using static Models.Entity.Role;
using static Models.Entity.Service;
using static Models.Entity.TechnicalTest;

namespace API.Controllers.UtilityControllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnumsController : ControllerBase
    {
        [HttpGet("ServiceType")]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел обслуживания")]
        public ActionResult getServiceTypes()
        {
            return Ok(EnumUtility.GetStringsValues(typeof(ServiceTypeEnum)));
        }

        [HttpGet("EquipmentStatus")]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел тестирования, Отдел обслуживания")]
        public ActionResult<List<string>> getEquipmentStatuses()
        {
            return Ok(EnumUtility.GetStringsValues(typeof(EquipmentStatusEnum)));
        }

        [HttpGet("ServiceStatus")]
        [Authorize(Roles = "Администратор, Менеджер по работе с клиентами, Отдел обслуживания")]
        public ActionResult<List<string>> getServiceStatuses()
        {
            return Ok(EnumUtility.GetStringsValues(typeof(ServiceStatusEnum)));
        }

        [HttpGet("TestPriority")]
        [Authorize(Roles = "Администратор, Отдел тестирования")]
        public ActionResult<List<string>> getTestPriorities()
        {
            return Ok(EnumUtility.GetStringsValues(typeof(TestPriorityEnum)));
        }


        [HttpGet("Roles")]
        [Authorize(Roles = "Администратор, Отдел кадров")]
        public ActionResult<List<string>> getRoles()
        {
            return Ok(EnumUtility.GetStringsValues(typeof(NameRole)));
        }
    }
}
