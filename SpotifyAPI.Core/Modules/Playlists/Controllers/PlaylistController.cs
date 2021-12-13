using System.Threading.Tasks;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Core.Api;
using SpotifyAPI.Core.Bus;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Modules.Playlists.Commands;
using SpotifyAPI.Core.Modules.Playlists.Queries;

namespace SpotifyAPI.Core.Modules.Playlists.Controllers;

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
    
    [HttpPost]
    public async Task<CreatePlaylist.Response> Create ([FromBody] CreatePlaylist.Request request)
    {
        var response = await _mediator.Request<CreatePlaylist.Request, CreatePlaylist.Response>(request);
        return response.Message;
    }
}