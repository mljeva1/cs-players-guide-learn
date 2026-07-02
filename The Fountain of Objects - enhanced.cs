DisplayIntro();
Console.WriteLine("Please select a game size: small, medium or large:");
string? input = Console.ReadLine();
Game? game = input switch
{
    "small" => CreateSmallGame(),
    "medium" => CreateMediumGame(),
    "large" => CreateLargeGame(),
    _ => null
};
if (game == null) { Console.WriteLine("Invalid game size. Please select small, medium or large."); }
else game.Run();
static Game CreateSmallGame()
{
    Map map = new Map(4, 4);
    map.SetRoomType(0, 2, RoomType.Fountain);
    map.SetRoomType(0, 1, RoomType.Maelstrom);
    map.SetRoomType(0, 0, RoomType.Entrance);
    map.SetRoomType(3, 1, RoomType.PitRoom);
    map.SetRoomType(3, 2, RoomType.Amarok);
    Location start = new Location(map, 0, 0);
    Player player = new Player(start);
    return new Game(map, player);
}
static Game CreateMediumGame()
{
    Map map = new Map(6, 6);
    map.SetRoomType(3, 3, RoomType.Fountain);
    map.SetRoomType(1, 1, RoomType.Entrance);
    map.SetRoomType(2, 1, RoomType.Maelstrom);
    map.SetRoomType(3, 2, RoomType.PitRoom);
    map.SetRoomType(5, 5, RoomType.PitRoom);
    Location start = new Location(map, 1, 1);
    Player player = new Player(start);
    return new Game(map, player);
}
static Game CreateLargeGame()
{
    Map map = new Map(8, 8);
    map.SetRoomType(2, 6, RoomType.Fountain);
    map.SetRoomType(2, 2, RoomType.Entrance);
    map.SetRoomType(2, 1, RoomType.Maelstrom);
    map.SetRoomType(3, 2, RoomType.PitRoom);
    map.SetRoomType(6, 6, RoomType.PitRoom);
    map.SetRoomType(1, 1, RoomType.PitRoom);
    map.SetRoomType(7, 6, RoomType.PitRoom);
    Location start = new Location(map, 2, 2);
    Player player = new Player(start);
    return new Game(map, player);
}
void DisplayIntro()
{
    Console.WriteLine("You enter the Cavern of Objects, a maze filled with dangerous pits, in search");
    Console.WriteLine("of the Fountain of Objects.");
    Console.WriteLine("Light is visible only in the entrance, and no other light is seen anywhere in the caverns.");
    Console.WriteLine("You must navigate the Caverns with your other senses.");
    Console.WriteLine("Find the Fountain of Objects, activate it, and return to the entrance.");
}
public class Game(Map map, Player player)
{
    public bool IsRunning { get; private set; } = true;
    public bool FountainEnabled { get; private set; } = false;
    public Map map { get; } = map;
    public Player player { get; } = player;
    public void Run()
    {
        while(IsRunning)
        {
            player.location.Current(this);
            if(!IsRunning) { break; }
            player.location.Move(this);
        }
    }
    public void End() { IsRunning = false; }
    public void EnableFountain()
    {
        FountainEnabled = true;
        Console.WriteLine("You hear rushing waters from the Fountain of Objects. It has been reactivated!");
    }
}
public readonly struct Player(Location start)
{
    public Location location { get; } = start;
}
public class Map
{
    public RoomType[,] Grid { get; }
    public Map(int row, int column)
    {
        Grid = new RoomType[row, column];
    }
    public Map SetRoomType(int row, int column, RoomType type)
    {
        Grid[row, column] = type;
        return this;
    }
    public bool IsOnMap(int row, int column)
    {
        if (Grid == null) return false;
        return row >= 0 && row < Grid.GetLength(0)
            && column >= 0 && column < Grid.GetLength(1);
    }
    public bool CheckRoomType(int row, int column, RoomType type)
    {
        return IsOnMap(row, column) && Grid[row, column] == type;
    }
    public bool Sense(int row, int column)
    {
        if (Grid == null) return false;
        else if (IsOnMap(row - 1, column) && Grid[row - 1, column] == RoomType.PitRoom) return true;
        else if (IsOnMap(row + 1, column) && Grid[row + 1, column] == RoomType.PitRoom) return true;
        else if (IsOnMap(row, column - 1) && Grid[row, column - 1] == RoomType.PitRoom) return true;
        else if (IsOnMap(row, column + 1) && Grid[row, column + 1] == RoomType.PitRoom) return true;
        else if (IsOnMap(row - 1, column) && Grid[row - 1, column] == RoomType.Maelstrom) return true;
        else if (IsOnMap(row + 1, column) && Grid[row + 1, column] == RoomType.Maelstrom) return true;
        else if (IsOnMap(row, column - 1) && Grid[row, column - 1] == RoomType.Maelstrom) return true;
        else if (IsOnMap(row, column + 1) && Grid[row, column + 1] == RoomType.Maelstrom) return true;
        else if (IsOnMap(row - 1, column) && Grid[row - 1, column] == RoomType.Amarok) return true;
        else if (IsOnMap(row + 1, column) && Grid[row + 1, column] == RoomType.Amarok) return true;
        else if (IsOnMap(row, column - 1) && Grid[row, column - 1] == RoomType.Amarok) return true;
        else if (IsOnMap(row, column + 1) && Grid[row, column + 1] == RoomType.Amarok) return true;
        else return false;
    }
}
public class Location
{
    private readonly Map _map;
    public static int[]? Player { get; private set; }
    public int Row { get; private set; }
    public int Column { get; private set; }
    public int ArrowsRemaining { get; private set; } = 5;

    // accept a Map in the constructor
    public Location(Map map, int startRow, int startColumn)
    {
        _map = map;
        Row = startRow;
        Column = startColumn;
        Player = [startRow, startColumn];
    }
    private void ShootArrowAtMonster(int targetRow, int targetColumn)
    {
        if (ArrowsRemaining == 0)
        {
            Console.WriteLine("You are out of arrows.");
        }

        ArrowsRemaining--;
        if (!_map.IsOnMap(targetRow, targetColumn))
        {
            Console.WriteLine("You missed! The arrow flew beyond the map.");
            return;
        }

        var room = _map.Grid[targetRow, targetColumn];

        if (room == RoomType.Amarok || room == RoomType.Maelstrom)
        {
            Console.WriteLine($"You hit the {room}!");
            _map.SetRoomType(targetRow, targetColumn, RoomType.Empty);
        }
        else
        {
            Console.WriteLine("You missed!");
        }
    }
    private void HelpCommand()
    {
        Console.WriteLine("help");
        Console.WriteLine("    Displays this help information.");
        Console.WriteLine("enable fountain");
        Console.WriteLine("    Turns on the Fountain of Objects if you are in the fountain room, or does nothing if you are not.");
        Console.WriteLine("move north");
        Console.WriteLine("    Moves to the room directly north of the current room, as long as there are no walls.");
        Console.WriteLine("move south");
        Console.WriteLine("    Moves to the room directly south of the current room, as long as there are no walls.");
        Console.WriteLine("move east");
        Console.WriteLine("    Moves to the room directly east of the current room, as long as there are no walls.");
        Console.WriteLine("move west");
        Console.WriteLine("    Moves to the room directly west of the current room, as long as there are no walls.");
    }
    public void Move(Game game)
    {
        Console.WriteLine("What do you want to do? ");
        string? response = Console.ReadLine();
        switch (response)
        {
            case "move north":
                if (_map.IsOnMap(Row - 1, Column)) Row--;
                else { Console.WriteLine("You cannot move north. You are at the edge of the map."); }
                break;
            case "move south":
                if (_map.IsOnMap(Row + 1, Column)) Row++;
                else { Console.WriteLine("You cannot move south. You are at the edge of the map."); }
                break;
            case "move east":
                if (_map.IsOnMap(Row, Column + 1)) Column++;
                else { Console.WriteLine("You cannot move east. You are at the edge of the map."); }
                break;
            case "move west":
                if (_map.IsOnMap(Row, Column - 1)) Column--;
                else { Console.WriteLine("You cannot move west. You are at the edge of the map."); }
                break;
            case "enable fountain":
                if (_map.CheckRoomType(Row, Column, RoomType.Fountain)) { game.EnableFountain(); }
                else { Console.WriteLine("You are not in the Fountain room."); }
                break;
            case "shoot north":
                ShootArrowAtMonster(Row - 1, Column);
                break;
            case "shoot south":
                ShootArrowAtMonster(Row + 1, Column);
                break;
            case "shoot east":
                ShootArrowAtMonster(Row, Column + 1);
                break;
            case "shoot west":
                ShootArrowAtMonster(Row, Column - 1);
                break;
            case "help":
                HelpCommand();
                break;
            default:
                Console.WriteLine("Invalid command. Type 'help' for a list of commands.");
                break;
        }
        Player = [Row, Column];
        Console.WriteLine();
    }
    public void Current(Game game)
    {
        Console.WriteLine("-------------------------------------------------------");
        Console.WriteLine($"You are in the room at (Row={Row}, Column={Column})");
        Console.WriteLine($"You have {ArrowsRemaining}/5 arrows remaining.");
        if (game.FountainEnabled && _map.CheckRoomType(Row, Column, RoomType.Entrance)) {
            Console.WriteLine("The Fountain of Objects has been reactivated, and you have escaped with your life!");
            Console.WriteLine("You win!");
            game.End();
        }
        else if (_map.CheckRoomType(Row, Column, RoomType.Fountain) && !game.FountainEnabled)
        {
            Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
        }
        else if(!game.FountainEnabled && _map.CheckRoomType(Row, Column, RoomType.Entrance)) {
            Console.WriteLine("You see light coming from the cavern entrance.");
        }
        else if(_map.CheckRoomType(Row, Column, RoomType.PitRoom))
        {
            Console.WriteLine("You fell into a pit and died!");
            game.End();
        }
        else if (_map.CheckRoomType(Row, Column, RoomType.Amarok))
        {
            Console.WriteLine("You encountered an amarok and died!");
            game.End();
        }
        else if (_map.CheckRoomType(Row, Column, RoomType.Maelstrom))
        {
            int oldRow = Row;
            int oldColumn = Column;

            int lastRow = _map.Grid!.GetLength(0) - 1;
            int lastColumn = _map.Grid.GetLength(1) - 1;

            int playerNewRow = oldRow - 1;
            int playerNewColumn = oldColumn + 2;

            if (playerNewRow < 0) playerNewRow = lastRow;
            if (playerNewColumn > lastColumn) playerNewColumn = 0;

            int maelNewRow = oldRow + 1;
            int maelNewColumn = oldColumn - 2;

            if (maelNewRow > lastRow) maelNewRow = 0;
            if (maelNewColumn < 0) maelNewColumn = lastColumn;

            if (_map.CheckRoomType(maelNewRow, maelNewColumn, RoomType.Fountain))
            {
                maelNewColumn--;

                if (maelNewColumn < 0)
                {
                    maelNewColumn = lastColumn;
                }
            }

            _map.SetRoomType(oldRow, oldColumn, RoomType.Empty);
            _map.SetRoomType(maelNewRow, maelNewColumn, RoomType.Maelstrom);

            Row = playerNewRow;
            Column = playerNewColumn;
            Player = [Row, Column];

            Console.WriteLine($"You were sucked into a maelstrom and moved to a different room! Row: {Row}, Column: {Column}");
        }
        else if (_map.Sense(Row, Column))
        {
            if (_map.IsOnMap(Row - 1, Column) && _map.CheckRoomType(Row - 1, Column, RoomType.PitRoom)) Console.WriteLine("You feel a draft. There is a pit nearby.");
            if (_map.IsOnMap(Row + 1, Column) && _map.CheckRoomType(Row + 1, Column, RoomType.PitRoom)) Console.WriteLine("You feel a draft. There is a pit nearby.");
            if (_map.IsOnMap(Row, Column - 1) && _map.CheckRoomType(Row, Column - 1, RoomType.PitRoom)) Console.WriteLine("You feel a draft. There is a pit nearby.");
            if (_map.IsOnMap(Row, Column + 1) && _map.CheckRoomType(Row, Column + 1, RoomType.PitRoom)) Console.WriteLine("You feel a draft. There is a pit nearby.");
            if (_map.IsOnMap(Row - 1, Column) && _map.CheckRoomType(Row - 1, Column, RoomType.Maelstrom)) Console.WriteLine("You hear a howling wind. There is a maelstrom nearby.");
            if (_map.IsOnMap(Row + 1, Column) && _map.CheckRoomType(Row + 1, Column, RoomType.Maelstrom)) Console.WriteLine("You hear a howling wind. There is a maelstrom nearby.");
            if (_map.IsOnMap(Row, Column - 1) && _map.CheckRoomType(Row, Column - 1, RoomType.Maelstrom)) Console.WriteLine("You hear a howling wind. There is a maelstrom nearby.");
            if (_map.IsOnMap(Row, Column + 1) && _map.CheckRoomType(Row, Column + 1, RoomType.Maelstrom)) Console.WriteLine("You hear a howling wind. There is a maelstrom nearby.");
            if (_map.IsOnMap(Row - 1, Column) && _map.CheckRoomType(Row - 1, Column, RoomType.Amarok)) Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");
            if (_map.IsOnMap(Row + 1, Column) && _map.CheckRoomType(Row + 1, Column, RoomType.Amarok)) Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");
            if (_map.IsOnMap(Row, Column - 1) && _map.CheckRoomType(Row, Column - 1, RoomType.Amarok)) Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");
            if (_map.IsOnMap(Row, Column + 1) && _map.CheckRoomType(Row, Column + 1, RoomType.Amarok)) Console.WriteLine("You can smell the rotten stench of an amarok in a nearby room.");

        }
    }
}
public enum RoomType { Empty, PitRoom, Fountain, Maelstrom, Entrance, Amarok }
