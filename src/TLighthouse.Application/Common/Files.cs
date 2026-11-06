namespace TLighthouse.Application.Common;

public sealed record FileContent(byte[] Content, string FileName)
{
    public const string ExcelContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
}

public sealed record ImportRowError(int Row, string Message);

public sealed record ImportResult(int Imported, IReadOnlyList<ImportRowError> Errors)
{
    public bool Succeeded => Errors.Count == 0;
}
