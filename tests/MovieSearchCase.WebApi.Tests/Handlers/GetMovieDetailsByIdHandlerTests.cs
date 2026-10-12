using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.WebApi.Handlers.Movies;
using Xunit;

namespace MovieSearchCase.WebApi.Tests.Handlers;

public class GetMovieDetailsByIdHandlerTests
{
    [Fact]
    public async Task HandleShouldReturnMovieWhenIdIsValid()
    {
        // Arrange
        var domainMovie = new Movie
        {
            Id = 550,
            Title = "Animals",
            VoteAverage = 8.4
        };

        var serviceMock = new Mock<IMovieService>();
        serviceMock
            .Setup(service => service.GetMovieDetailsByIdAsync(550, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainMovie);

        var handler = new GetMovieDetailsByIdHandler(serviceMock.Object);

        var context = new DefaultHttpContext();
        context.Request.RouteValues["id"] = "550";

        // Act
        var result = await handler.HandleAsync(context.Request);

        // Assert
        var okResult = result.Should()
            .BeOfType<OkObjectResult>()
            .Subject;
      
        var apiMovie = okResult.Value.Should().BeOfType<Models.Movies.Movie>().Subject;
        apiMovie.Id.Should().Be(550);
        apiMovie.Title.Should().Be("Animals");
        apiMovie.VoteAverage.Should().Be(8.4);

        serviceMock.Verify(service => service.GetMovieDetailsByIdAsync(
            550, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Theory]
    [InlineData("0")]
    [InlineData("-1")]
    [InlineData("abc")]
    [InlineData(null)] 
    public async Task HandleShouldReturnBadRequestWhenIdIsInvalid(string? id)
    {
        // Arrange
        var serviceMock = new Mock<IMovieService>();
        var handler = new GetMovieDetailsByIdHandler(serviceMock.Object);

        var context = new DefaultHttpContext();
        if (id != null)
        {
            context.Request.RouteValues["id"] = id;
        }

        // Act
        var result = await handler.HandleAsync(context.Request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        serviceMock.Verify(service => service.GetMovieDetailsByIdAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
