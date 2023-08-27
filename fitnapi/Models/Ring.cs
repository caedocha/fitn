using fitnapi.Events;

namespace fitnapi.Models;

public class Ring
{
  public Tile[,] Tiles { get; set; }
  public Fight Fight { get; set; }

  private const int xAxisSize = 2;
  private const int yAxisSize = 2;

  private Dictionary<string, int> directionVectorMapping = new Dictionary<string, int>
  {
    {"N", 1},
    {"E", 1},
    {"S", -1},
    {"W", -1}
  };

  private Dictionary<string, int> movementVectorMapping = new Dictionary<string, int>
  {
    {"N", -1},
    {"E", 1},
    {"S", 1},
    {"W", -1}
  };

  public Ring(Fight fight)
  {
    Fight = fight;
    Tiles = new Tile[xAxisSize, yAxisSize];
    InitilizeTiles();
  }

  public void Add(Fighter fighter)
  {
    var tile = GetRandomEmptyTile();
    tile.Add(fighter);
    fighter.Tile = tile;
  }

  public void Populate(Fighter fighter)
  {
    var fighterTile = fighter.Tile;
    var ringTile = Tiles[fighterTile.X, fighterTile.Y];
    ringTile.Add(fighter);
  }

  public void PropagateMove(MoveEvent moveEvent)
  {
    int newX;
    int newY;
    var fighter = moveEvent.Fighter;
    var fighterTile = fighter.Tile;
    var direction = moveEvent.Direction;
    var vector = movementVectorMapping[direction];

    if( direction == "N" || direction == "S")
    {
      newX = fighterTile.X; 
      newY =  fighterTile.Y + vector;
    }
    else
    {
      newX = fighterTile.X + vector;
      newY = fighterTile.Y;
    }

    if(IsCoordinatesInsideBounds(newX, newY))
    {
      var newTile = Tiles[newX, newY];

      if(newTile.IsEmpty())
      {
        newTile.Fighter = fighter;
        fighter.Tile = newTile;
      }
    }
  }

  public void PropagateDeath(DeathEvent deathEvent)
  {
    for (int i = 0; i < xAxisSize; i++)
    {
      for (int j = 0; j < yAxisSize; j++)
      {
        var tile = Tiles[i, j];
        if(!tile.IsEmpty())
        {
          if(tile.Fighter.Name == deathEvent.Fighter.Name)
          {
            tile.Fighter = null;
          }
        }
      } 
    }
  }

  public void PropagatePunch(PunchEvent punchEvent)
  {
    int newX;
    int newY;

    var x = punchEvent.X;
    var y = punchEvent.Y;
    var direction = punchEvent.Direction;
    var vector = directionVectorMapping[direction];

    if( direction == "N" || direction == "S")
    {
      newX = x + vector;
      newY = y;
    }
    else
    {
      newX = x; 
      newY = y + vector;
    }

    System.Console.WriteLine("Punch event is in the ring");
    System.Console.WriteLine($"Punch info: newX '{newX}', newY: '{newY}'");

    if(IsCoordinatesInsideBounds(newX, newY))
    {
      System.Console.WriteLine("Punch is within bounds");
      var tile = Tiles[newX, newY];

      if(!tile.IsEmpty())
      {
        System.Console.WriteLine("Tile is not empty");
        var fighter = tile.Fighter;
        fighter.ListenToPunchEvent(punchEvent);
        System.Console.WriteLine($"Fighter '{fighter.Name}' received the punch");
        if(fighter.Health <= 0)
        {
          var deathEvent = new DeathEvent(fighter);
          Fight.PropagateDeath(deathEvent);
        }
      }
      else
      {
        System.Console.WriteLine("Tile is empty");
      }
    }
    else
    {
      System.Console.WriteLine("Punch is out of bounds");
    }
  }

  private bool IsCoordinatesInsideBounds(int x, int y)
  {
    return (x >= 0 && x < xAxisSize) && (y >= 0 && y < yAxisSize);
  }

  public bool IsFull()
  {
    var isFull = true;

    for (int i = 0; i < xAxisSize; i++)
    {
       for (int j = 0; j < yAxisSize; j++)
       {
         var tile = Tiles[i, j];
         if(tile.IsEmpty())
         {
           isFull = false;
         }
       } 
    }

    return isFull;
  }

  private Tile? GetRandomEmptyTile()
  {
    Tile tile = null;
    var keepLooking = true;
    var rand = new Random();

    while(keepLooking)
    {
      var x = rand.Next(xAxisSize);
      var y = rand.Next(yAxisSize);
      tile = Tiles[x, y];

      if(tile.IsEmpty())
      {
        keepLooking = false;
      }
    }

    return tile;
  }

  private void InitilizeTiles()
  {
    for (int i = 0; i < xAxisSize; i++)
    {
       for (int j = 0; j < yAxisSize; j++)
       {
          Tiles[i, j] = new Tile(i, j); 
       } 
    }
  }
}
