using CP5_JogosAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CP5_JogosAPI.Data;

/// <summary>
/// Contexto do Entity Framework Core responsável pelo acesso ao banco de dados.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Jogo> Jogos { get; set; } = null!;
    public DbSet<Desenvolvedora> Desenvolvedoras { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Relacionamento 1:N -> uma Desenvolvedora possui vários Jogos.
        modelBuilder.Entity<Jogo>()
            .HasOne(j => j.Desenvolvedora)
            .WithMany(d => d.Jogos)
            .HasForeignKey(j => j.DesenvolvedoraId)
            .OnDelete(DeleteBehavior.Restrict); // impede exclusão de desenvolvedora com jogos vinculados

        // Dados iniciais (seed) para facilitar os testes da API.
        modelBuilder.Entity<Desenvolvedora>().HasData(
            new Desenvolvedora { Id = 1, Nome = "CD Projekt Red", PaisOrigem = "Polônia", AnoFundacao = 1994 },
            new Desenvolvedora { Id = 2, Nome = "FromSoftware", PaisOrigem = "Japão", AnoFundacao = 1986 },
            new Desenvolvedora { Id = 3, Nome = "Nintendo EPD", PaisOrigem = "Japão", AnoFundacao = 1983 }
        );

        modelBuilder.Entity<Jogo>().HasData(
            new Jogo { Id = 1, Titulo = "The Witcher 3: Wild Hunt", Genero = "RPG", Plataforma = "PC", AnoLancamento = 2015, Preco = 79.90m, Nota = 9.8, DesenvolvedoraId = 1 },
            new Jogo { Id = 2, Titulo = "Elden Ring", Genero = "Souls-like", Plataforma = "PS5", AnoLancamento = 2022, Preco = 249.90m, Nota = 9.6, DesenvolvedoraId = 2 },
            new Jogo { Id = 3, Titulo = "The Legend of Zelda: Tears of the Kingdom", Genero = "Aventura", Plataforma = "Switch", AnoLancamento = 2023, Preco = 299.90m, Nota = 9.7, DesenvolvedoraId = 3 }
        );
    }
}
