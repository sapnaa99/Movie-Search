using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MovieSearchCase.WebApi.Application.Swagger;

public class SwaggerResponseCodesFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {

        EnsureResponse(operation, StatusCodes.Status503ServiceUnavailable.ToString(), "TMDB external service is currently unavailable");

        var httpMethod = context.ApiDescription.HttpMethod;
        var relativePath = context.ApiDescription.RelativePath ?? string.Empty;


        if (string.Equals(httpMethod, "GET", StringComparison.OrdinalIgnoreCase))
        {

            if (context.ApiDescription.ParameterDescriptions.Any())
            {
                EnsureResponse(operation, StatusCodes.Status400BadRequest.ToString(), "Invalid request parameters or malformed input data");
            }


            if (relativePath.Contains("{id}", StringComparison.OrdinalIgnoreCase))
            {
                EnsureResponse(operation, StatusCodes.Status404NotFound.ToString(), "The requested movie or resource could not be found");
            }
        }
    }

    private static void EnsureResponse(OpenApiOperation operation, string statusCode, string description)
    {
        if (!operation.Responses.ContainsKey(statusCode))
        {
            operation.Responses.Add(statusCode, new OpenApiResponse { Description = description });
        }
    }
}
