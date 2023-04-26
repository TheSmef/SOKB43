using ClosedXML.Excel;
using Models.Entity;

namespace API.Utility
{
    public static class ExcelExporter
    {
        public static XLWorkbook getExcelReport<T>(List<T> values, string worksheetname)
        {
            MemoryStream stream = new MemoryStream();
            XLWorkbook wb = new XLWorkbook();
            wb.AddWorksheet(worksheetname).FirstCell().InsertTable(values).SetShowTotalsRow(true);
            wb.Worksheet(worksheetname).Cells().Style.Alignment.WrapText = true;
            wb.Worksheet(worksheetname).Columns().Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            wb.Worksheet(worksheetname).Columns().Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
            wb.Worksheet(worksheetname).Columns().Width = 50;
            return wb;
        }
    }
}
