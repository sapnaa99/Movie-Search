
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;
using MovieSearchCase.WebApi.Mappers;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class GetMovieVideosHandler : IRequestHandlerAsync
{
    private readonly IMovieService _movieService;

    public GetMovieVideosHandler(IMovieService movieService)
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
                Title = "Invalid request",
                Detail = "The movie ID must be a positive integer."
            });
        }

        var videos = await _movieService.GetMovieVideosAsync(
            id,
            request.HttpContext.RequestAborted);

        var result = videos.Select(video => video.ToApiModel());

        return new OkObjectResult(result);
    }
}
