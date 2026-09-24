using CP5_JogosAPI.Data;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// ===== Serviços =====

builder.Services.AddControllers();

// Contexto do EF Core usando SQLite (arquivo local, sem necessidade de servidor externo)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                        ?? "Data Source=jogos.db";
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "API de Jogos - CP5",
        Version = "v1",
        Description = "API RESTful para gerenciamento de um catálogo de jogos e suas desenvolvedoras, " +
                      "desenvolvida com C# .NET 10 e Entity Framework Core."
    });
});

var app = builder.Build();

// ===== Aplica migrations automaticamente ao iniciar (facilita a avaliação/execução local) =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ===== Middleware de tratamento global de erros =====
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        var feature = context.Features.Get<IExceptionHandlerFeature>();
        var problemDetails = new ProblemDetails
        {
            Status = StatusCodes.Status500InternalServerError,
            Title = "Ocorreu um erro inesperado ao processar a requisição.",
            Detail = app.Environment.IsDevelopment() ? feature?.Error.Message : null
        };

        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    });
});

// ===== Pipeline HTTP =====
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API de Jogos v1");
    options.RoutePrefix = string.Empty; // Swagger disponível na raiz: https://localhost:xxxx/
});

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
