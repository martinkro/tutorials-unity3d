using UnityEngine;

public static class GridPositions
{
    private static Vector2[,] positions = new Vector2[10, 10];

    static GridPositions()
    {
        initAll();
    }

    public static Vector2 GetVector(int x, int y)
    {
        return positions[x, y];
    }

    public static void Init()
    {
        initAll();
    }

    public static GridPoint GetGridPosition(Vector2 worldCoords)
    {
        GridPoint gp = new GridPoint();
        gp.x = -1;
        gp.y = -1;
        for (int y = 0; y < Board.Instance.rows; y++)
        {
            for (int x = 0; x < Board.Instance.columns; x++)
            {
                if (Board.gdesc[x, y] != (int)TileType.NoTile && Board.gdesc[x, y] != (int)TileType.BlockedTile && Mathf.Abs(Vector2.Distance(worldCoords, positions[x, y])) <= Board.halfStep)
                {
                    gp.x = x;
                    gp.y = y;
                    return gp;
                }
            }
        }
        return gp;
    }

    public static void SetPosition(int x, int y, float xp, float yp)
    {
        positions[x, y] = new Vector2(xp, yp);
    }

    private static void initAll()
    {
        positions = new Vector2[Board.Instance.columns, Board.Instance.rows];
        for (int y = 0; y < Board.Instance.rows; y++)
        {
            for (int x = 0; x < Board.Instance.columns; x++)
            {
                positions[x, y] = new Vector2(0, 0);
            }
        }
    }
}
