using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace MovieSearchCase.Infrastructure.Clients;

public class TmdbVideoResponse
{
    [JsonPropertyName("id")]
    public int Id { get; init; }

    [JsonPropertyName("results")]
    public required IReadOnlyList<TmdbVideo> Results { get; init; }
}

public class TmdbVideo
{
    [JsonPropertyName("key")]
    public required string Key { get; init; }

    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("site")]
    public required string Site { get; init; }

    [JsonPropertyName("type")]
    public required string Type { get; init; }
}
