using fitnapi.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace fitnapi.Repositories;

class FightsRepository
{

  private ILogger Logger { get; set; }
  private IConnectionMultiplexer Redis { get; set; }

  public FightsRepository(ILogger logger, IConnectionMultiplexer redis)
  {
    Logger = logger;
    Redis = redis;
  }
  
  public AllFights AllFights(Action<AllFights, ILogger> block)
  {
    var redisDb = Redis.GetDatabase();
    var rawFights = redisDb.StringGet("fights");

    AllFights fights;
    if(string.IsNullOrEmpty(rawFights))
    {
      fights = new AllFights(new List<Fight>());
    }
    else
    {
      fights = JsonSerializer.Deserialize<AllFights>(rawFights);
    }

    block(fights, Logger);

    var json =  JsonSerializer.Serialize(fights);
    Logger.LogInformation($"JSON: '{json}'");
    redisDb.StringSet("fights", json);

    return fights;
  }

  public void SingleFight(string fightId, Action<Fight, ILogger> block)
  {
    var redisDb = Redis.GetDatabase();
    var json = redisDb.StringGet("fights");
    var fights = JsonSerializer.Deserialize<AllFights>(json);
    var fight = fights.Get(fightId);

    block(fight, Logger);

    redisDb.StringSet("fights", JsonSerializer.Serialize(fights));
  }

}
