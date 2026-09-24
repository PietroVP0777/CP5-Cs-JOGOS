using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CP5_JogosAPI.Models;

/// <summary>
/// Representa o estúdio/empresa responsável por desenvolver um ou mais jogos.
/// </summary>
public class Desenvolvedora
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "O nome da desenvolvedora é obrigatório.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "O nome deve ter entre 2 e 100 caracteres.")]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60, ErrorMessage = "O país de origem deve ter no máximo 60 caracteres.")]
    public string? PaisOrigem { get; set; }

    [Range(1950, 2100, ErrorMessage = "Ano de fundação inválido.")]
    public int AnoFundacao { get; set; }

    // Navegação: uma desenvolvedora possui vários jogos.
    // Ignorado na serialização padrão para evitar referência circular nas respostas de Jogo.
    [JsonIgnore]
    public ICollection<Jogo> Jogos { get; set; } = new List<Jogo>();
}
