namespace fitnapi.Models;

public class AllFights
{
  public List<Fight> Fights { get; set; }
  public long Count { get => Fights.Count; }

  public AllFights(List<Fight> fights)
  {
     Fights = fights; 
  }

  public void Add(Fight fight)
  {
    Fights.Add(fight);
  }

  public Fight Get(string id)
  {
    return Fights
      .Where(f => f.Name == id)
      .FirstOrDefault();
  }

}
