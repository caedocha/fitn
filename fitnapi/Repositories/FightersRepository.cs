using fitnapi.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace fitnapi.Repositories;

class FightersRepository
{
  private ILogger Logger { get; set; }
  private IConnectionMultiplexer Redis { get; set; }

  public FightersRepository(ILogger logger, IConnectionMultiplexer redis)
  {
    Logger = logger;
    Redis = redis;
  }
  
  public void SingleFighter(string userId, string fightId, Action<Fighter, Fight, ILogger> block)
  {
    var redisDb = Redis.GetDatabase();
    var json = redisDb.StringGet("fights");
    var fights = JsonSerializer.Deserialize<AllFights>(json);
    var fight = fights.Get(fightId);
    var fighter = fight.Fighters.Where(f => f.Name == userId).FirstOrDefault();

    block(fighter, fight, Logger);

    redisDb.StringSet("fights", JsonSerializer.Serialize(fights));
  }

}
