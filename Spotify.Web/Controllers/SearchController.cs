using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Spotify.Application.DTOs.Search;
using Spotify.Application.Interfaces;

namespace Spotify.Web.Controllers;

[ApiController]
[Route("api")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public sealed class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    [HttpGet("tracks/search/{maxPerPage:int}/{page:int}")]
    public Task<ActionResult<SearchPage<SearchTrackResponse>>> SearchTracks(
        [FromQuery] string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken)
        => SearchAsync(
            query, maxPerPage, page,
            _searchService.SearchTracksAsync,
            cancellationToken);

    [HttpGet("albums/search/{maxPerPage:int}/{page:int}")]
    public Task<ActionResult<SearchPage<SearchAlbumResponse>>> SearchAlbums(
        [FromQuery] string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken)
        => SearchAsync(
            query, maxPerPage, page,
            _searchService.SearchAlbumsAsync,
            cancellationToken);

    [HttpGet("authors/search/{maxPerPage:int}/{page:int}")]
    public Task<ActionResult<SearchPage<SearchAuthorResponse>>> SearchAuthors(
        [FromQuery] string query,
        int maxPerPage,
        int page,
        CancellationToken cancellationToken)
        => SearchAsync(
            query, maxPerPage, page,
            _searchService.SearchAuthorsAsync,
            cancellationToken);

    private async Task<ActionResult<SearchPage<T>>> SearchAsync<T>(
        string query,
        int maxPerPage,
        int page,
        Func<string, int, int, CancellationToken, Task<SearchPage<T>>> search,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query is required.");

        if (maxPerPage is < 1 or > 100)
            return BadRequest("maxPerPage must be between 1 and 100.");

        if (page < 1)
            return BadRequest("page must be a positive integer.");

        return Ok(await search(query.Trim(), maxPerPage, page, cancellationToken));
    }
}
