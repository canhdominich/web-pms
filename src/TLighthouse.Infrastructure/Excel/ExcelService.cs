using ClosedXML.Excel;
using TLighthouse.Application.Abstractions;
using TLighthouse.Application.Common;

namespace TLighthouse.Infrastructure.Excel;

public sealed class ExcelService : IExcelService
{
    private const int MaxRows = 5000;

    public byte[] Write(params ExcelSheet[] sheets)
    {
        using var workbook = new XLWorkbook();
        foreach (var sheet in sheets)
        {
            var worksheet = workbook.Worksheets.Add(sheet.Name);
            for (var c = 0; c < sheet.Headers.Count; c++) worksheet.Cell(1, c + 1).Value = sheet.Headers[c];

            var header = worksheet.Row(1);
            header.Style.Font.Bold = true;
            header.Style.Fill.BackgroundColor = XLColor.LightGray;

            var r = 2;
            foreach (var row in sheet.Rows)
            {
                for (var c = 0; c < row.Count; c++) SetValue(worksheet.Cell(r, c + 1), row[c]);
                r++;
            }
            worksheet.SheetView.FreezeRows(1);
            worksheet.Columns(1, Math.Max(1, sheet.Headers.Count)).Width = 20;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public IReadOnlyList<ExcelRow> Read(Stream stream)
    {
        try
        {
            using var workbook = new XLWorkbook(stream);
            var worksheet = workbook.Worksheets.First();
            var range = worksheet.RangeUsed();
            if (range is null) return [];

            var firstRow = range.FirstRow().RowNumber();
            var lastRow = range.LastRow().RowNumber();
            var lastColumn = range.LastColumn().ColumnNumber();
            if (lastRow - firstRow > MaxRows)
                throw AppException.Invalid("FILE_TOO_LARGE", $"Tệp tối đa {MaxRows} dòng dữ liệu");

            var headers = Enumerable.Range(1, lastColumn)
                .Select(c => worksheet.Cell(firstRow, c).GetFormattedString().Trim().ToLowerInvariant())
                .ToList();

            var rows = new List<ExcelRow>();
            for (var r = firstRow + 1; r <= lastRow; r++)
            {
                var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                for (var c = 1; c <= lastColumn; c++)
                    if (headers[c - 1].Length > 0) values[headers[c - 1]] = worksheet.Cell(r, c).GetFormattedString().Trim();
                if (values.Values.Any(v => v.Length > 0)) rows.Add(new ExcelRow(r, values));
            }
            return rows;
        }
        catch (Exception ex) when (ex is not AppException)
        {
            throw AppException.Invalid("INVALID_FILE", "Tệp Excel không hợp lệ");
        }
    }

    private static void SetValue(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null:
                cell.Value = Blank.Value;
                break;
            case DateOnly date:
                cell.Value = date.ToDateTime(TimeOnly.MinValue);
                cell.Style.DateFormat.Format = "yyyy-mm-dd";
                break;
            case DateTime dateTime:
                cell.Value = dateTime;
                cell.Style.DateFormat.Format = "yyyy-mm-dd hh:mm:ss";
                break;
            case int number:
                cell.Value = number;
                break;
            case long number:
                cell.Value = number;
                break;
            case double number:
                cell.Value = number;
                break;
            case decimal number:
                cell.Value = number;
                break;
            case bool flag:
                cell.Value = flag;
                break;
            default:
                cell.Value = value.ToString();
                break;
        }
    }
}
