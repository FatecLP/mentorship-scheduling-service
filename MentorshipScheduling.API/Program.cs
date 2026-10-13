using System.Text.Json.Serialization;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using MentorshipScheduling.Application.DTOs;
using MentorshipScheduling.Application.Services;
using MentorshipScheduling.Application.Validators;
using MentorshipScheduling.Infrastructure;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:5173", "https://localhost:5173", "https://menthfy.vercel.app")
            .AllowAnyHeader()
            .AllowAnyMethod();
        });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new JsonStringEnumConverter(allowIntegerValues: false));
    });
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(entry => entry.Value?.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                _ => new[] { "O valor informado é inválido ou está ausente." });
        var problemDetails = new ValidationProblemDetails(errors)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Um ou mais dados da requisição são inválidos.",
            Instance = context.HttpContext.Request.Path
        };

        return new BadRequestObjectResult(problemDetails);
    };
});
builder.Services.AddSwaggerGen();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        if (context.ProblemDetails.Status == StatusCodes.Status500InternalServerError)
        {
            context.ProblemDetails.Title = "Ocorreu um erro inesperado ao processar a solicitação.";
        }
    };
});
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddScoped<ILessonScheduleService, LessonScheduleService>();
builder.Services.AddScoped<IValidator<CreateLessonScheduleRequest>, CreateLessonScheduleRequestValidator>();
builder.Services.AddScoped<IValidator<UpdateLessonScheduleRequest>, UpdateLessonScheduleRequestValidator>();

var app = builder.Build();

await app.Services.ApplyMigrationsAsync();

app.UseExceptionHandler();
app.UseStatusCodePages((StatusCodeContext context) =>
{
    var statusCode = context.HttpContext.Response.StatusCode;
    var title = statusCode switch
    {
        StatusCodes.Status404NotFound => "O recurso solicitado não foi encontrado.",
        StatusCodes.Status405MethodNotAllowed => "O método HTTP não é permitido para este recurso.",
        _ => "A solicitação não pôde ser processada."
    };

    return context.HttpContext.Response.WriteAsJsonAsync(
        new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Instance = context.HttpContext.Request.Path
        },
        options: (JsonSerializerOptions?)null,
        contentType: "application/problem+json",
        cancellationToken: context.HttpContext.RequestAborted);
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(MyAllowSpecificOrigins);

app.MapControllers();

app.Run();

public partial class Program;
