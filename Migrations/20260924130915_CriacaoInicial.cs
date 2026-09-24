using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CP5_JogosAPI.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Desenvolvedoras",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PaisOrigem = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    AnoFundacao = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Desenvolvedoras", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Jogos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Titulo = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                    Genero = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    Plataforma = table.Column<string>(type: "TEXT", maxLength: 60, nullable: true),
                    AnoLancamento = table.Column<int>(type: "INTEGER", nullable: false),
                    Preco = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Nota = table.Column<double>(type: "REAL", nullable: false),
                    DesenvolvedoraId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Jogos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Jogos_Desenvolvedoras_DesenvolvedoraId",
                        column: x => x.DesenvolvedoraId,
                        principalTable: "Desenvolvedoras",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Desenvolvedoras",
                columns: new[] { "Id", "AnoFundacao", "Nome", "PaisOrigem" },
                values: new object[,]
                {
                    { 1, 1994, "CD Projekt Red", "Polônia" },
                    { 2, 1986, "FromSoftware", "Japão" },
                    { 3, 1983, "Nintendo EPD", "Japão" }
                });

            migrationBuilder.InsertData(
                table: "Jogos",
                columns: new[] { "Id", "AnoLancamento", "DesenvolvedoraId", "Genero", "Nota", "Plataforma", "Preco", "Titulo" },
                values: new object[,]
                {
                    { 1, 2015, 1, "RPG", 9.8000000000000007, "PC", 79.90m, "The Witcher 3: Wild Hunt" },
                    { 2, 2022, 2, "Souls-like", 9.5999999999999996, "PS5", 249.90m, "Elden Ring" },
                    { 3, 2023, 3, "Aventura", 9.6999999999999993, "Switch", 299.90m, "The Legend of Zelda: Tears of the Kingdom" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jogos_DesenvolvedoraId",
                table: "Jogos",
                column: "DesenvolvedoraId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Jogos");

            migrationBuilder.DropTable(
                name: "Desenvolvedoras");
        }
    }
}
