namespace EIM.Application.Abstractions;

public sealed record ExcelSheet(string Name, IReadOnlyList<string> Headers, IEnumerable<IReadOnlyList<object?>> Rows);

public sealed record ExcelRow(int Number, IReadOnlyDictionary<string, string> Values)
{
    public string? Get(string column) => Values.TryGetValue(column, out var value) && value.Length > 0 ? value : null;
}

public interface IExcelService
{
    byte[] Write(params ExcelSheet[] sheets);
    IReadOnlyList<ExcelRow> Read(Stream stream);
}
