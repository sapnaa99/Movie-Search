using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Clients;
using MovieSearchCase.Domain.Interfaces.Services;

namespace MovieSearchCase.Infrastructure.Services;

public class MovieService : IMovieService
{
    private readonly ITmdbClient _tmdbClient;

    public MovieService(ITmdbClient tmdbClient)
    {
        _tmdbClient = tmdbClient;
    }

    public Task<IReadOnlyList<Movie>> GetTrendingAsync(CancellationToken cancellationToken) =>
        _tmdbClient.GetTrendingMoviesAsync(cancellationToken);

    /// <summary>
    /// Search movies 
    /// </summary>
    /// <param name="query"></param>
    /// <param name="page"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<MovieSearchResult> SearchMoviesAsync(string query, int page, CancellationToken cancellationToken = default) =>
         _tmdbClient.SearchMoviesAsync(query, page, cancellationToken);

    /// <summary>
    /// Get movie details by Id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<Movie> GetMovieDetailsByIdAsync(int id, CancellationToken cancellationToken = default) =>
         _tmdbClient.GetMovieDetailsByIdAsync(id, cancellationToken);

    /// <summary>
    /// Get movie Videos
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(int id, CancellationToken cancellationToken = default) => _tmdbClient.GetMovieVideosAsync(id, cancellationToken);
}
