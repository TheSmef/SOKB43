using ClosedXML.Attributes;
using Models.Entity;
using Models.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Models.Entity.Equipment;

namespace Models.ExportModels
{
    public class EquipmentExportModel
    {
        [XLColumn(Header = "Тип оборудования")]
        public string? TypeEquipment { get; set; }
        [XLColumn(Header = "Название оборудования")]
        public string? NameEquipment { get; set; }
        [XLColumn(Header = "Статус")]
        public string? Status { get; set; }
        [XLColumn(Header = "Код оборудования")]
        public string? EquipmentCode { get; set; }
        [XLColumn(Header = "Дата создания оборудования")]
        public DateTime Date { get; set; }
    }
}
