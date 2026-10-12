using System;
using System.Collections.Generic;
using System.Text;

namespace MovieSearchCase.Domain.Entities;

public class MovieVideo
{
    public string Key { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string Site { get; init; } = string.Empty;

    public string Type { get; init; } = string.Empty;
}
