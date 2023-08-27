using fitnapi.Models;

namespace fitnapi.Events;

public class DeathEvent : IActionEvent
{

  public Fighter Fighter{ get; set; }

  public DeathEvent(Fighter fighter)
  {
    Fighter = fighter;
  }

  public void Execute() {}
}
