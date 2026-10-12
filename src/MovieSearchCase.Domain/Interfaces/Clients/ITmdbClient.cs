using MovieSearchCase.Domain.Entities;

namespace MovieSearchCase.Domain.Interfaces.Clients;

/// <summary>
/// Talks to The Movie Database (TMDB) REST API. See https://developer.themoviedb.org/reference.
/// </summary>
public interface ITmdbClient
{
    /// <summary>
    /// Returns this week's trending movies (TMDB's /trending/movie/week endpoint).
    /// </summary>
    Task<IReadOnlyList<Movie>> GetTrendingMoviesAsync(CancellationToken cancellationToken);

    /// <summary>
    ///  Searches movies using a query and optional page number. IClient
    /// </summary>
    /// <param name="query"></param>
    /// <param name="page"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<MovieSearchResult> SearchMoviesAsync(
      string query,
      int page,
      CancellationToken cancellationToken = default);

    /// <summary>
    /// Get movie details by id
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<Movie> GetMovieDetailsByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets movie video
    /// </summary>
    /// <param name="id"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    Task<IReadOnlyList<MovieVideo>> GetMovieVideosAsync(int id, CancellationToken cancellationToken = default);

}
