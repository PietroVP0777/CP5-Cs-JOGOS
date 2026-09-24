using CP5_JogosAPI.Data;
using CP5_JogosAPI.DTOs;
using CP5_JogosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP5_JogosAPI.Controllers;

/// <summary>
/// Endpoints para gerenciamento das desenvolvedoras (estúdios) de jogos.
/// Versão 1 da API: api/v1/desenvolvedoras
/// </summary>
[ApiController]
[Route("api/v1/desenvolvedoras")]
[Produces("application/json")]
public class DesenvolvedorasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<DesenvolvedorasController> _logger;

    public DesenvolvedorasController(AppDbContext context, ILogger<DesenvolvedorasController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Lista todas as desenvolvedoras cadastradas.</summary>
    /// <response code="200">Lista retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DesenvolvedoraResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<DesenvolvedoraResponseDto>>> ListarDesenvolvedoras()
    {
        var desenvolvedoras = await _context.Desenvolvedoras
            .Include(d => d.Jogos)
            .OrderBy(d => d.Id)
            .Select(d => ParaDto(d))
            .ToListAsync();

        return Ok(desenvolvedoras);
    }

    /// <summary>Busca uma desenvolvedora específica pelo Id.</summary>
    /// <response code="200">Desenvolvedora encontrada.</response>
    /// <response code="404">Desenvolvedora não encontrada.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(DesenvolvedoraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DesenvolvedoraResponseDto>> ObterDesenvolvedora(int id)
    {
        var desenvolvedora = await _context.Desenvolvedoras
            .Include(d => d.Jogos)
            .FirstOrDefaultAsync(d => d.Id == id);

        if (desenvolvedora == null)
            return NotFound(new { mensagem = $"Desenvolvedora com Id {id} não foi encontrada." });

        return Ok(ParaDto(desenvolvedora));
    }

    /// <summary>Lista os jogos pertencentes a uma desenvolvedora.</summary>
    /// <response code="200">Lista de jogos retornada com sucesso.</response>
    /// <response code="404">Desenvolvedora não encontrada.</response>
    [HttpGet("{id:int}/jogos")]
    [ProducesResponseType(typeof(IEnumerable<JogoResponseDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<JogoResponseDto>>> ListarJogosDaDesenvolvedora(int id)
    {
        var existe = await _context.Desenvolvedoras.AnyAsync(d => d.Id == id);
        if (!existe)
            return NotFound(new { mensagem = $"Desenvolvedora com Id {id} não foi encontrada." });

        var jogos = await _context.Jogos
            .Where(j => j.DesenvolvedoraId == id)
            .Include(j => j.Desenvolvedora)
            .OrderBy(j => j.Id)
            .Select(j => new JogoResponseDto
            {
                Id = j.Id,
                Titulo = j.Titulo,
                Genero = j.Genero,
                Plataforma = j.Plataforma,
                AnoLancamento = j.AnoLancamento,
                Preco = j.Preco,
                Nota = j.Nota,
                DesenvolvedoraId = j.DesenvolvedoraId,
                NomeDesenvolvedora = j.Desenvolvedora!.Nome
            })
            .ToListAsync();

        return Ok(jogos);
    }

    /// <summary>Cadastra uma nova desenvolvedora.</summary>
    /// <response code="201">Desenvolvedora criada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    [HttpPost]
    [ProducesResponseType(typeof(DesenvolvedoraResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<DesenvolvedoraResponseDto>> CriarDesenvolvedora([FromBody] DesenvolvedoraCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var desenvolvedora = new Desenvolvedora
        {
            Nome = dto.Nome,
            PaisOrigem = dto.PaisOrigem,
            AnoFundacao = dto.AnoFundacao
        };

        _context.Desenvolvedoras.Add(desenvolvedora);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Desenvolvedora {Nome} (Id {Id}) criada com sucesso.", desenvolvedora.Nome, desenvolvedora.Id);

        return CreatedAtAction(nameof(ObterDesenvolvedora), new { id = desenvolvedora.Id }, ParaDto(desenvolvedora));
    }

    /// <summary>Atualiza uma desenvolvedora existente.</summary>
    /// <response code="200">Desenvolvedora atualizada com sucesso.</response>
    /// <response code="400">Dados inválidos.</response>
    /// <response code="404">Desenvolvedora não encontrada.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(DesenvolvedoraResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DesenvolvedoraResponseDto>> AtualizarDesenvolvedora(int id, [FromBody] DesenvolvedoraUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var desenvolvedora = await _context.Desenvolvedoras.Include(d => d.Jogos).FirstOrDefaultAsync(d => d.Id == id);
        if (desenvolvedora == null)
            return NotFound(new { mensagem = $"Desenvolvedora com Id {id} não foi encontrada." });

        desenvolvedora.Nome = dto.Nome;
        desenvolvedora.PaisOrigem = dto.PaisOrigem;
        desenvolvedora.AnoFundacao = dto.AnoFundacao;

        await _context.SaveChangesAsync();

        return Ok(ParaDto(desenvolvedora));
    }

    /// <summary>Remove uma desenvolvedora, desde que não possua jogos vinculados.</summary>
    /// <response code="204">Desenvolvedora removida com sucesso.</response>
    /// <response code="400">Desenvolvedora possui jogos vinculados e não pode ser removida.</response>
    /// <response code="404">Desenvolvedora não encontrada.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirDesenvolvedora(int id)
    {
        var desenvolvedora = await _context.Desenvolvedoras.Include(d => d.Jogos).FirstOrDefaultAsync(d => d.Id == id);
        if (desenvolvedora == null)
            return NotFound(new { mensagem = $"Desenvolvedora com Id {id} não foi encontrada." });

        if (desenvolvedora.Jogos.Any())
            return BadRequest(new { mensagem = "Não é possível excluir uma desenvolvedora que possui jogos cadastrados. Remova os jogos primeiro." });

        _context.Desenvolvedoras.Remove(desenvolvedora);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Desenvolvedora Id {Id} removida com sucesso.", id);

        return NoContent();
    }

    private static DesenvolvedoraResponseDto ParaDto(Desenvolvedora desenvolvedora) => new()
    {
        Id = desenvolvedora.Id,
        Nome = desenvolvedora.Nome,
        PaisOrigem = desenvolvedora.PaisOrigem,
        AnoFundacao = desenvolvedora.AnoFundacao,
        QuantidadeJogos = desenvolvedora.Jogos.Count
    };
}
