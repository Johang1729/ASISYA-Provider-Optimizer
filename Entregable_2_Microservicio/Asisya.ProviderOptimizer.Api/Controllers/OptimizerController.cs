using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Asisya.ProviderOptimizer.Application.Commands;
using Asisya.ProviderOptimizer.Application.Queries;

namespace Asisya.ProviderOptimizer.Api.Controllers;

[ApiController]
[Route("api/v1/optimizer")]
public class OptimizerController : ControllerBase
{
    private readonly IMediator _mediator;

    public OptimizerController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("optimize")]
    public async Task<IActionResult> Optimize([FromBody] OptimizeAssignmentCommand command, CancellationToken cancellationToken)
    {
        if (command == null) 
            return BadRequest(new { MatchSuccess = false, Error = "Request body is required." });

        try
        {
            var result = await _mediator.Send(command, cancellationToken);
            
            return Ok(new {
                Message = "Assignment optimization completed.",
                MatchSuccess = true,
                Data = result
            });
        }
        catch (InvalidOperationException ex)
        {
            return NotFound(new { MatchSuccess = false, Error = ex.Message });
        }
    }
    
    [HttpGet("providers/available")]
    public async Task<IActionResult> GetAvailableProviders([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radius = 20.0, CancellationToken cancellationToken = default)
    {
        var query = new GetAvailableProvidersQuery { Lat = lat, Lng = lng, RadiusKm = radius };
        var result = await _mediator.Send(query, cancellationToken);
        return Ok(new { Total = result.Count, Radius = radius, Providers = result });
    }
}

