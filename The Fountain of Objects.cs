Game game = CreateGame();
game.Run();

static Game CreateGame()
{
    int total_rows = 4;
    int total_cols = 4;
    Map map = new Map(total_rows, total_cols);

    int start_row = 0;
    int start_column = 0;
    Location start = new Location(start_row, start_column);

    Player player = new Player(start);
    return new Game(map, player);
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
            player.Location.Current(this);
            if (!IsRunning) { break; }
            player.Location.Move(this);
        }
    }
    public void End() { IsRunning = false; }
    public void EnableFountain() { 
        FountainEnabled = true; 
        Console.WriteLine("You hear rushing waters from the Fountain of Objects. It has been reactivated!"); 
    }
    public bool HasEscaped() { 
        if (FountainEnabled && player.Location.Row == 0 && player.Location.Column == 0) { return true; }
        return false;
    }
}
public readonly struct Player(Location start)
{
    public Location Location { get; } = start;
}
public class Map
{    
    public RoomType[,]? Grid { get; private set; }
    public Map(int rows, int columns)
    {
        Grid = new RoomType[rows, columns];
        Grid[0, 0] = RoomType.Entrance;
        Grid[0, 2] = RoomType.Fountain;
    }
}
public class Location
{
    public static int[] Player { get; private set; } = [0, 0];
    public int[] Fountain { get; } = [0, 2];
    public int[] Entrance { get; } = [0, 0];
    public int Row { get; private set; } = Player[0];
    public int Column { get; private set; } = Player[1];
    public Location(int startRow, int startColumn)
    {
        Player = [startRow, startColumn];
    }

    public void Move(Game game)
    {
        Console.WriteLine("What do you want to do? ");
        string? response = Console.ReadLine();
        switch (response)
        {
            case "move north":
                if (IsOffMap(Row - 1)) Row--;
                break;
            case "move south":
                if (IsOffMap(Row + 1)) Row++;
                break;
            case "move east":
                if (IsOffMap(Column + 1)) Column++;
                break;
            case "move west":
                if (IsOffMap(Column - 1)) Column--;
                break;
            case "enable fountain":
                if (Row == Fountain[0] && Column == Fountain[1])
                {
                    game.EnableFountain();
                    Console.WriteLine("You have enabled the fountain!");
                }
                else
                {
                    Console.WriteLine("You are not at the fountain.");
                }
                break;
        }
        Player = [Row, Column];
        Console.WriteLine();
    }
    public void Current(Game game)
    {
        Console.WriteLine("-------------------------------------------------------");
        Console.WriteLine($"You are in the room at (Row={Row}, Column={Column})");
        if (game.FountainEnabled && Row == Entrance[0] && Column == Entrance[1])
        {
            Console.WriteLine("The Fountain of Objects has been reactivated, and you have escaped with your life!");
            Console.WriteLine("You win!");
            game.End();
        }
        else if (Row == Fountain[0] && Column == Fountain[1])
        {
            Console.WriteLine("You hear water dripping in this room. The Fountain of Objects is here!");
        }
        else if (Row == Entrance[0] && Column == Entrance[1])
        {
            Console.WriteLine("You see light coming from the cavern entrance.");
        }
    }
    public bool IsOffMap(int direction)
    {
        if (direction >= 0 && direction <= 3) { return true; }
        Console.WriteLine("Direction is out of bounds.");
        return false;
    }
}
public enum RoomType { Empty, Fountain, Entrance }