using System.Net;
using System.Text;
using FluentAssertions;
using MovieSearchCase.Domain.Exceptions;
using MovieSearchCase.Infrastructure.Clients;
using Xunit;

namespace MovieSearchCase.Infrastructure.Tests.Clients;

public class TmdbClientTests
{
    [Fact]
    public async Task GetTrendingMoviesAsyncShouldSendCorrectRequestAndMapResponse()
    {
        // Arrange
        var responseJson = """
        {
            "page": 1,
            "total_pages": 1,
            "total_results": 1,
            "results": [
                {
                    "id": 100,
                    "title": "Trending Movie",
                    "vote_average": 7.8
                }
            ]
        }
        """;

        var handler = new FakeHttpMessageHandler(request =>
        {
            request.RequestUri!.PathAndQuery.Should().Contain("trending/movie/week");
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var tmdbClient = new TmdbClient(httpClient);

        // Act
        var result = await tmdbClient.GetTrendingMoviesAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
        result[0].Id.Should().Be(100);
        result[0].Title.Should().Be("Trending Movie");
        result[0].VoteAverage.Should().Be(7.8);
    }

    [Fact]
    public async Task SearchMoviesAsyncShouldSendCorrectRequestAndMapResponse()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(request =>
        {
            request.RequestUri!.PathAndQuery.Should().Contain("/search/movie");
            request.RequestUri.Query.Should().Contain("query=Spider-Man");
            request.RequestUri.Query.Should().Contain("page=2");

            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    """
                    {
                        "page": 2,
                        "total_pages": 5,
                        "total_results": 1,
                        "results": [
                            {
                                "id": 42,
                                "title": "The Answer",
                                "vote_average": 9.2
                            }
                        ]
                    }
                    """,
                    Encoding.UTF8,
                    "application/json")
            };
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var tmdbClient = new TmdbClient(httpClient);

        // Act
        var result = await tmdbClient.SearchMoviesAsync("Spider-Man", 2, CancellationToken.None);

        // Assert
        result.Page.Should().Be(2);
        result.TotalPages.Should().Be(5);
        result.TotalResults.Should().Be(1);
        result.Results.Should().HaveCount(1);
        result.Results[0].Id.Should().Be(42);
        result.Results[0].Title.Should().Be("The Answer");
        result.Results[0].VoteAverage.Should().Be(9.2);
    }

    [Fact]
    public async Task SearchMoviesAsyncShouldThrowMovieExceptionWhenRequestFails()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var tmdbClient = new TmdbClient(httpClient);

        // Act
        var act = async () => await tmdbClient.SearchMoviesAsync("Batman", 1, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<MovieException>();
        exception.Which.ErrorCode.Should().Be(ErrorType.UpstreamServiceUnavailable);
    }

    [Fact]
    public async Task SearchMoviesAsyncShouldThrowMovieExceptionWhenResponseIsNull()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null", Encoding.UTF8, "application/json")
            });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var tmdbClient = new TmdbClient(httpClient);

        // Act
        var act = async () => await tmdbClient.SearchMoviesAsync("Batman", 1, CancellationToken.None);

        // Assert
        var exception = await act.Should().ThrowAsync<MovieException>();
        exception.Which.ErrorCode.Should().Be(ErrorType.UpstreamServiceUnavailable);
    }

    [Fact]
    public async Task GetMovieDetailsByIdAsyncShouldReturnMovieWhenResponseIsSuccessful()
    {
        // Arrange
        var responseJson = """
        {
            "id": 550,
            "title": "Fight Club",
            "vote_average": 8.4
        }
        """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var client = new TmdbClient(httpClient);

        // Act
        var result = await client.GetMovieDetailsByIdAsync(550);

        // Assert
        result.Id.Should().Be(550);
        result.Title.Should().Be("Fight Club");
        result.VoteAverage.Should().Be(8.4);
    }

    [Fact]
    public async Task GetMovieDetailsByIdAsyncShouldThrowMovieExceptionWhenTmdbReturnsNotFound()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.NotFound));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var client = new TmdbClient(httpClient);

        // Act
        var act = async () => await client.GetMovieDetailsByIdAsync(999999);

        // Assert
        var exception = await act.Should().ThrowAsync<MovieException>();
        exception.Which.ErrorCode.Should().Be(ErrorType.EntityNotFound); 
    }

    [Fact]
    public async Task GetMovieDetailsByIdAsyncShouldThrowMovieExceptionWhenTmdbReturnsServerError()
    {
        // Arrange
        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.ServiceUnavailable));
        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var client = new TmdbClient(httpClient);

        // Act
        var act = async () => await client.GetMovieDetailsByIdAsync(550);

        // Assert
        var exception = await act.Should().ThrowAsync<MovieException>();
        exception.Which.ErrorCode.Should().Be(ErrorType.UpstreamServiceUnavailable);
    }

    [Fact]
    public async Task GetMovieVideosAsyncShouldReturnYouTubeVideos()
    {
        // Arrange
        var responseJson = """
        {
            "id": 550,
            "results": [
                {
                    "key": "O-b2VfmmbyA",
                    "name": "Official Trailer",
                    "site": "YouTube",
                    "type": "Trailer"
                },
                {
                    "key": "abc123",
                    "name": "Other Video",
                    "site": "Vimeo",
                    "type": "Trailer"
                }
            ]
        }
        """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var client = new TmdbClient(httpClient);

        // Act
        var result = await client.GetMovieVideosAsync(550);

        // Assert
        result.Should().HaveCount(1);
        result[0].Key.Should().Be("O-b2VfmmbyA");
        result[0].Name.Should().Be("Official Trailer");
        result[0].Site.Should().Be("YouTube");
        result[0].Type.Should().Be("Trailer");
    }

    [Fact]
    public async Task GetMovieVideosAsyncShouldReturnEmptyListWhenNoVideosExist()
    {
        // Arrange
        var responseJson = """
        {
            "id": 550,
            "results": []
        }
        """;

        var handler = new FakeHttpMessageHandler(_ => new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(responseJson, Encoding.UTF8, "application/json")
        });

        using var httpClient = new HttpClient(handler) { BaseAddress = new Uri("https://api.themoviedb.org/3/") };
        var client = new TmdbClient(httpClient);

        // Act
        var result = await client.GetMovieVideosAsync(550);

        // Assert
        result.Should().BeEmpty(); 
    }

    private sealed class FakeHttpMessageHandler(Func<HttpRequestMessage, HttpResponseMessage> responseFactory)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Task.FromResult(responseFactory(request));
        }
    }
}
