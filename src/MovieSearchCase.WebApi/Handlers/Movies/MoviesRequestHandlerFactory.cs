using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.Shared;

namespace MovieSearchCase.WebApi.Handlers.Movies;

public class MoviesRequestHandlerFactory : IMoviesRequestHandlerFactory
{
    private readonly IMovieService _movieService;

    public MoviesRequestHandlerFactory(IMovieService movieService)
    {
        _movieService = movieService;
    }

    public IRequestHandlerAsync GetTrendingMovies() => new GetTrendingMoviesHandler(_movieService);

    public IRequestHandlerAsync SearchMovies() => new SearchMoviesHandler(_movieService);

    public IRequestHandlerAsync GetMovieDetailsById() => new GetMovieDetailsByIdHandler(_movieService);

    public IRequestHandlerAsync GetMovieVideos() => new GetMovieVideosHandler(_movieService);

}
