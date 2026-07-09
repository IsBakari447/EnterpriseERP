using ClosedXML.Excel;
using EnterpriseERP.Services.Export;

namespace EnterpriseERP.Services
{
    public static class ExcelExportService
    {
        public static byte[] ExportTable<T>(
            string sheetName,
            List<string> headers,
            List<List<object?>> rows)
        {
            return ExportTable<T>(
                sheetName,
                headers,
                rows,
                new CompanyBrand(),
                "EnterpriseERP report");
        }

        public static byte[] ExportTable<T>(
            string sheetName,
            List<string> headers,
            List<List<object?>> rows,
            CompanyBrand brand,
            string reportTitle)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(SafeSheetName(sheetName));

            int colCount = headers.Count;
            int lastCol = Math.Max(colCount, 1);

            ws.Cell(1, 1).Value = brand.CompanyName;
            ws.Range(1, 1, 1, lastCol).Merge();
            ws.Cell(1, 1).Style.Font.Bold = true;
            ws.Cell(1, 1).Style.Font.FontSize = 20;
            ws.Cell(1, 1).Style.Font.FontColor = XLColor.White;
            ws.Cell(1, 1).Style.Fill.BackgroundColor = XLColor.FromHtml(brand.PrimaryColor);
            ws.Cell(1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(2, 1).Value = brand.Slogan;
            ws.Range(2, 1, 2, lastCol).Merge();
            ws.Cell(2, 1).Style.Font.Italic = true;
            ws.Cell(2, 1).Style.Font.FontColor = XLColor.White;
            ws.Cell(2, 1).Style.Fill.BackgroundColor = XLColor.FromHtml(brand.PrimaryColor);
            ws.Cell(2, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(3, 1).Value = $"{brand.Address} | {brand.Phone} | {brand.Email} | {brand.Website}";
            ws.Range(3, 1, 3, lastCol).Merge();
            ws.Cell(3, 1).Style.Font.FontColor = XLColor.White;
            ws.Cell(3, 1).Style.Fill.BackgroundColor = XLColor.FromHtml(brand.PrimaryColor);
            ws.Cell(3, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(5, 1).Value = reportTitle;
            ws.Range(5, 1, 5, lastCol).Merge();
            ws.Cell(5, 1).Style.Font.Bold = true;
            ws.Cell(5, 1).Style.Font.FontSize = 16;
            ws.Cell(5, 1).Style.Font.FontColor = XLColor.FromHtml(brand.PrimaryColor);

            ws.Cell(6, 1).Value = $"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}";
            ws.Range(6, 1, 6, lastCol).Merge();
            ws.Cell(6, 1).Style.Font.FontColor = XLColor.Gray;

            int headerRow = 8;

            for (int i = 0; i < headers.Count; i++)
                ws.Cell(headerRow, i + 1).Value = headers[i];

            for (int r = 0; r < rows.Count; r++)
            {
                for (int c = 0; c < rows[r].Count; c++)
                    ws.Cell(headerRow + 1 + r, c + 1).Value = rows[r][c]?.ToString() ?? "";
            }

            FormatTable(ws, headerRow, rows.Count, headers.Count, brand);
            AddFooter(ws, headerRow + rows.Count + 3, lastCol, brand);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void FormatTable(
            IXLWorksheet ws,
            int headerRow,
            int rowCount,
            int colCount,
            CompanyBrand brand)
        {
            if (colCount <= 0)
                return;

            var header = ws.Range(headerRow, 1, headerRow, colCount);
            header.Style.Font.Bold = true;
            header.Style.Font.FontColor = XLColor.White;
            header.Style.Fill.BackgroundColor = XLColor.FromHtml(brand.AccentColor);
            header.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            header.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            header.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

            if (rowCount > 0)
            {
                var tableRange = ws.Range(headerRow, 1, headerRow + rowCount, colCount);
                tableRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                tableRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                var table = tableRange.CreateTable();
                table.Theme = XLTableTheme.TableStyleMedium2;
                table.ShowAutoFilter = true;
            }

            ws.SheetView.FreezeRows(headerRow);
            ws.Columns().AdjustToContents();

            ws.PageSetup.PageOrientation = XLPageOrientation.Landscape;
            ws.PageSetup.FitToPages(1, 0);
            ws.PageSetup.Margins.Top = 0.5;
            ws.PageSetup.Margins.Bottom = 0.5;
        }

        private static void AddFooter(IXLWorksheet ws, int row, int lastCol, CompanyBrand brand)
        {
            ws.Cell(row, 1).Value = brand.FooterMessage;
            ws.Range(row, 1, row, lastCol).Merge();
            ws.Cell(row, 1).Style.Font.Italic = true;
            ws.Cell(row, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Cell(row, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            ws.Cell(row + 1, 1).Value = $"{brand.CompanyName} - Document generated automatically by EnterpriseERP";
            ws.Range(row + 1, 1, row + 1, lastCol).Merge();
            ws.Cell(row + 1, 1).Style.Font.FontSize = 9;
            ws.Cell(row + 1, 1).Style.Font.FontColor = XLColor.Gray;
            ws.Cell(row + 1, 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        private static string SafeSheetName(string name)
        {
            var invalid = new[] { "\\", "/", "?", "*", "[", "]", ":" };

            foreach (var item in invalid)
                name = name.Replace(item, "");

            return string.IsNullOrWhiteSpace(name)
                ? "Report"
                : name.Length > 31 ? name[..31] : name;
        }
    }
}
