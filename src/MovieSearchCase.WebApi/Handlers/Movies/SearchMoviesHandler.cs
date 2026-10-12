
using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;
using MovieSearchCase.WebApi.Mappers;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class SearchMoviesHandler : IRequestHandlerAsync
{
    private readonly IMovieService _movieService;

    public SearchMoviesHandler(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public async Task<IActionResult> HandleAsync(HttpRequest request)
    {
        var query = request.Query["query"].ToString().Trim();

        if (string.IsNullOrWhiteSpace(query))
        {
            return new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid query",
                Detail = "The query parameter is required and cannot be empty."
            });
        }

        var pageValue = request.Query["page"].ToString();
        var page = 1;

        if (!string.IsNullOrEmpty(pageValue) &&
            (!int.TryParse(pageValue, out page) || page < 1 || page > 500))
        {
            return new BadRequestObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Invalid page",
                Detail = "The page parameter must be an integer between 1 and 500."
            });
        }

        var searchResult = await _movieService.SearchMoviesAsync(
            query,
            page,
            request.HttpContext.RequestAborted);

        return new OkObjectResult(new
        {
            page = searchResult.Page,
            totalPages = searchResult.TotalPages,
            totalResults = searchResult.TotalResults,
            results = searchResult.Results.Select(movie => movie.ToApiModel())
        });
    }
}
