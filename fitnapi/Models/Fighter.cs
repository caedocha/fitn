using fitnapi.Events;

namespace fitnapi.Models;

public class Fighter
{
  private Dictionary<string, string> directionsMapping = new Dictionary<string, string>
  {
    {"N", "E"},
    {"E", "S"},
    {"S", "W"},
    {"W", "N"}
  };

  public string Name { get; set; }
  public int Health { get; set; }
  public string Direction { get; set; }
  public Tile Tile { get; set; }

  private int punchDamage = 2;

  public Fighter(string name, int health = 10, string direction = "N")
  {
     Name = name; 
     Health = health;
     Direction = direction;
  }

  public MoveEvent Move(string direction)
  {
    var moveEvent = new MoveEvent(this, direction);
    return moveEvent;
  }

  public void Rotate()
  {
    var newDirection = directionsMapping[Direction];
    Direction = newDirection;
  }

  public PunchEvent Punch()
  {
    var punchEvent = new PunchEvent(punchDamage, Direction, Tile.X, Tile.Y);
    return punchEvent;
  }

  public void ListenToPunchEvent(PunchEvent punch)
  {
    var punchDamage = punch.Damage;
    System.Console.WriteLine($"Receiving '{punchDamage}' damage");
    Health -= punchDamage;
    System.Console.WriteLine($"Health is '{Health}'");
  }
}
