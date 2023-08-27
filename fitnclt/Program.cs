using System.Text.Json;
using System.Linq;
using System.Net;

string url = "http://localhost:5211";

Console.Clear();
MainMenuResult mainMenuResult = new MainMenuResult();

PrintUserMenu();
var fighterName = Console.ReadLine();
Console.Clear();

PrintFightMenu();
var command = Console.ReadLine();

switch(command)
{
  case "s":
    mainMenuResult = await StartFight(fighterName);
    break;
  case "j":
    mainMenuResult = await JoinFight(fighterName);
    break;
  default:
    UnrecognizeCommand();
    break;
}


Console.Clear();

if(mainMenuResult.Result == HttpStatusCode.OK)
{
  await StartFighting(mainMenuResult);
}
else
{
  System.Console.WriteLine("Whoops, something went wrong. Maybe the streets ain't ready for you!");
}

async Task StartFighting(MainMenuResult result)
{
  var actionResult = "";
  var keepGoing = true;
  while(keepGoing)
  {
    PrintFightingMenu();
    PrintFightingResult(actionResult);
    var command = Console.ReadLine();

    Console.WriteLine("What you gonna do?");
    switch(command)
    {
      case "r":
        actionResult = await Rotate(result);
        break;
      case "p":
        actionResult = await Punch(result);
        break;
      case "m":
        actionResult = await Move(result);
        break;
      case "e":
        keepGoing = false;
        break;
      default:
        UnrecognizeCommand();
        break;
    }
    Console.Clear();
  }
}

void PrintFightingResult(string result)
{
  if(string.IsNullOrEmpty(result)) { return; }

  System.Console.WriteLine(result);
}

async Task<string> Rotate(MainMenuResult result)
{
  var responseResult = "";
  var client = new HttpClient()
  {
    BaseAddress = new Uri(url),
  };

  var response = await client.PostAsync(
      $"/rotate?fightId={result.FightId}&userId={result.UserId}",
      new StringContent(""));

  if(response.StatusCode == HttpStatusCode.OK)
  {
    responseResult = "Rotated!";
  }
  else{
    responseResult = "Whoops!";
  }

  return responseResult;
}

async Task<string> Punch(MainMenuResult result)
{
  var responseResult = "";
  var client = new HttpClient()
  {
    BaseAddress = new Uri(url),
  };

  var response = await client.PostAsync(
      $"/punch?fightId={result.FightId}&userId={result.UserId}",
      new StringContent(""));

  if(response.StatusCode == HttpStatusCode.OK)
  {
    responseResult = "Punched!";
  }
  else{
    responseResult = "Whoops!";
  }

  return responseResult;
}

async Task<string> Move(MainMenuResult result)
{
  Console.WriteLine("Which direction? (N, W, S, E)");
  var direction = Console.ReadLine();
  var responseResult = "";

  var client = new HttpClient()
  {
    BaseAddress = new Uri(url),
  };

  var response = await client.PostAsync(
      $"/move?fightId={result.FightId}&userId={result.UserId}&direction={direction}",
      new StringContent(""));

  if(response.StatusCode == HttpStatusCode.OK)
  {
    responseResult = "Moved!";
  }
  else{
    responseResult = "Whoops!";
  }

  return responseResult;
}

void PrintFightingMenu()
{
  var menu = @"
    *******************************************
    *                                         *
    *                                         *
    *          'r' - Rotate                   *
    *          'p' - Punch                    *
    *          'm' - Move                     *
    *          'e' - Exit                     *
    *                                         *
    *******************************************
    ";
  Console.WriteLine(menu);

}

void PrintUserMenu()
{

  var menu = @"
    *******************************************
    *                                         *
    *          - FIGHTS AT NIGHT -            *
    *                                         *
    *                                         *
    *                                         *
    *******************************************
    ";
  Console.WriteLine(menu);
  Console.WriteLine("What is your fighter name?");
}
void PrintFightMenu()
{
  var menu = @"
    *******************************************
    *                                         *
    *                                         *
    *          's' - Start a fight            *
    *          'j' - Join a fight             *
    *                                         *
    *******************************************
    ";
  Console.WriteLine(menu);
  Console.WriteLine("What you gonna do?");
}

void PrintJoinAFightMenu(List<string> fightNames)
{
  var menu = @"
    ***********************************************************************
    *                                                                     *
    *          - THESE ARE THE FIGHTS THAT ARE GOING DOWN FAM -           *";

  menu += "\n";

  for (int i = 0; i < fightNames.Count; i++)
  {
     menu += $"    *               {i} - {fightNames[i]}                                         * \n";
  }
  
  menu += @"    *                                                                     *
    *                                                                     *
    ***********************************************************************
    ";
  Console.WriteLine(menu);
}

async Task<MainMenuResult> JoinFight(string fighterName)
{
  var allFights = await ListFights();
  var fightNames = allFights
    .Fights
    .Select(fight => fight.Name)
    .ToList<string>();

  Console.Clear();
  PrintJoinAFightMenu(fightNames);
  Console.WriteLine("What's your pick? (Use the fight number)");

  var fightNumber = Console.ReadLine(); 
  var pickedFight = allFights
    .Fights
    .ElementAt(int.Parse(fightNumber));

  System.Console.WriteLine(pickedFight.Name);

  var responseResult = await UpdateFight(pickedFight.Name, fighterName);

  var mainMenuResult = new MainMenuResult
  {
    UserId = fighterName,
    FightId = pickedFight.Name,
    Result = responseResult
  };

  return mainMenuResult;
}

async Task<MainMenuResult> StartFight(string figherName)
{
  return new MainMenuResult();
}

async Task<HttpStatusCode> UpdateFight(string fightName, string userId)
{
  var client = new HttpClient()
  {
    BaseAddress = new Uri(url),
  };

  var response = await client.PutAsync($"/fight?fightId={fightName}&userId={userId}", new StringContent(""));

  return response.StatusCode;
}

async Task<AllFights> ListFights()
{
  var client = new HttpClient()
  {
    BaseAddress = new Uri(url),
  };

  var response = await client.GetAsync("/fight");

  response.EnsureSuccessStatusCode();

  var rawJson = await response.Content.ReadAsStringAsync();

  var jsonOptions = new JsonSerializerOptions
  {
    PropertyNameCaseInsensitive = true
  };

  var fights = JsonSerializer.Deserialize<AllFights>(rawJson, jsonOptions);

  return fights;
}

void UnrecognizeCommand()
{
  Console.WriteLine("Unrecognize command, try another one!");
}
