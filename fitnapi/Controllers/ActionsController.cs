using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using fitnapi.Models;
using fitnapi.Repositories;
using System.Text.Json;

namespace fitnapi.Controllers;

[ApiController]
[Route("[controller]")]
public class ActionsController : ControllerBase
{
  private ILogger<ActionsController> _logger;
  private IConnectionMultiplexer _redis;

  public ActionsController(ILogger<ActionsController> logger, IConnectionMultiplexer redis)
  {
      _logger = logger;
      _redis = redis;
  }

  [HttpPost]
  [Route("/action")]
  public IActionResult ExecuteAction([FromBody]string action)
  {
    _logger.LogInformation($"Action executed: '{action}'");

    return Ok();
  }

  [HttpPost]
  [Route("/rotate")]
  public IActionResult Rotate(string userId, string fightId)
  {
    var repo = new FightersRepository(_logger, _redis);

    repo.SingleFighter(userId, fightId, (fighter, fight, logger) =>
    {
      logger.LogInformation($"Fighter {fighter.Name} rotated");
      fighter.Rotate();
    });

    return Ok();
  }

  [HttpPost]
  [Route("/punch")]
  public IActionResult Punch(string userId, string fightId)
  {
    var repo = new FightersRepository(_logger, _redis);

    repo.SingleFighter(userId, fightId, (fighter, fight, logger) =>
    {
      fight.Populate();
      var punchEvent = fighter.Punch();
      logger.LogInformation($"Fighter {fighter.Name} punched");
      fight.PropagatePunch(punchEvent);
      logger.LogInformation($"Punch was propagated");
    });

    return Ok();
  }

  [HttpPost]
  [Route("/move")]
  public IActionResult Punch(string userId, string fightId, string direction)
  {
    var repo = new FightersRepository(_logger, _redis);

    repo.SingleFighter(userId, fightId, (fighter, fight, logger) =>
    {
      fight.Populate();
      logger.LogInformation($"Fighter {fighter.Name} moved");
      var moveEvent = fighter.Move(direction);
      fight.PropagateMove(moveEvent);
      logger.LogInformation($"Move was propagated");
    });

    return Ok();
  }
}
