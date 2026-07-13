using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using ramos_kyoto_hr.Domain.Exceptions;

namespace ramos_kyoto_hr.WebApi.Middleware;

/// <summary>
/// Middleware global para capturar exceções e convertê-las em respostas ProblemDetails padronizadas
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "Ocorreu uma exceção: {Message}", exception.Message);

        var problemDetails = exception switch
        {
            EntityNotFoundException notFoundEx => CreateProblemDetails(
                context,
                HttpStatusCode.NotFound,
                "Entidade não encontrada",
                notFoundEx.Message,
                new Dictionary<string, object>
                {
                    { "entityName", notFoundEx.EntityName },
                    { "entityId", notFoundEx.EntityId.ToString() ?? string.Empty }
                }
            ),

            DuplicateEntityException duplicateEx => CreateProblemDetails(
                context,
                HttpStatusCode.Conflict,
                "Entidade duplicada",
                duplicateEx.Message,
                new Dictionary<string, object>
                {
                    { "entityName", duplicateEx.EntityName },
                    { "duplicateField", duplicateEx.DuplicateField },
                    { "duplicateValue", duplicateEx.DuplicateValue.ToString() ?? string.Empty }
                }
            ),

            ValidationException validationEx => CreateProblemDetails(
                context,
                HttpStatusCode.BadRequest,
                "Erro de validação",
                validationEx.Message,
                new Dictionary<string, object>
                {
                    { "errors", validationEx.Errors }
                }
            ),

            InvalidStateTransitionException stateEx => CreateProblemDetails(
                context,
                HttpStatusCode.Conflict,
                "Transição de estado inválida",
                stateEx.Message,
                new Dictionary<string, object>
                {
                    { "entityName", stateEx.EntityName },
                    { "entityId", stateEx.EntityId.ToString() ?? string.Empty },
                    { "currentState", stateEx.CurrentState },
                    { "attemptedState", stateEx.AttemptedState }
                }
            ),

            DomainException domainEx => CreateProblemDetails(
                context,
                HttpStatusCode.BadRequest,
                "Erro de domínio",
                domainEx.Message
            ),

            ArgumentNullException argNullEx => CreateProblemDetails(
                context,
                HttpStatusCode.BadRequest,
                "Argumento nulo",
                $"O parâmetro '{argNullEx.ParamName}' não pode ser nulo."
            ),

            ArgumentException argEx => CreateProblemDetails(
                context,
                HttpStatusCode.BadRequest,
                "Argumento inválido",
                argEx.Message
            ),

            KeyNotFoundException keyNotFoundEx => CreateProblemDetails(
                context,
                HttpStatusCode.NotFound,
                "Recurso não encontrado",
                keyNotFoundEx.Message
            ),

            _ => CreateProblemDetails(
                context,
                HttpStatusCode.InternalServerError,
                "Erro interno do servidor",
                "Ocorreu um erro inesperado. Por favor, tente novamente mais tarde."
            )
        };

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = problemDetails.Status ?? (int)HttpStatusCode.InternalServerError;

        var json = JsonSerializer.Serialize(problemDetails, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }

    private static ProblemDetails CreateProblemDetails(
        HttpContext context,
        HttpStatusCode statusCode,
        string title,
        string detail,
        Dictionary<string, object>? extensions = null)
    {
        var problemDetails = new ProblemDetails
        {
            Status = (int)statusCode,
            Title = title,
            Detail = detail,
            Instance = context.Request.Path,
            Type = $"https://httpstatuses.com/{(int)statusCode}"
        };

        if (extensions != null)
        {
            foreach (var extension in extensions)
            {
                problemDetails.Extensions[extension.Key] = extension.Value;
            }
        }

        return problemDetails;
    }
}


