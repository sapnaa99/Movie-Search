using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using MovieSearchCase.WebApi.Application.Versioning;
using MovieSearchCase.WebApi.Handlers.Movies;
using Swashbuckle.AspNetCore.Annotations;

namespace MovieSearchCase.WebApi.Controllers.V1;

[ApiVersion("1.0")]
[VersionedRoute]
public class MoviesController : ApiControllerBase
{
    private readonly IMoviesRequestHandlerFactory _requestHandlerFactory;

    public MoviesController(IMoviesRequestHandlerFactory requestHandlerFactory)
    {
        _requestHandlerFactory = requestHandlerFactory;
    }

    /// <summary>
    /// Returns this week's TMDB-backed trending movies.
    /// </summary>
    [HttpGet("trending")]
    [SwaggerOperation(Summary = "Get trending movies", OperationId = "GetTrendingMovies")]
    [SwaggerResponse(StatusCodes.Status200OK, "This week's trending movies", typeof(IEnumerable<Models.Movies.Movie>))]
    [SwaggerResponse(StatusCodes.Status503ServiceUnavailable, "TMDB service is unavailable")]
    public async Task<IActionResult> GetTrending() =>
        await _requestHandlerFactory.GetTrendingMovies().HandleAsync(Request);

    /// <summary>
    /// Searches movies using a query and optional page number.
    /// </summary>
    [HttpGet("search")]
    [SwaggerOperation(Summary = "Search movies", OperationId = "SearchMovies")]
    [SwaggerResponse(StatusCodes.Status200OK, "Paginated movie search results")]
    public async Task<IActionResult> SearchMovies([FromQuery] string query, [FromQuery] int? page)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("Search query parameter cannot be null or empty.");
        }

        return await _requestHandlerFactory.SearchMovies().HandleAsync(Request);
    }

    /// <summary>
    /// Gets movie detail by Id
    /// </summary>
    [HttpGet("{id:int}")]
    [SwaggerOperation(Summary = "Get movie details by ID", OperationId = "GetMovieDetailsById")]
    [SwaggerResponse(StatusCodes.Status200OK, "Movie details retrieved successfully")]
    public async Task<IActionResult> GetMovieDetailsById([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Movie ID must be a positive integer.");
        }

        return await _requestHandlerFactory.GetMovieDetailsById().HandleAsync(Request);
    }

    /// <summary>
    /// Gets movie videos 
    /// </summary>
    [HttpGet("{id:int}/videos")]
    [SwaggerOperation(Summary = "Get movie videos by ID", OperationId = "GetMovieVideos")]
    [SwaggerResponse(StatusCodes.Status200OK, "Movie videos retrieved successfully")]
    public async Task<IActionResult> GetMovieVideos([FromRoute] int id)
    {
        if (id <= 0)
        {
            return BadRequest("Movie ID must be a positive integer.");
        }

        return await _requestHandlerFactory.GetMovieVideos().HandleAsync(Request);
    }
}
