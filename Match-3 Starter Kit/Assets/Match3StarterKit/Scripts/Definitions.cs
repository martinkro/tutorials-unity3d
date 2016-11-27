using UnityEngine;

public enum PieceColour
{
    Blue = 0,
    Green,
    Orange,
    Purple,
    Red,
    Yellow,
    Special
}

public struct GridPoint
{
    public int x;
    public int y;

    public static bool operator ==(GridPoint a, GridPoint b)
    {
        return (a.x == b.x) && (a.y == b.y);
    }

    public static bool operator !=(GridPoint a, GridPoint b)
    {
        return (a.x != b.x) || (a.y != b.y);
    }

    public override bool Equals(object obj)
    {
        GridPoint tmp = (GridPoint)obj;
        if (this == tmp)
            return true;
        else
            return false;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public static int operator -(GridPoint a, GridPoint b) // not a real subtraction, just there to speed up things
    {
        if (a.x == b.x)
            return a.y - b.y;
        else if (a.y == b.y)
            return a.x - b.x;
        else
            return 100; // Just to make sure we don't process the move
    }
}

internal class PlayingPiece
{
    internal bool Active = false;
    internal PieceColour Type = PieceColour.Yellow;
    internal bool SpecialPiece = false;

    private bool selected = false;
    private GameObject piece = null;
    private PieceScript ps = null;

    internal GameObject Piece
    {
        get
        {
            return piece;
        }
        set
        {
            piece = value;
            if (piece != null)
                ps = piece.GetComponent<PieceScript>();
        }
    }

    internal bool Moving
    {
        get
        {
            if (ps != null)
                return ps.Moving;
            else
                return false;
        }
    }


    public PieceScript pieceScript
    {
        get
        {
            return ps;
        }
    }
    internal bool Selected
    {
        get
        {
            return selected;
        }
        set
        {
            if (piece != null)
            {
                if (ps.Moving)
                    selected = false;
                else
                    selected = value;
            }
        }
    }


    internal PlayingPiece(GameObject obj, PieceColour planet)
    {
        Piece = obj;
        Type = planet;
    }
}

public enum TileType
{
    NoTile = -1,
    Done = 0,
    TodoNormal = 1,
    TodoStrong = 2,
    TodoExtraStrong = 3,
    TodoSuperStrong = 4,
    BlockedTile = 5
}


internal class BoardTile
{
    internal GameObject Tile = null;
    internal TileType Type = TileType.TodoNormal;

    public BoardTile(GameObject tile, TileType type)
    {
        Tile = tile;
        Type = type;
    }
}

public enum GameStyle
{
    Standard = 0,
    Marinas
}

/// <summary>
/// This is the player's data class. Feel free to change it as it suits you better
/// </summary>
[System.Serializable]
internal class gameState
{
    internal string PlayerName { get; set; } // This is the key used in the dictionary
    internal int CurrentLevel { get; set; }
    internal int CurrentStage { get; set; }
    internal long TotalScore { get; set; }
    internal bool gotAchievement_1 { get; set; }
    internal bool gotAchievement_2 { get; set; }
    internal bool gotAchievement_3 { get; set; }
    internal bool gotAchievement_4 { get; set; }
    internal bool gotAchievement_5 { get; set; }
    internal bool gotAchievement_6 { get; set; }
    internal bool gotAchievement_7 { get; set; }
    internal bool gotAchievement_8 { get; set; }
    internal bool gotAchievement_9 { get; set; }
    internal bool gotAchievement_10 { get; set; }
}

/// <summary>
/// This is the class that will be actually serialized.
/// It's indexed on the player's name
/// </summary>
[System.Serializable]
internal class ScoreTable : System.Collections.Generic.Dictionary<string, gameState>
{
}
