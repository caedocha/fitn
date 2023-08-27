using fitnapi.Models;

namespace fitnapi.Events;

public class MoveEvent : IActionEvent
{

  public Fighter Fighter { get; set; }
  public string Direction { get; set; }

  public MoveEvent(Fighter fighter, string direction)
  {
    Fighter = fighter;
    Direction = direction;
  }

  public void Execute() {}
}
