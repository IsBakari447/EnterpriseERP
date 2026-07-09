using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EnterpriseERP.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CompanyName = table.Column<string>(type: "TEXT", nullable: false),
                    Slogan = table.Column<string>(type: "TEXT", nullable: false),
                    Address = table.Column<string>(type: "TEXT", nullable: false),
                    Phone = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: false),
                    Website = table.Column<string>(type: "TEXT", nullable: false),
                    LegalInfo = table.Column<string>(type: "TEXT", nullable: false),
                    FooterMessage = table.Column<string>(type: "TEXT", nullable: false),
                    LogoPath = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "CompanyProfiles",
                columns: new[] { "Id", "Address", "CompanyName", "Email", "FooterMessage", "LegalInfo", "LogoPath", "Phone", "Slogan", "Website" },
                values: new object[] { 1, "Stockholm, Suède", "EnterpriseERP AB", "bakarii447@gmail.com", "Merci pour votre confiance. Nous restons à votre disposition pour vous accompagner.", "Document généré automatiquement par EnterpriseERP.", "", "+46 70 736 45 55", "Votre succès, notre priorité.", "www.enterpriseerp.com" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfiles");
        }
    }
}
