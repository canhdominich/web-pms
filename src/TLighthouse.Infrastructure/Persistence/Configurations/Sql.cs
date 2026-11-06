namespace TLighthouse.Infrastructure.Persistence.Configurations;

internal static class Sql
{
    public const string Now = "CURRENT_TIMESTAMP(6)";
    public const string Varchar255 = "varchar(255)";
    public const int CodePrefix = 50;
    public const int StatusPrefix = 20;
    public const int PhonePrefix = 20;
    public const int EmailPrefix = 100;

    public static string In(string column, IEnumerable<string> values) =>
        $"{column} IN ({string.Join(", ", values.Select(v => $"'{v}'"))})";

    public static string Between(string column, int min, int max) => $"{column} BETWEEN {min} AND {max}";
}
