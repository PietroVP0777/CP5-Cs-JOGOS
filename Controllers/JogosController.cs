using CP5_JogosAPI.Data;
using CP5_JogosAPI.DTOs;
using CP5_JogosAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CP5_JogosAPI.Controllers;

/// <summary>
/// Endpoints para gerenciamento do catálogo de jogos.
/// Versão 1 da API: api/v1/jogos
/// </summary>
[ApiController]
[Route("api/v1/jogos")]
[Produces("application/json")]
public class JogosController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<JogosController> _logger;

    public JogosController(AppDbContext context, ILogger<JogosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>Lista todos os jogos, com filtros opcionais por gênero e plataforma.</summary>
    /// <response code="200">Lista de jogos retornada com sucesso.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<JogoResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<JogoResponseDto>>> ListarJogos(
        [FromQuery] string? genero,
        [FromQuery] string? plataforma)
    {
        var query = _context.Jogos.Include(j => j.Desenvolvedora).AsQueryable();

        if (!string.IsNullOrWhiteSpace(genero))
            query = query.Where(j => j.Genero != null && j.Genero.ToLower() == genero.ToLower());

        if (!string.IsNullOrWhiteSpace(plataforma))
            query = query.Where(j => j.Plataforma != null && j.Plataforma.ToLower() == plataforma.ToLower());

        var jogos = await query
            .OrderBy(j => j.Id)
            .Select(j => ParaDto(j))
            .ToListAsync();

        return Ok(jogos);
    }

    /// <summary>Busca um jogo específico pelo Id.</summary>
    /// <response code="200">Jogo encontrado.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(JogoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogoResponseDto>> ObterJogo(int id)
    {
        var jogo = await _context.Jogos.Include(j => j.Desenvolvedora).FirstOrDefaultAsync(j => j.Id == id);

        if (jogo == null)
            return NotFound(new { mensagem = $"Jogo com Id {id} não foi encontrado." });

        return Ok(ParaDto(jogo));
    }

    /// <summary>Cadastra um novo jogo.</summary>
    /// <response code="201">Jogo criado com sucesso.</response>
    /// <response code="400">Dados inválidos ou desenvolvedora inexistente.</response>
    [HttpPost]
    [ProducesResponseType(typeof(JogoResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JogoResponseDto>> CriarJogo([FromBody] JogoCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var desenvolvedoraExiste = await _context.Desenvolvedoras.AnyAsync(d => d.Id == dto.DesenvolvedoraId);
        if (!desenvolvedoraExiste)
            return BadRequest(new { mensagem = $"Desenvolvedora com Id {dto.DesenvolvedoraId} não existe." });

        var jogo = new Jogo
        {
            Titulo = dto.Titulo,
            Genero = dto.Genero,
            Plataforma = dto.Plataforma,
            AnoLancamento = dto.AnoLancamento,
            Preco = dto.Preco,
            Nota = dto.Nota,
            DesenvolvedoraId = dto.DesenvolvedoraId
        };

        _context.Jogos.Add(jogo);
        await _context.SaveChangesAsync();

        await _context.Entry(jogo).Reference(j => j.Desenvolvedora).LoadAsync();

        _logger.LogInformation("Jogo {Titulo} (Id {Id}) criado com sucesso.", jogo.Titulo, jogo.Id);

        return CreatedAtAction(nameof(ObterJogo), new { id = jogo.Id }, ParaDto(jogo));
    }

    /// <summary>Atualiza um jogo existente.</summary>
    /// <response code="200">Jogo atualizado com sucesso.</response>
    /// <response code="400">Dados inválidos ou desenvolvedora inexistente.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(JogoResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JogoResponseDto>> AtualizarJogo(int id, [FromBody] JogoUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);
        if (jogo == null)
            return NotFound(new { mensagem = $"Jogo com Id {id} não foi encontrado." });

        var desenvolvedoraExiste = await _context.Desenvolvedoras.AnyAsync(d => d.Id == dto.DesenvolvedoraId);
        if (!desenvolvedoraExiste)
            return BadRequest(new { mensagem = $"Desenvolvedora com Id {dto.DesenvolvedoraId} não existe." });

        jogo.Titulo = dto.Titulo;
        jogo.Genero = dto.Genero;
        jogo.Plataforma = dto.Plataforma;
        jogo.AnoLancamento = dto.AnoLancamento;
        jogo.Preco = dto.Preco;
        jogo.Nota = dto.Nota;
        jogo.DesenvolvedoraId = dto.DesenvolvedoraId;

        await _context.SaveChangesAsync();
        await _context.Entry(jogo).Reference(j => j.Desenvolvedora).LoadAsync();

        return Ok(ParaDto(jogo));
    }

    /// <summary>Remove um jogo do catálogo.</summary>
    /// <response code="204">Jogo removido com sucesso.</response>
    /// <response code="404">Jogo não encontrado.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExcluirJogo(int id)
    {
        var jogo = await _context.Jogos.FirstOrDefaultAsync(j => j.Id == id);
        if (jogo == null)
            return NotFound(new { mensagem = $"Jogo com Id {id} não foi encontrado." });

        _context.Jogos.Remove(jogo);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Jogo Id {Id} removido com sucesso.", id);

        return NoContent();
    }

    private static JogoResponseDto ParaDto(Jogo jogo) => new()
    {
        Id = jogo.Id,
        Titulo = jogo.Titulo,
        Genero = jogo.Genero,
        Plataforma = jogo.Plataforma,
        AnoLancamento = jogo.AnoLancamento,
        Preco = jogo.Preco,
        Nota = jogo.Nota,
        DesenvolvedoraId = jogo.DesenvolvedoraId,
        NomeDesenvolvedora = jogo.Desenvolvedora?.Nome
    };
}
