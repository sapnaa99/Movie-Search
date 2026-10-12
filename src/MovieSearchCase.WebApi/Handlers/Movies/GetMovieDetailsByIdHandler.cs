
using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.Domain.Exceptions;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;
using MovieSearchCase.WebApi.Mappers;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class GetMovieDetailsByIdHandler : IRequestHandlerAsync
{
    private readonly IMovieService _movieService;

    public GetMovieDetailsByIdHandler(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> HandleAsync(HttpRequest request)
    {
        if (!int.TryParse(request.RouteValues["id"]?.ToString(), out var id) || id <= 0)
        {
            return new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid movie ID",
                Detail = "The movie ID must be a positive integer."
            });
        }

        var movie = await _movieService.GetMovieDetailsByIdAsync(
            id,
            request.HttpContext.RequestAborted);

        return new OkObjectResult(movie.ToApiModel());
    }
}
