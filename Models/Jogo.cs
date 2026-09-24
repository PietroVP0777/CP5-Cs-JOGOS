using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CP5_JogosAPI.Models;

/// <summary>
/// Representa um jogo do catálogo.
/// </summary>
public class Jogo
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O título do jogo é obrigatório.")]
    [StringLength(150, MinimumLength = 1, ErrorMessage = "O título deve ter entre 1 e 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Genero { get; set; }

    [StringLength(60)]
    public string? Plataforma { get; set; }

    [Range(1958, 2100, ErrorMessage = "Ano de lançamento inválido.")]
    public int AnoLancamento { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Range(0, 100000, ErrorMessage = "O preço deve ser um valor positivo.")]
    public decimal Preco { get; set; }

    [Range(0, 10, ErrorMessage = "A nota deve estar entre 0 e 10.")]
    public double Nota { get; set; }

    // Chave estrangeira para a desenvolvedora responsável pelo jogo.
    [Required]
    public int DesenvolvedoraId { get; set; }

    public Desenvolvedora? Desenvolvedora { get; set; }
}
