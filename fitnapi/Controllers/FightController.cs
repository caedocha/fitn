using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using fitnapi.Models;
using fitnapi.Repositories;
using System.Text.Json;

namespace fitnapi.Controllers;

[ApiController]
[Route("[controller]")]
public class FightController : ControllerBase
{
  private IConnectionMultiplexer _redis;
  private readonly ILogger<ActionsController> _logger;

  public FightController(ILogger<ActionsController> logger, IConnectionMultiplexer redis)
  {
    _logger = logger;
    _redis = redis;
  }

  [HttpGet]
  public IActionResult Get()
  {
    _logger.LogInformation("Getting list of fights");

    var repo = new FightsRepository(_logger, _redis);
    var fights = repo.AllFights((allFights, logger) =>
    {
      logger.LogInformation($"'{allFights.Count}' fights founds");
    });

    return Ok(fights);
  }

  [HttpPost]
  public IActionResult Start(string userId)
  {
    _logger.LogInformation($" User '{userId}' is starting a fight");

    var repo = new FightsRepository(_logger, _redis);
    repo.AllFights((fights, logger) =>
    {
      var newFight = new Fight(Fight.GenerateRandomName());
      var newFighter = new Fighter(userId);
      newFight.Join(newFighter);
      fights.Add(newFight);
    });

    return Ok();
  }

  [HttpPut]
  public IActionResult Join(string fightId, string userId)
  {
    _logger.LogInformation($"User '{userId}' is joining fight {fightId}");
    var repo = new FightsRepository(_logger, _redis);

    repo.SingleFight(fightId, (fight, logger) => 
    {
      fight.Populate();
      logger.LogInformation($"fight: {fight}");
      fight.Join(new Fighter(userId));
    });

    _logger.LogInformation($"User '{userId}' joined fight {fightId}");

    return Ok();
  }
}
