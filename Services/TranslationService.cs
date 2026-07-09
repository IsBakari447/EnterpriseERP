using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace EnterpriseERP.Services;

public class TranslationService
{
    private readonly IHttpContextAccessor _http;
    public static readonly IReadOnlyDictionary<string, string> SupportedLanguages = new Dictionary<string, string>
    {
        ["fr"] = "Français",
        ["en"] = "English",
        ["sv"] = "Svenska",
        ["es"] = "Español",
        ["de"] = "Deutsch"
    };

    public static readonly IReadOnlyDictionary<string, string> SupportedCultures = new Dictionary<string, string>
    {
        ["fr"] = "fr-FR",
        ["en"] = "en-US",
        ["sv"] = "sv-SE",
        ["es"] = "es-ES",
        ["de"] = "de-DE"
    };

    public TranslationService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string Lang =>
        NormalizeLanguage(
            _http.HttpContext?.Session.GetString("Language")
            ?? _http.HttpContext?.Request.Cookies["Language"]
            ?? CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);

    public string CultureName => SupportedCultures.TryGetValue(Lang, out var culture) ? culture : "fr-FR";

    public string T(string key)
    {
        var dict = Lang switch
        {
            "en" => TranslationRepository.English,
            "sv" => TranslationRepository.Swedish,
            "es" => TranslationRepository.Spanish,
            "de" => TranslationRepository.German,
            _ => TranslationRepository.French
        };

        if (dict.TryGetValue(key, out var value))
            return value;

        return TranslationRepository.French.TryGetValue(key, out var fallback) ? fallback : HumanizeKey(key);
    }

    public static string NormalizeLanguage(string? lang)
    {
        if (string.IsNullOrWhiteSpace(lang))
            return "fr";

        var code = lang.Trim().ToLowerInvariant();
        if (code.Length > 2)
            code = code[..2];

        return SupportedLanguages.ContainsKey(code) ? code : "fr";
    }

    private static string HumanizeKey(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return string.Empty;

        return System.Text.RegularExpressions.Regex.Replace(key, "([a-z])([A-Z])", "$1 $2");
    }
}
