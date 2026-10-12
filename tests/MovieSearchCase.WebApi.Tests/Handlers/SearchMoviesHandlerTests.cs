using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.WebApi.Handlers.Movies;
using Xunit;
using ApiMovie = MovieSearchCase.WebApi.Models.Movies.Movie;

namespace MovieSearchCase.WebApi.Tests.Handlers;

public class SearchMoviesHandlerTests
{
    [Fact]
    public async Task HandleShouldReturnOkWithMappedMoviesAndPaginationWhenServiceSucceeds()
    {
        // Arrange
        var searchResult = new MovieSearchResult
        {
            Page = 2,
            TotalPages = 5,
            TotalResults = 100,
            Results = new List<Movie>
            {
                new()
                {
                    Id = 42,
                    Title = "Batman",
                    VoteAverage = 8.5,
                },
            },
        };
        var movieServiceMock = new Mock<IMovieService>();

        movieServiceMock
            .Setup(service => service.SearchMoviesAsync("Batman", 2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResult);

        var handler = new SearchMoviesHandler(movieServiceMock.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString("?query=Batman&page=2");

        // Act
        var result = await handler.HandleAsync(httpContext.Request);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;

      
        okResult.Value.Should().BeEquivalentTo(new
        {
            page = 2,
            totalPages = 5,
            totalResults = 100,
            results = new List<object>
            {
                new { Id = 42, Title = "Batman", VoteAverage = 8.5 }
            }
        });

        movieServiceMock.Verify(service => service.SearchMoviesAsync(
            "Batman",
            2,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task HandleShouldReturnBadRequestWhenQueryIsMissingOrBlank(string query)
    {
        // Arrange
        var movieServiceMock = new Mock<IMovieService>();
        var handler = new SearchMoviesHandler(movieServiceMock.Object);
        var httpContext = new DefaultHttpContext();

        httpContext.Request.QueryString = new QueryString($"?query={Uri.EscapeDataString(query)}");

        // Act
        var result = await handler.HandleAsync(httpContext.Request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        movieServiceMock.Verify(service => service.SearchMoviesAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("0")]
    [InlineData("501")]
    [InlineData("-5")] 
    public async Task HandleShouldReturnBadRequestWhenPageIsInvalid(string page)
    {
        // Arrange
        var movieServiceMock = new Mock<IMovieService>();
        var handler = new SearchMoviesHandler(movieServiceMock.Object);
        var httpContext = new DefaultHttpContext();

        httpContext.Request.QueryString = new QueryString($"?query=Spider-Man&page={page}");

        // Act
        var result = await handler.HandleAsync(httpContext.Request);

        // Assert
        var badRequest = result.Should().BeOfType<BadRequestObjectResult>().Subject;
        badRequest.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        movieServiceMock.Verify(service => service.SearchMoviesAsync(
            It.IsAny<string>(),
            It.IsAny<int>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task HandleShouldDefaultToFirstPageWhenPageIsOmitted()
    {
        // Arrange
        var searchResult = new MovieSearchResult
        {
            Page = 1,
            TotalPages = 1,
            TotalResults = 0,
            Results = Array.Empty<Movie>(),
        };

        var movieServiceMock = new Mock<IMovieService>();

        movieServiceMock
            .Setup(service => service.SearchMoviesAsync("Spider-Man", 1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(searchResult);

        var handler = new SearchMoviesHandler(movieServiceMock.Object);
        var httpContext = new DefaultHttpContext();
        httpContext.Request.QueryString = new QueryString("?query=Spider-Man");

        // Act
        var result = await handler.HandleAsync(httpContext.Request);

        // Assert
        result.Should().BeOfType<OkObjectResult>();

        movieServiceMock.Verify(service => service.SearchMoviesAsync(
            "Spider-Man",
            1,
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
