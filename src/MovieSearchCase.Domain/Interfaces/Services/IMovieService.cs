using MovieSearchCase.Domain.Entities;

namespace MovieSearchCase.Domain.Interfaces.Services;

/// <summary>
/// TMDB-backed movie lookups used by the API.
/// </summary>
public interface IMovieService
{
    Task<IReadOnlyList<Movie>> GetTrendingAsync(CancellationToken cancellationToken);
  
    Task<MovieSearchResult> SearchMoviesAsync(
      string query,
      int page,
      CancellationToken cancellationToken = default);

    Task<Movie> GetMovieDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(int id, CancellationToken cancellationToken = default);

}
