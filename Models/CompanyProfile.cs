using System.ComponentModel.DataAnnotations;

namespace EnterpriseERP.Models
{
    public class CompanyProfile
    {
        public int Id { get; set; }

        [Required]
        public string CompanyName { get; set; } = "EnterpriseERP AB";

        public string Slogan { get; set; } = "Votre succès, notre priorité.";

        public string Address { get; set; } = "Stockholm, Suède";

        public string Phone { get; set; } = "+46 70 736 45 55";

        public string Email { get; set; } = "bakarii447@gmail.com";

        public string Website { get; set; } = "www.enterpriseerp.com";

        public string LegalInfo { get; set; } = "";

        public string FooterMessage { get; set; } =
            "Merci pour votre confiance. Nous restons à votre disposition pour vous accompagner.";

        public string LogoPath { get; set; } = "";
    }
}