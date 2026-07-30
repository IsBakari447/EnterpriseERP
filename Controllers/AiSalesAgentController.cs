using System.Text;
using EnterpriseERP.Attributes;
using EnterpriseERP.Services;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseERP.Controllers;

public class AiSalesAgentController : Controller
{
    private readonly TranslationService _translation;

    private static readonly string[] Sectors =
    {
        "Commerce",
        "Restauration",
        "Services",
        "Industrie",
        "Sante",
        "Education",
        "Immobilier",
        "Transport",
        "Construction"
    };

    private static readonly string[] Objectives =
    {
        "Prospection",
        "Relance",
        "Presentation",
        "Proposition",
        "Demo",
        "Objection",
        "Fidelisation"
    };

    public AiSalesAgentController(TranslationService translation)
    {
        _translation = translation;
    }

    [RequirePermission("IA", "Voir")]
    public IActionResult Index()
    {
        ViewBag.Sectors = Sectors;
        ViewBag.Objectives = Objectives;
        ViewBag.Result = BuildDefaultResult(_translation.Lang);
        return View(new SalesAgentRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("IA", "Voir")]
    public IActionResult Generate(SalesAgentRequest request)
    {
        request.Sector = Normalize(request.Sector, "Commerce");
        request.Objective = Normalize(request.Objective, "Prospection");
        request.CompanyName = Normalize(request.CompanyName, DefaultCompany(_translation.Lang));
        request.Offer = Normalize(request.Offer, DefaultOffer(_translation.Lang));
        request.Target = Normalize(request.Target, DefaultTarget(_translation.Lang));
        request.Context = Normalize(request.Context, DefaultContext(_translation.Lang));

        ViewBag.Sectors = Sectors;
        ViewBag.Objectives = Objectives;
        ViewBag.Result = BuildResult(request, _translation.Lang);
        return View("Index", request);
    }

    private static SalesAgentResult BuildDefaultResult(string lang)
    {
        return BuildResult(new SalesAgentRequest
        {
            Sector = "Commerce",
            Objective = "Prospection",
            CompanyName = "EnterpriseERP",
            Offer = DefaultOffer(lang),
            Target = DefaultTarget(lang),
            Context = DefaultContext(lang)
        }, lang);
    }

    private static SalesAgentResult BuildResult(SalesAgentRequest request, string lang)
    {
        var pain = GetPainPoint(request.Sector, lang);
        var value = GetValueProposition(request.Sector, lang);
        var sector = SectorLabel(request.Sector, lang);
        var objective = ObjectiveLabel(request.Objective, lang);

        return new SalesAgentResult
        {
            SectorAnalysis = BuildSectorAnalysis(request, sector, objective, pain, value, lang),
            Email = BuildEmail(request, sector, pain, value, lang),
            CallScript = BuildCallScript(request, sector, pain, value, lang),
            LinkedInMessage = BuildLinkedInMessage(request, sector, value, lang),
            WhatsAppMessage = BuildWhatsAppMessage(request, sector, value, lang),
            SalesPage = BuildSalesPage(request, sector, pain, value, lang),
            ProspectingActions = BuildProspectingActions(request, sector, lang)
        };
    }

    private static string BuildSectorAnalysis(SalesAgentRequest request, string sector, string objective, string pain, string value, string lang)
    {
        return lang switch
        {
            "en" => $"""
            Analyzed sector: {sector}
            Target: {request.Target}
            Sales objective: {objective}

            Likely main problem:
            {pain}

            Recommended sales angle:
            Position {request.Offer} as a simple way to save time, secure sales follow-up and give managers a clear business view.

            Value proposition:
            {value}
            """,
            "sv" => $"""
            Analyserad sektor: {sector}
            Malgrupp: {request.Target}
            Saljmal: {objective}

            Troligt huvudproblem:
            {pain}

            Rekommenderad saljvinkel:
            Positionera {request.Offer} som ett enkelt satt att spara tid, sakra uppfoljning och ge ledningen tydlig oversikt.

            Vardeforslag:
            {value}
            """,
            _ => $"""
            Secteur analyse : {sector}
            Cible : {request.Target}
            Objectif commercial : {objective}

            Probleme principal probable :
            {pain}

            Angle de vente recommande :
            Positionner {request.Offer} comme une solution simple pour gagner du temps, securiser le suivi commercial et donner une vision claire au dirigeant.

            Proposition de valeur :
            {value}
            """
        };
    }

    private static string BuildEmail(SalesAgentRequest request, string sector, string pain, string value, string lang)
    {
        return lang switch
        {
            "en" => $"""
            Subject: A simpler way to manage {request.CompanyName}

            Hello,

            I am contacting you because companies in {sector.ToLowerInvariant()} often face the same challenge: {pain.ToLowerInvariant()}.

            With {request.Offer}, you can centralize customers, sales, invoices, payments, stock and follow-ups in one professional tool available on web, cloud and mobile.

            {value}

            Would you be available this week for a short 15-minute demo?

            Best regards,
            The EnterpriseERP team
            """,
            "sv" => $"""
            Amne: Ett enklare satt att styra {request.CompanyName}

            Hej,

            Jag kontaktar dig eftersom foretag inom {sector.ToLowerInvariant()} ofta har samma utmaning: {pain.ToLowerInvariant()}.

            Med {request.Offer} kan ni samla kunder, forsäljning, fakturor, betalningar, lager och uppfoljningar i ett professionellt verktyg for webb, moln och mobil.

            {value}

            Har du mojlighet till en kort demo pa 15 minuter denna vecka?

            Vanliga halsningar,
            EnterpriseERP-teamet
            """,
            _ => $"""
            Objet : Une facon plus simple de piloter {request.CompanyName}

            Bonjour,

            Je me permets de vous contacter car les entreprises du secteur {sector.ToLowerInvariant()} rencontrent souvent le meme defi : {pain.ToLowerInvariant()}.

            Avec {request.Offer}, vous pouvez centraliser vos clients, ventes, factures, paiements, stock et relances dans un seul outil professionnel, accessible depuis le web, le cloud et le mobile.

            {value}

            Seriez-vous disponible cette semaine pour une courte presentation de 15 minutes ?

            Cordialement,
            L'equipe EnterpriseERP
            """
        };
    }

    private static string BuildCallScript(SalesAgentRequest request, string sector, string pain, string value, string lang)
    {
        return lang switch
        {
            "en" => $"""
            Opening:
            Hello, I am calling briefly about the sales management of {request.CompanyName}. Is this a bad time?

            Hook:
            Many companies in {sector.ToLowerInvariant()} lose time tracking customers, invoices, stock and follow-ups across several tools.

            Discovery:
            How do you currently track customers, sales and payments?

            Proposal:
            EnterpriseERP helps centralize these actions and automate part of the sales follow-up.

            Value:
            {value}

            Price objection:
            I understand. The goal is first to measure saved time, avoided mistakes and better followed sales before discussing budget.

            Close:
            I suggest a short 15-minute demo to see if it fits your organization.
            """,
            "sv" => $"""
            Inledning:
            Hej, jag ringer kort om saljstyrningen hos {request.CompanyName}. Storar jag?

            Krok:
            Manga foretag inom {sector.ToLowerInvariant()} tappar tid pa att folja kunder, fakturor, lager och uppfoljningar i flera verktyg.

            Behovsfraga:
            Hur foljer ni kunder, forsaljning och betalningar idag?

            Forslag:
            EnterpriseERP hjalper er att samla arbetet och automatisera delar av saljuppfoljningen.

            Varde:
            {value}

            Prisinvandning:
            Jag forstar. Forst vill vi visa tidsvinsten, farre missar och battre saljuppfoljning innan budget diskuteras.

            Avslut:
            Jag foreslar en kort demo pa 15 minuter for att se om det passar er organisation.
            """,
            _ => $"""
            Introduction :
            Bonjour, je vous appelle rapidement au sujet de la gestion commerciale de {request.CompanyName}. Est-ce que je vous derange ?

            Accroche :
            Beaucoup d'entreprises dans le secteur {sector.ToLowerInvariant()} perdent du temps a suivre les clients, les factures, le stock et les relances dans plusieurs outils.

            Diagnostic :
            Aujourd'hui, comment suivez-vous vos clients, vos ventes et vos paiements ?

            Proposition :
            EnterpriseERP peut vous aider a centraliser ces actions et a automatiser une partie du suivi commercial.

            Valeur :
            {value}

            Objection prix :
            Je comprends. L'objectif est justement de mesurer le temps gagne, les oublis evites et les ventes mieux suivies avant de parler budget.

            Conclusion :
            Je vous propose une demo courte de 15 minutes pour voir si cela correspond a votre organisation.
            """
        };
    }

    private static string BuildLinkedInMessage(SalesAgentRequest request, string sector, string value, string lang)
    {
        return lang switch
        {
            "en" => $"Hello,\n\nI saw that you work in {sector.ToLowerInvariant()}. We help {request.Target.ToLowerInvariant()} organize sales, customers, invoices and follow-ups with EnterpriseERP.\n\n{value}\n\nOpen to a short discussion this week?",
            "sv" => $"Hej,\n\nJag sag att du arbetar inom {sector.ToLowerInvariant()}. Vi hjalper {request.Target.ToLowerInvariant()} att organisera forsaljning, kunder, fakturor och uppfoljning med EnterpriseERP.\n\n{value}\n\nOppet for ett kort samtal denna vecka?",
            _ => $"Bonjour,\n\nJ'ai vu que vous travaillez dans le secteur {sector.ToLowerInvariant()}. Nous aidons les {request.Target.ToLowerInvariant()} a mieux organiser leurs ventes, clients, factures et relances avec EnterpriseERP.\n\n{value}\n\nOuvert a une courte discussion cette semaine ?"
        };
    }

    private static string BuildWhatsAppMessage(SalesAgentRequest request, string sector, string value, string lang)
    {
        return lang switch
        {
            "en" => $"Hello, I am contacting you about {request.CompanyName}.\nEnterpriseERP helps {sector.ToLowerInvariant()} companies manage customers, sales, invoices, stock and follow-ups from one platform.\n{value}\nMay I send you a demo link?",
            "sv" => $"Hej, jag kontaktar dig om {request.CompanyName}.\nEnterpriseERP hjalper foretag inom {sector.ToLowerInvariant()} att hantera kunder, forsaljning, fakturor, lager och uppfoljningar fran en plattform.\n{value}\nKan jag skicka en demolank?",
            _ => $"Bonjour, je vous contacte au sujet de {request.CompanyName}.\nEnterpriseERP aide les entreprises du secteur {sector.ToLowerInvariant()} a gerer clients, ventes, factures, stock et relances depuis une seule plateforme.\n{value}\nJe peux vous envoyer un lien de demo ?"
        };
    }

    private static string BuildSalesPage(SalesAgentRequest request, string sector, string pain, string value, string lang)
    {
        var builder = new StringBuilder();
        if (lang == "en")
        {
            builder.AppendLine($"Title: EnterpriseERP for {sector}");
            builder.AppendLine($"Hero: Manage {request.CompanyName} with an intelligent sales and mobile ERP.");
            builder.AppendLine($"Problem: {pain}");
            builder.AppendLine($"Solution: {request.Offer} centralizes prospecting, customers, quotes, invoices, payments, stock and reports.");
            builder.AppendLine($"Proof of value: {value}");
            builder.AppendLine("Recommended sections: sector problem, adapted modules, dashboard demo, simple pricing, call to action.");
            return builder.ToString();
        }

        if (lang == "sv")
        {
            builder.AppendLine($"Titel: EnterpriseERP for {sector}");
            builder.AppendLine($"Hero: Styr {request.CompanyName} med ett intelligent, kommersiellt och mobilt ERP.");
            builder.AppendLine($"Problem: {pain}");
            builder.AppendLine($"Losning: {request.Offer} samlar prospektering, kunder, offerter, fakturor, betalningar, lager och rapporter.");
            builder.AppendLine($"Vardebevis: {value}");
            builder.AppendLine("Rekommenderade sektioner: sektorproblem, anpassade moduler, dashboard-demo, enkla priser, call to action.");
            return builder.ToString();
        }

        builder.AppendLine($"Titre : EnterpriseERP pour le secteur {sector}");
        builder.AppendLine($"Hero : Pilotez {request.CompanyName} avec un ERP intelligent, commercial et mobile.");
        builder.AppendLine($"Probleme : {pain}");
        builder.AppendLine($"Solution : {request.Offer} centralise la prospection, les clients, les devis, les factures, les paiements, le stock et les rapports.");
        builder.AppendLine($"Preuve de valeur : {value}");
        builder.AppendLine("Sections recommandees : probleme du secteur, modules adaptes, demo dashboard, tarifs simples, appel a l'action.");
        return builder.ToString();
    }

    private static string[] BuildProspectingActions(SalesAgentRequest request, string sector, string lang)
    {
        return lang switch
        {
            "en" => new[]
            {
                $"List 30 companies in {sector} in the target area.",
                "Identify the owner, sales manager or administrative manager.",
                "Send a short LinkedIn message, then a personalized email the next day.",
                "Follow up after 48 hours with a clear business angle and demo proposal.",
                "Log every response in EnterpriseERP to track the sales pipeline."
            },
            "sv" => new[]
            {
                $"Lista 30 foretag inom {sector} i malomradet.",
                "Identifiera agare, saljansvarig eller administrativ ansvarig.",
                "Skicka ett kort LinkedIn-meddelande och sedan ett personligt mejl dagen efter.",
                "Folj upp efter 48 timmar med tydlig affarsvinkel och demoerbjudande.",
                "Registrera varje svar i EnterpriseERP for att folja saljpipelinen."
            },
            _ => new[]
            {
                $"Lister 30 entreprises du secteur {sector} dans la zone cible.",
                "Identifier le dirigeant, le responsable commercial ou le responsable administratif.",
                "Envoyer un message LinkedIn court puis un email personnalise le lendemain.",
                "Relancer apres 48 heures avec un angle metier clair et une proposition de demo.",
                "Noter chaque reponse dans EnterpriseERP pour suivre le pipeline commercial."
            }
        };
    }

    private static string GetPainPoint(string sector, string lang)
    {
        return (sector.ToLowerInvariant(), lang) switch
        {
            ("commerce", "en") => "sales, stock, orders and customer follow-ups are often spread across several tools",
            ("commerce", "sv") => "forsaljning, lager, order och kunduppfoljning ar ofta utspridda i flera verktyg",
            ("commerce", _) => "les ventes, le stock, les commandes et les relances clients sont souvent disperses entre plusieurs outils",
            (_, "en") => "business information is scattered and difficult to track every day",
            (_, "sv") => "affarsinformation ar utspridd och svar att folja dagligen",
            _ => "les informations commerciales sont dispersees et difficiles a suivre au quotidien"
        };
    }

    private static string GetValueProposition(string sector, string lang)
    {
        return (sector.ToLowerInvariant(), lang) switch
        {
            ("commerce", "en") => "You get a clear view of products, sales, low stock and customers to follow up.",
            ("commerce", "sv") => "Du far tydlig oversikt over produkter, forsaljning, lagt lager och kunder att folja upp.",
            ("commerce", _) => "Vous gagnez une vue claire sur les produits, les ventes, les stocks faibles et les clients a relancer.",
            (_, "en") => "You save time and improve the quality of sales follow-up.",
            (_, "sv") => "Du sparar tid och forbattrar kvaliteten i saljuppfoljningen.",
            _ => "Vous gagnez du temps et ameliorez la qualite du suivi commercial."
        };
    }

    private static string SectorLabel(string sector, string lang)
    {
        return (sector, lang) switch
        {
            ("Sante", "en") => "Healthcare",
            ("Sante", "sv") => "Halsa",
            ("Education", "sv") => "Utbildning",
            ("Immobilier", "en") => "Real estate",
            ("Immobilier", "sv") => "Fastigheter",
            ("Restauration", "en") => "Restaurants",
            ("Restauration", "sv") => "Restaurang",
            ("Commerce", "sv") => "Handel",
            ("Industrie", "en") => "Industry",
            ("Industrie", "sv") => "Industri",
            ("Construction", "sv") => "Bygg",
            _ => sector
        };
    }

    private static string ObjectiveLabel(string objective, string lang)
    {
        return (objective, lang) switch
        {
            ("Prospection", "en") => "Prospecting",
            ("Relance", "en") => "Follow-up",
            ("Presentation", "en") => "Presentation",
            ("Demo", "en") => "Demo",
            ("Objection", "en") => "Objection handling",
            ("Fidelisation", "en") => "Retention",
            ("Prospection", "sv") => "Prospektering",
            ("Relance", "sv") => "Uppfoljning",
            ("Presentation", "sv") => "Presentation",
            ("Proposition", "sv") => "Forslag",
            ("Objection", "sv") => "Invandningar",
            ("Fidelisation", "sv") => "Kundlojalitet",
            _ => objective
        };
    }

    private static string DefaultCompany(string lang) => lang == "en" ? "your company" : lang == "sv" ? "ert foretag" : "votre entreprise";
    private static string DefaultOffer(string lang) => lang == "en" ? "web, cloud and mobile ERP with AI assistant" : lang == "sv" ? "webb-, moln- och mobil-ERP med AI-assistent" : "ERP web, cloud et mobile avec assistant IA";
    private static string DefaultTarget(string lang) => lang == "en" ? "local SMEs" : lang == "sv" ? "lokala SME-foretag" : "PME locales";
    private static string DefaultContext(string lang) => lang == "en" ? "need to better manage customers, sales, invoices and stock" : lang == "sv" ? "behov av battre kontroll pa kunder, forsaljning, fakturor och lager" : "besoin de mieux gerer les clients, les ventes, les factures et le stock";
    private static string Normalize(string? value, string fallback) => string.IsNullOrWhiteSpace(value) ? fallback : value.Trim();
}

public class SalesAgentRequest
{
    public string Sector { get; set; } = "Commerce";
    public string Objective { get; set; } = "Prospection";
    public string CompanyName { get; set; } = "";
    public string Offer { get; set; } = "";
    public string Target { get; set; } = "";
    public string Context { get; set; } = "";
}

public class SalesAgentResult
{
    public string SectorAnalysis { get; set; } = "";
    public string Email { get; set; } = "";
    public string CallScript { get; set; } = "";
    public string LinkedInMessage { get; set; } = "";
    public string WhatsAppMessage { get; set; } = "";
    public string SalesPage { get; set; } = "";
    public string[] ProspectingActions { get; set; } = Array.Empty<string>();
}
