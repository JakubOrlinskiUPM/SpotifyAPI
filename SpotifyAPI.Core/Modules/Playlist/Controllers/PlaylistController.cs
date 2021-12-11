using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Core.Api;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Modules.Playlist.Queries;

namespace SpotifyAPI.Core.Modules.Playlist.Controllers;

public class PlaylistController : ApiController
{
    private readonly AppDb _db;
    private readonly IMediator _mediator;

    public PlaylistController(AppDb db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<GetPlaylists.Response> Get ([FromQuery] GetPlaylists.Request request)
    {
        var response = await _mediator.Request<GetPlaylists.Request, GetPlaylists.Response>(request);
        return response.Message;
    }
}