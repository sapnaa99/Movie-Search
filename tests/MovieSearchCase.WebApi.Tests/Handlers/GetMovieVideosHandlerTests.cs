using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using MovieSearchCase.Domain.Entities;
using MovieSearchCase.Domain.Interfaces.Services;
using MovieSearchCase.WebApi.Handlers.Movies;
using Xunit;

namespace MovieSearchCase.WebApi.Tests.Handlers;

public class GetMovieVideosHandlerTests
{
    [Fact]
    public async Task HandleShouldReturnVideosWhenIdIsValid()
    {
        // Arrange
        IReadOnlyList<MovieVideo> domainVideos = new List<MovieVideo>
        {
            new()
            {
                Key = "O-b2VfmmbyA",
                Name = "Official Trailer",
                Site = "YouTube",
                Type = "Trailer"
            }
        };

        var serviceMock = new Mock<IMovieService>();

        serviceMock
            .Setup(service => service.GetMovieVideosAsync(550, It.IsAny<CancellationToken>()))
            .ReturnsAsync(domainVideos);

        var handler = new GetMovieVideosHandler(serviceMock.Object);
        var context = new DefaultHttpContext();
        context.Request.RouteValues["id"] = "550";

        // Act
        var result = await handler.HandleAsync(context.Request);

        // Assert
        var okResult = result.Should()
            .BeOfType<OkObjectResult>().Subject;

       
        var apiVideos = okResult.Value.Should().BeAssignableTo<IEnumerable<Models.Movies.MovieVideo>>().Subject.ToList();
        apiVideos.Should().HaveCount(1);
        apiVideos[0].Key.Should().Be("O-b2VfmmbyA");
        apiVideos[0].Name.Should().Be("Official Trailer");
        apiVideos[0].Site.Should().Be("YouTube");
        apiVideos[0].Type.Should().Be("Trailer");

        serviceMock.Verify(service => service.GetMovieVideosAsync(
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
        var handler = new GetMovieVideosHandler(serviceMock.Object);
        var context = new DefaultHttpContext();

        if (id != null)
        {
            context.Request.RouteValues["id"] = id;
        }

        // Act
        var result = await handler.HandleAsync(context.Request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();

        serviceMock.Verify(service => service.GetMovieVideosAsync(
            It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
