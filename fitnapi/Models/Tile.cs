using System.Text.Json;
using System.Text.Json.Serialization;

namespace fitnapi.Models;

public class Tile
{
  public int X { get; set; }
  public int Y { get; set; }

  [JsonIgnore]
  public Fighter? Fighter { get; set; }

  public Tile(int x, int y)
  {
     X = x;
     Y = y;
     Fighter = null;
  }

  public bool IsEmpty()
  {
    return (Fighter == null);
  }

  public void Add(Fighter fighter)
  {
    if(!IsEmpty()) { return; }

    Fighter = fighter;
  }
  
}
