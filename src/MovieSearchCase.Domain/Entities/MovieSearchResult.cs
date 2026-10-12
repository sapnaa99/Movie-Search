using System;
using System.Collections.Generic;
using System.Text;

namespace MovieSearchCase.Domain.Entities;

public class MovieSearchResult
{
    public int Page { get; init; }

    public int TotalPages { get; init; }

    public int TotalResults { get; init; }

    public required IReadOnlyList<Movie> Results { get; init; }
}
