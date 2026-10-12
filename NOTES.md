# Developer Implementation Notes: Movie Search Backend

Hey team, here is a quick breakdown of my approach to the tasks, what I prioritized, and my notes on the API contract for this submission.

---

## 1. What I Prioritized and Why

### Keeping the Controller/Handler Pattern Consistent

The baseline project sets up a Vertical Slice / REPR (Request-Endpoint-Response) pattern where the controller acts as a thin pass-through forwarding raw requests directly to specific handlers via `.HandleAsync(Request)`. I made sure to stick strictly to this design for the search, details, and video endpoints.

To ensure the frontend team gets full interactive text fields in the Swagger UI, I added explicit parameter bindings (`[FromQuery]` and `[FromRoute]`) to the controller signatures. This fixes the empty-box issue in Swagger while letting me keep the exact `HandleAsync(Request)` pattern you already had in place. I also added quick, fail-fast validation guards right at the controller layer to drop bad requests early.

### Cleaning Up "Attribute Soup" in the Controller

Decorating every endpoint method with 3 or 4 `[SwaggerResponse]` attributes for common errors (`400`, `404`, `503`) makes the controller messy and hard to scan. Instead, I wrote a custom `SwaggerResponseCodesFilter` (`IOperationFilter`) and hooked it into `SwaggerExtensions.cs`.

It automatically looks at the route shape behind the scenes and injects the right error documentation into Swagger based on whether the endpoint expects a query or a route ID. This keeps the controller super clean and readable while ensuring the API documentation contract remains 100% accurate.

### Centralized, Polymorphic Error Handling

I wanted to handle errors cleanly without building heavy custom middleware from scratch. I noticed the domain project already has an abstract `ProblemDetailsException` base class and a handy `.ToStatusCode()` extension.

I extended the native `.AddProblemDetails()` configuration inside `ProblemDetailsExtensions.cs` to look for this abstract base class. Now, whenever a client or handler throws a domain exception, the framework catches it globally, reads the enum status code, and outputs a properly formatted `application/problem+json` payload. This keeps the execution code clean and completely hides raw .NET stack traces from the client.

### Bulletproof Unit Tests

For the services and handlers, I focused on high-performance passthroughs (using direct expression tasks `=>` to avoid unnecessary object mapping allocations). On the testing side, I leaned heavily on FluentAssertions. I avoided using string-based reflection stubs (`GetProperty().GetValue()`) for the anonymous search payloads, switching instead to structural matchers (`.Should().BeEquivalentTo(new { ... })`). This ensures the tests won't break due to compiler optimizations or JSON casing shifts in a CI/CD environment.

---

## 2. What I Skipped and Why

### TV/Series Namespace (P3 Stretch Goal)

I decided to skip the optional `/tv` endpoints. I felt it was much better to spend my time hardening the core movie features, ensuring the validation logic was rock solid, and making sure the test suite was complete rather than shipping a partially finished secondary feature.

### Caching Layers (P3 Stretch Goal)

While caching TMDB data is great for production scale, introducing memory cache providers can muddy the waters when evaluating core clean-architecture boundaries. I stuck to making the standard asynchronous pipeline as lean and performant as possible.

---

## 3. API Contract and Mapping Decisions

- **Handling Network vs. Data Faults:** In `TmdbClient.cs`, I made a point to explicitly catch standard `404 NotFound` errors from TMDB and throw them as `ErrorType.EntityNotFound`. General network dropouts throw as `ErrorType.UpstreamServiceUnavailable`. This distinction ensures that if a user requests a movie ID that doesn't exist, they get a proper `404` error payload instead of a `503` message falsely claiming the entire TMDB service is down.
- **Introduced Infrastructure Video DTOs & Mappers (P2):** Since the baseline project template only supported trending endpoints and lacked video models, I introduced the `TmdbVideoResponse` and `TmdbVideo` DTOs inside the Infrastructure layer to parse raw network results from TMDB's `/movie/{id}/videos` endpoint. I also added `TmdbVideoMapper` to handle incoming data translation cleanly.
- **Strongly Typed Public Mappings:** I expanded `MovieMapper.cs` in the WebApi layer to include a clear mapping extension profile for `MovieVideo` records. This allowed me to avoid using dynamic anonymous objects inside `GetMovieVideosHandler`, ensuring the videos endpoint remains strongly typed and consistent with the rest of the application's design.
