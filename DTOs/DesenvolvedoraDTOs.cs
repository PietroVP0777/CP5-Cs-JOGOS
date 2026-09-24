using System.ComponentModel.DataAnnotations;

namespace CP5_JogosAPI.DTOs;

/// <summary>Dados enviados pelo cliente para criar uma desenvolvedora.</summary>
public class DesenvolvedoraCreateDto
{
    [Required(ErrorMessage = "O nome da desenvolvedora é obrigatório.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60)]
    public string? PaisOrigem { get; set; }

    [Range(1950, 2100)]
    public int AnoFundacao { get; set; }
}

/// <summary>Dados enviados pelo cliente para atualizar uma desenvolvedora.</summary>
public class DesenvolvedoraUpdateDto
{
    [Required(ErrorMessage = "O nome da desenvolvedora é obrigatório.")]
    [StringLength(100, MinimumLength = 2)]
    public string Nome { get; set; } = string.Empty;

    [StringLength(60)]
    public string? PaisOrigem { get; set; }

    [Range(1950, 2100)]
    public int AnoFundacao { get; set; }
}

/// <summary>Dados retornados pela API para representar uma desenvolvedora.</summary>
public class DesenvolvedoraResponseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? PaisOrigem { get; set; }
    public int AnoFundacao { get; set; }
    public int QuantidadeJogos { get; set; }
}
