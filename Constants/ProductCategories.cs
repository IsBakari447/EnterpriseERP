namespace EnterpriseERP.Constants;

public static class ProductCategories
{
    public static readonly string[] All =
    [
        "EnterpriseERP",
        "Mobile",
        "Cloud"
    ];

    public static bool IsValid(string? category)
    {
        return All.Contains(category, StringComparer.OrdinalIgnoreCase);
    }

    public static string Normalize(string? category)
    {
        return All.FirstOrDefault(item => string.Equals(item, category, StringComparison.OrdinalIgnoreCase))
            ?? All[0];
    }
}
