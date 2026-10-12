
using MovieSearchCase.Domain.Entities;

namespace MovieSearchCase.Infrastructure.Clients;

public static class TmdbVideoMapper
{
    public static MovieVideo ToDomainModel(TmdbVideo video)
    {
        return new MovieVideo
        {
            Key = video.Key,
            Name = video.Name,
            Site = video.Site,
            Type = video.Type
        };
    }
}
