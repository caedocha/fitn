using fitnapi.Events;

namespace fitnapi.Models;

public class Fight
{
  public string Name { get; set; }
  public List<Fighter> Fighters { get; set; }

  private Ring Ring { get; set; }

  public Fight(string name)
  {
     Name = name; 
     Ring = new Ring(this);
     Fighters = new List<Fighter>();
  }  

  public void PropagateMove(MoveEvent moveEvent)
  {
    Ring.PropagateMove(moveEvent);
  }

  public void PropagatePunch(PunchEvent punchEvent)
  {
    Ring.PropagatePunch(punchEvent);
  }

  public void PropagateDeath(DeathEvent deathEvent)
  {
    Fighters.Remove(deathEvent.Fighter);
    Ring.PropagateDeath(deathEvent);
  }

  public void Populate()
  {
    foreach (var fighter in Fighters)
    {
      Ring.Populate(fighter);        
    }
  }

  public void Join(Fighter fighter)
  {
    if(Ring.IsFull()) { return; }

    Fighters.Add(fighter);
    Ring.Add(fighter);
  }

  public static string GenerateRandomName()
  {
    var rand = new Random();
    return $"fight{rand.Next(10000)}";
  }

  public override string ToString()
  {
    return $"Fight name: {Name}, fighters: '{string.Join(", ", Fighters.Select(f => f.Name))}', ring isFull: '{Ring.IsFull()}'";
  }
}
