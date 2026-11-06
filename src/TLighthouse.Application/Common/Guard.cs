namespace TLighthouse.Application.Common;

public static class Guard
{
    public static string OneOf(string? value, IReadOnlyCollection<string> allowed, string field)
    {
        var normalized = value?.Trim().ToUpperInvariant();
        if (normalized is null || !allowed.Contains(normalized))
            throw AppException.Invalid("INVALID_VALUE", $"{field} phải là một trong: {string.Join(", ", allowed)}");
        return normalized;
    }

    public static string? OptionalOneOf(string? value, IReadOnlyCollection<string> allowed, string field) =>
        string.IsNullOrWhiteSpace(value) ? null : OneOf(value, allowed, field);

    public static string? Clean(this string? value) => string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}
