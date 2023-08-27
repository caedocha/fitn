namespace fitnapi.Events;

public class PunchEvent : IActionEvent
{

  public int Damage { get; set; }
  public string Direction { get; set; }
  public int X { get; set; }
  public int Y { get; set; }

  public PunchEvent(int damage, string direction, int x, int y)
  {
    Damage = damage;
    Direction = direction;
    X = x;
    Y = y;
  }

  public void Execute() {}
}
