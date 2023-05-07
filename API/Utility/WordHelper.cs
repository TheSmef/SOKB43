using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Models.Entity;

namespace API.Utility
{
    public static class WordHelper
    {
        public static MemoryStream GetOrderDocument(Order order, List<Equipment> equipments)
        {
            WordprocessingDocument document =
                    WordprocessingDocument.CreateFromTemplate(@"Templates/Word/Шаблон договора.docx", false);
            Body body = document.MainDocumentPart!.Document.Body!;
            var paras = body.Elements<Paragraph>();
            foreach (var para in paras)
            {
                var bookMarkStarts = para.Elements<BookmarkStart>();
                var bookMarkEnds = para.Elements<BookmarkEnd>();

                foreach (BookmarkStart bookMarkStart in bookMarkStarts)
                {
                    if (bookMarkStart != null)
                    {
                        var id = bookMarkStart.Id!.Value;
                        var bookmarkEnd = bookMarkEnds.Where(i => i.Id!.Value == id).First();
                        switch (bookMarkStart.Name!.Value)
                        {
                            case "Sum":
                                {
                                    var runElement = new Run(new Text(order.Sum.ToString("F")));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "Contractor":
                                {
                                    var runElement = new Run(new Text(order.Contractor!.Name));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "IdOrder":
                                {
                                    var runElement = new Run(new Text(order.Id.ToString()));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "OrderDate":
                                {
                                    var runElement = new Run(new Text(order.Date.ToShortDateString()));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                        }
                    }
                }
            }
            IEnumerable<TableProperties> tableProperties = body.Descendants<TableProperties>().Where(tp => tp.TableCaption != null);
            foreach (TableProperties tProp in tableProperties)
            {
                if (tProp.TableCaption!.Val!.Equals("OrderContentTable"))
                {
                    foreach (var equipment in equipments)
                    {
                        Table table = (Table)tProp.Parent!;
                        TableRow tableRow = new TableRow();
                        tableRow.Append(GetStyledCell((equipments.IndexOf(equipment) + 1).ToString()));
                        tableRow.Append(GetStyledCell(equipment.TechnicalTask!.NameEquipment));
                        tableRow.Append(GetStyledCell(equipment.EquipmentCode));
                        tableRow.Append(GetStyledCell(equipment.TechnicalTask!.TypeEquipment!.Name));
                        tableRow.Append(GetStyledCell(equipment.Date.ToShortDateString()));
                        table.Append(tableRow);
                    }

                }
            }
            MemoryStream ms = new MemoryStream();
            document.Clone(ms);
            document.Dispose();
            return ms;
        }

        public static MemoryStream GetServiceDocument(Service service)
        {
            WordprocessingDocument document =
                    WordprocessingDocument.CreateFromTemplate(@"Templates/Word/Шаблон договора обслуживания.docx", false);
            Body body = document.MainDocumentPart!.Document.Body!;
            var paras = body.Elements<Paragraph>();
            foreach (var para in paras)
            {
                var bookMarkStarts = para.Elements<BookmarkStart>();
                var bookMarkEnds = para.Elements<BookmarkEnd>();

                foreach (BookmarkStart bookMarkStart in bookMarkStarts)
                {
                    if (bookMarkStart != null)
                    {
                        var id = bookMarkStart.Id!.Value;
                        var bookmarkEnd = bookMarkEnds.Where(i => i.Id!.Value == id).First();
                        switch (bookMarkStart.Name!.Value)
                        {
                            case "Sum":
                                {
                                    var runElement = new Run(new Text(service.Sum.ToString("F")));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "Contractor":
                                {
                                    var runElement = new Run(new Text(service.Equipment!.Order!.Contractor!.Name));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "IdService":
                                {
                                    var runElement = new Run(new Text(service.Id.ToString()));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                            case "ServiceDate":
                                {
                                    var runElement = new Run(new Text(service.Date.ToShortDateString()));
                                    para.InsertAfter(runElement, bookmarkEnd);
                                    break;
                                }
                        }
                    }
                }
            }
            IEnumerable<TableProperties> tableProperties = body.Descendants<TableProperties>().Where(tp => tp.TableCaption != null);
            foreach (TableProperties tProp in tableProperties)
            {
                if (tProp.TableCaption!.Val!.Equals("TableEquipment"))
                {
                    Table table = (Table)tProp.Parent!;
                    TableRow tableRowCode = new TableRow();
                    tableRowCode.Append(GetStyledCell("1"));
                    tableRowCode.Append(GetStyledCell("Код оборудования"));
                    tableRowCode.Append(GetStyledCell(service.Equipment!.EquipmentCode));
                    TableRow tableRowName = new TableRow();
                    tableRowName.Append(GetStyledCell("2"));
                    tableRowName.Append(GetStyledCell("Название оборудования"));
                    tableRowName.Append(GetStyledCell(service.Equipment!.TechnicalTask!.NameEquipment));
                    TableRow tableRowType = new TableRow();
                    tableRowType.Append(GetStyledCell("3"));
                    tableRowType.Append(GetStyledCell("Тип оборудования"));
                    tableRowType.Append(GetStyledCell(service.Equipment!.TechnicalTask!.TypeEquipment!.Name));
                    TableRow tableRowDate = new TableRow();
                    tableRowDate.Append(GetStyledCell("4"));
                    tableRowDate.Append(GetStyledCell("Дата производства оборудования"));
                    tableRowDate.Append(GetStyledCell(service.Equipment!.Date.ToShortDateString()));
                    table.Append(tableRowCode);
                    table.Append(tableRowName);
                    table.Append(tableRowType);
                    table.Append(tableRowDate);
                }
                else if (tProp.TableCaption!.Val!.Equals("ServiceTable"))
                {
                    Table table = (Table)tProp.Parent!;
                    TableRow tableRowType = new TableRow();
                    tableRowType.Append(GetStyledCell("1"));
                    tableRowType.Append(GetStyledCell("Тип обслуживания"));
                    tableRowType.Append(GetStyledCell(service.ServiceType));
                    TableRow tableRowContent = new TableRow();
                    tableRowContent.Append(GetStyledCell("2"));
                    tableRowContent.Append(GetStyledCell("Содержание работ"));
                    tableRowContent.Append(GetStyledCell(service.WorkContent));
                    table.Append(tableRowType);
                    table.Append(tableRowContent);
                }
            }
            MemoryStream ms = new MemoryStream();
            document.Clone(ms);
            document.Dispose();
            return ms;
        }


        private static TableCell GetStyledCell(string content)
        {
            var tc = new TableCell();
            var paragraph = new Paragraph();
            var run = new Run();
            var text = new Text(content);
            RunProperties runProperties1 = new RunProperties();
            FontSize fontSize = new FontSize() { Val = "20" };
            runProperties1.Append(fontSize);
            Indentation indentation = new Indentation() { Left = "0", Right = "0", FirstLine = "0" };
            Justification justification = new Justification() { Val = JustificationValues.Left };
            SpacingBetweenLines spacing = new SpacingBetweenLines() { Line = "240", LineRule = LineSpacingRuleValues.Auto, Before = "0", After = "0" };
            ParagraphProperties paragraphProperties = new ParagraphProperties();
            TableCellVerticalAlignment tcVA = new TableCellVerticalAlignment() { Val = TableVerticalAlignmentValues.Center };
            TableCellProperties tcp = new TableCellProperties();
            paragraphProperties.Append(justification);
            paragraphProperties.Append(spacing);
            paragraphProperties.Append(indentation);
            tcp.Append(tcVA);
            run.Append(runProperties1);
            run.Append(text);
            paragraph.Append(paragraphProperties);
            paragraph.Append(run);
            tc.Append(paragraph);
            tc.Append(tcp);
            return tc;
        }
    }
}
