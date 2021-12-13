using System.Threading.Tasks;
using MassTransit.Mediator;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Core.Api;
using SpotifyAPI.Core.Bus;
using SpotifyAPI.Core.Database;
using SpotifyAPI.Core.Modules.Authors.Queries;

namespace SpotifyAPI.Core.Modules.Authors.Controllers;

public class AuthorController : ApiController
{
    private readonly AppDb _db;
    private readonly IMediator _mediator;

    public AuthorController(AppDb db, IMediator mediator)
    {
        _db = db;
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<GetAuthors.Response> Get ([FromQuery] GetAuthors.Request request)
    {
        var response = await _mediator.Request<GetAuthors.Request, GetAuthors.Response>(request);
        return response.Message;
    }
}