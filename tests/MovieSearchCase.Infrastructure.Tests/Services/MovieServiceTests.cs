using FluentAssertions;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Clients;
using MovieSearchCase.Infrastructure.Services;
using Xunit;

namespace MovieSearchCase.Infrastructure.Tests.Services;

public class MovieServiceTests
{
    [Fact]
    public async Task GetTrendingAsyncShouldReturnMoviesFromTmdbClient()
    {
        var expectedMovies = new List<Movie>
        {
            new()
            {
                Id = 42,
                Title = "The Answer",
                VoteAverage = 9.2,
            },
        };

        var tmdbClientMock = new Mock<ITmdbClient>();
        tmdbClientMock
            .Setup(client => client.GetTrendingMoviesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMovies);

        var movieService = new MovieService(tmdbClientMock.Object);

        var result = await movieService.GetTrendingAsync(CancellationToken.None);

        result.Should().BeEquivalentTo(expectedMovies);
    }

    [Fact]
    public async Task SearchMoviesAsyncShouldReturnResultsFromTmdbClient()
    {
        var expectedResult = new MovieSearchResult
        {
            Page = 2,
            TotalPages = 5,
            TotalResults = 100,
            Results = new List<Movie>
                        { new()
                            {
                                Id = 42,
                                Title = "The Answer",
                                VoteAverage = 9.2,
                            },
                        },
         };


        var tmdbClientMock = new Mock<ITmdbClient>();

                tmdbClientMock
                    .Setup(client => client.SearchMoviesAsync(
                        "Spider-Man",
                        2,
                        It.IsAny<CancellationToken>()))
                    .ReturnsAsync(expectedResult);

                var movieService = new MovieService(tmdbClientMock.Object);

                var result = await movieService.SearchMoviesAsync(
                    "Spider-Man",
                    2,
                    CancellationToken.None);

        result.Should().BeEquivalentTo(expectedResult);

        tmdbClientMock.Verify(client => client.SearchMoviesAsync(
            "Spider-Man",
            2,
            It.IsAny<CancellationToken>()), Times.Once);


    }


    [Fact]
    public async Task GetMovieDetailsByIdAsyncShouldReturnMovieFromTmdbClient()
    {
        var expectedMovie = new Movie
        {
            Id = 550,
            Title = "Animal",
            VoteAverage = 8.4
        };

        var tmdbClientMock = new Mock<ITmdbClient>();

        tmdbClientMock
            .Setup(client => client.GetMovieDetailsByIdAsync(
                550, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedMovie);

        var movieService = new MovieService(tmdbClientMock.Object);

        var result = await movieService.GetMovieDetailsByIdAsync(
            550, CancellationToken.None);

        result.Should().BeEquivalentTo(expectedMovie);

        tmdbClientMock.Verify(client => client.GetMovieDetailsByIdAsync(
            550, It.IsAny<CancellationToken>()), Times.Once);
    }


    [Fact]
    public async Task GetMovieVideosAsyncShouldReturnVideosFromTmdbClient()
    {
        // Arrange
        IReadOnlyList<MovieVideo> expectedVideos = new List<MovieVideo>
        {
            new()
            {
                Key = "O-b2VfmmbyA",
                Name = "Official Trailer",
                Site = "YouTube",
                Type = "Trailer"
            }
        };

        var tmdbClientMock = new Mock<ITmdbClient>();

        tmdbClientMock
            .Setup(client => client.GetMovieVideosAsync(
                550, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedVideos);

        var movieService = new MovieService(tmdbClientMock.Object);

        // Act
        var result = await movieService.GetMovieVideosAsync(
            550, CancellationToken.None);

        // Assert
        result.Should().BeEquivalentTo(expectedVideos);

        tmdbClientMock.Verify(client => client.GetMovieVideosAsync(
            550, It.IsAny<CancellationToken>()), Times.Once);
    }
}
