using System.ComponentModel.DataAnnotations;

namespace CP5_JogosAPI.DTOs;

/// <summary>Dados enviados pelo cliente para criar um jogo.</summary>
public class JogoCreateDto
{
    [Required(ErrorMessage = "O título do jogo é obrigatório.")]
    [StringLength(150, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Genero { get; set; }

    [StringLength(60)]
    public string? Plataforma { get; set; }

    [Range(1958, 2100)]
    public int AnoLancamento { get; set; }

    [Range(0, 100000)]
    public decimal Preco { get; set; }

    [Range(0, 10)]
    public double Nota { get; set; }

    [Required(ErrorMessage = "Informe o Id da desenvolvedora responsável pelo jogo.")]
    public int DesenvolvedoraId { get; set; }
}

/// <summary>Dados enviados pelo cliente para atualizar um jogo existente.</summary>
public class JogoUpdateDto
{
    [Required(ErrorMessage = "O título do jogo é obrigatório.")]
    [StringLength(150, MinimumLength = 1)]
    public string Titulo { get; set; } = string.Empty;

    [StringLength(60)]
    public string? Genero { get; set; }

    [StringLength(60)]
    public string? Plataforma { get; set; }

    [Range(1958, 2100)]
    public int AnoLancamento { get; set; }

    [Range(0, 100000)]
    public decimal Preco { get; set; }

    [Range(0, 10)]
    public double Nota { get; set; }

    [Required(ErrorMessage = "Informe o Id da desenvolvedora responsável pelo jogo.")]
    public int DesenvolvedoraId { get; set; }
}

/// <summary>Dados retornados pela API para representar um jogo.</summary>
public class JogoResponseDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? Genero { get; set; }
    public string? Plataforma { get; set; }
    public int AnoLancamento { get; set; }
    public decimal Preco { get; set; }
    public double Nota { get; set; }
    public int DesenvolvedoraId { get; set; }
    public string? NomeDesenvolvedora { get; set; }
}
