using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MovieSearchCase.WebApi.Application.Cors;
using Microsoft.AspNetCore.Diagnostics;
using MovieSearchCase.Domain.Exceptions;

namespace MovieSearchCase.WebApi.Application.ProblemDetails;

public static class ProblemDetailsExtensions
{
    public static IServiceCollection AddCustomProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                var exceptionFeature = context.HttpContext.Features.Get<IExceptionHandlerPathFeature>();
             
                if (exceptionFeature?.Error is ProblemDetailsException pdEx)
                {
                    context.ProblemDetails.Status = pdEx.ErrorCode.ToStatusCode();
                    context.ProblemDetails.Title = pdEx.Title;
                    context.ProblemDetails.Detail = pdEx.Details;
                    context.ProblemDetails.Instance = pdEx.Instance?.ToString();
                }
            };
        });

        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();

                var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                    context.HttpContext,
                    context.ModelState);

                return new BadRequestObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" },
                };
            };
        });

        return services;
    }
}
