using System.Collections.Generic;
using UnityEngine;

public delegate void TimedOutDelegate();
public delegate void LevelClearedDelegate();
public delegate void GamePausedDelegate();

public class Board : MonoBehaviour
{
    // Constants
    private const string STR_DataBoard = "Data/board";
    private const string STR_FailedToLoadTheBoardFile = "Failed to load the board file!";
    private const string STR_dot = ".";

    // implemented delegates
    public GamePausedDelegate GamePausedMethod;
    public LevelClearedDelegate LevelClearedMethod;
    public TimedOutDelegate TimedOutMethod;

    // Public members
    public GUIText BoardPoints;
    public float levelTime = 10f;
    public float hintTime = 40f;
    public GameObject hintEffect;
    public AudioClip hintSound;
    public int PointsNormal = 5;
    public int PointsStrong = 10;
    public int PointsExtraStrong = 15;
    public int PointsSuperStrong = 20;
    public GameObject timeBarMesh;
    /// <summary>
    /// Empty GameObject marking the left edge of the active part of the board
    /// </summary>
    public GameObject leftMark;
    /// <summary>
    /// Empty GameObject marking the right edge of the active part of the board
    /// </summary>
    public GameObject rightMark;
    public bool isMatch4 = false;
    public bool newPiecesFromTop = false;
    public GameStyle gameStyle = GameStyle.Marinas;
    public GameObject tile;
    public float zTilePosition = 0.0f;
    public float zPiecePosition = 0.0f;
    public Material tileTodoNormalMaterial;
    public Material tileDoneMaterial;
    public Material tileBlockedMaterial;
    public ParticleEmitter activeEffect;
    /// <summary>
    /// Z axis position for the Active piece effect
    /// </summary>
    public float zActiveEffectPosition = 0.0f;
    public GameObject pieceDestroyedEffect;
    public GameObject specialPiece;
    public List<GameObject> PiecesNormal;
    public List<GameObject> PiecesStrong;
    public List<GameObject> PiecesExtraStrong;
    public List<GameObject> PiecesSuperStrong;
    public int maxPieces = 10;
    public AudioClip newPiece;
    public AudioClip destroyPiece;
    public AudioClip SlidePiece;
    public int boardNumber = 1;
    public int rows = 10;
    public int columns = 10;
    public bool FillOnX = false;
    public bool CentreOnX = false;

    private float hintTimer = 0f;
    private static List<GridPoint> _Lgp = new List<GridPoint>();
    private int totalBlockedTiles = 0;
    private Vector2 _VPosition;
    private TileType _Strenght;
    private Vector2 _CurrentPosition;
    private bool _MovingPieces;
    private Vector2 _Ap;
    private static float gameTimer = 0f;
    internal static float step = 0.0f;
    internal static float halfStep = 0.0f;
    private static Vector3 startPosition = new Vector3(0f, 0f, 0f);
    internal static PlayingPiece[,] PlayingPieces = new PlayingPiece[10, 10];
    private static BoardTile[,] grid = new BoardTile[10, 10];
    internal static int[,] gdesc = new int[10, 10];
    private static int[,] intGrid = new int[10, 10];
    internal static bool CleanSlate = false;
    internal static bool TimeOut = false;
    internal static bool GamePaused = false;
    private static ParticleEmitter activeMarker = null;
    private static List<GameObject> piecesToUseNormal = new List<GameObject>();
    private static List<GameObject> piecesToUseStrong = new List<GameObject>();
    private static List<GameObject> piecesToUseExtraStrong = new List<GameObject>();
    private static List<GameObject> piecesToUseSuperStrong = new List<GameObject>();
    private static bool destroyed = false;
    private static bool checking = false;
    private static float checkTimer = 0.0f;
    private static float checkMovesTimer = 0.0f;
    private static Vector3 generalScale = new Vector3(1f, 1f, 1f);

    public static GridPoint ActivePiece = new GridPoint();
    public static bool PlayerCanMove = true;

    private static Board instance = null;
    private static Touch touch;
    internal static bool tap = false;
    internal static Vector2 tapPosition = Vector2.zero;
    private bool started = false;

    public static Board Instance
    {
        get
        {
            return instance;
        }
    }

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
            return;
        }
        instance = this;
        StartBoard();
    }

    internal void StartBoard()
    {
        PlayingPieces = new PlayingPiece[columns, rows];
        grid = new BoardTile[columns, rows];
        gdesc = new int[columns, rows];
        intGrid = new int[columns, rows];
        ActivePiece.x = -1;
        ActivePiece.y = -1;

        GetPiecesToUse();
        float val = Mathf.Max((float)(columns), (float)(rows));
        if (!FillOnX)
        {
            step = (Mathf.Abs(leftMark.transform.position.x) + Mathf.Abs(rightMark.transform.position.x)) / val;
            halfStep = step / 2.0f;
            startPosition = new Vector3(leftMark.transform.position.x + halfStep, leftMark.transform.position.y - halfStep, zTilePosition);
        }
        else
        {
            Vector3 left = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, Camera.main.transform.position.z));
            Vector3 right = Camera.main.ScreenToWorldPoint(new Vector3(Mathf.Min((float)Screen.width, (float)Screen.height), 0f, Camera.main.transform.position.z));
            step = (Mathf.Abs(left.x) + Mathf.Abs(right.x)) / val;
            halfStep = step / 2.0f;
            startPosition = new Vector3(-(Mathf.Abs(left.x)) + halfStep, left.y - halfStep, zTilePosition);
        }
        float tilesScale = ((step * val) / (tile.GetComponent<Renderer>().bounds.size.x * val));
        if (CentreOnX)
        {
            Vector3 realRight = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width, 0f, Camera.main.transform.position.z));
            startPosition.x += Mathf.Abs(Mathf.Abs(realRight.x * 2f) - (step * columns)) / 2f;
        }
        generalScale = new Vector3(tilesScale, tilesScale, 1f);
        specialPiece.transform.localScale = generalScale;
        activeMarker = Instantiate(activeEffect, new Vector3(0, 0, zActiveEffectPosition), Quaternion.identity) as ParticleEmitter;
        activeMarker.transform.localScale = generalScale;
        activeMarker.emit = false;
        GridPositions.Init();

        TextAsset TXTFile = (TextAsset)Resources.Load(STR_DataBoard + boardNumber.ToString() + STR_dot + rows.ToString() + STR_dot + columns.ToString());
        if (TXTFile == null)
        {
            Debug.LogError(STR_FailedToLoadTheBoardFile);
        }
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                gdesc[x, y] = new int();
                if (!int.TryParse((TXTFile.text[x + y * columns].ToString()), out gdesc[x, y]))
                    gdesc[x, y] = (int)TileType.NoTile;
            }
        }

        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                float xp, yp = 0f;
                if (gdesc[x, y] != (int)TileType.NoTile)
                {
                    xp = startPosition.x + (x * step);
                    yp = startPosition.y - (y * step);
                    GridPositions.SetPosition(x, y, xp, yp);
                    grid[x, y] = new BoardTile(Instantiate(tile, new Vector3(xp, yp, zTilePosition), Quaternion.identity) as GameObject, (TileType)gdesc[x, y]);
                    grid[x, y].Tile.transform.localScale = generalScale;
                    if (gdesc[x, y] == (int)TileType.BlockedTile)
                    {
                        grid[x, y].Tile.GetComponent<Renderer>().material = tileBlockedMaterial;
                        totalBlockedTiles++;
                    }
                    else
                        grid[x, y].Tile.GetComponent<Renderer>().material = tileTodoNormalMaterial;
                    if (gdesc[x, y] != (int)TileType.BlockedTile)
                    {
                        bool again = false;
                        do
                        {
                            int t = Random.Range(0, maxPieces);
                            switch (gdesc[x, y])
                            {
                                case (int)TileType.TodoNormal:
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColour)t);
                                    break;
                                case (int)TileType.TodoStrong:
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseStrong[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColour)t);
                                    break;
                                case (int)TileType.TodoExtraStrong:
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseExtraStrong[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColour)t);
                                    break;
                                case (int)TileType.TodoSuperStrong:
                                    PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseSuperStrong[t], new Vector3(xp, yp, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColour)t);
                                    break;
                            }
                            if (CheckTileMatchX(x, y, true) || CheckTileMatchY(x, y, true))
                            {
                                DestroyImmediate(PlayingPieces[x, y].Piece);
                                PlayingPieces[x, y] = null;
                                again = true;
                            }
                            else
                                again = false;
                        } while (again);
                        PlayingPieces[x, y].pieceScript.currentStrenght = (TileType)gdesc[x, y];
                        PlayingPieces[x, y].pieceScript.MoveTo(x, y, zPiecePosition);
                        PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                    }
                }
            }
        }
        started = true;
    }

    private void GetPiecesToUse()
    {
        piecesToUseNormal.Clear();
        piecesToUseStrong.Clear();
        piecesToUseExtraStrong.Clear();
        piecesToUseSuperStrong.Clear();
        for (int i = 0; i < maxPieces; i++)
        {
            bool redo = true;
            int t = 0;
            do
            {
                t = Random.Range(0, PiecesNormal.Count);
                GameObject tmp = PiecesNormal[t];
                if (!piecesToUseNormal.Contains(tmp))
                {
                    piecesToUseNormal.Add(tmp);
                    redo = false;
                }
            } while (redo);
            if (t < PiecesStrong.Count)
                piecesToUseStrong.Add(PiecesStrong[t]);
            if (t < PiecesExtraStrong.Count)
                piecesToUseExtraStrong.Add(PiecesExtraStrong[t]);
            if (t < PiecesSuperStrong.Count)
                piecesToUseSuperStrong.Add(PiecesSuperStrong[t]);
        }
    }

    void FillIntGrid()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                intGrid[x, y] = new int();
                if (PlayingPieces[x, y] != null)
                    intGrid[x, y] = (int)PlayingPieces[x, y].Type;
                else
                    intGrid[x, y] = -1;
            }
        }
    }

    private void ActivateHint(int x1, int y1, int x2, int y2)
    {
        hintTimer = 0;
        if (hintEffect != null)
        {
            Instantiate(hintEffect, PlayingPieces[x1, y1].Piece.transform.position, Quaternion.identity);
            Instantiate(hintEffect, PlayingPieces[x2, y2].Piece.transform.position, Quaternion.identity);
            if (hintSound != null)
                GetComponent<AudioSource>().PlayOneShot(hintSound);
        }
    }
    internal bool CheckTileMatchX(int x, int y, bool justCheck)
    {
        if (!isMatch4)
            return CheckTileMatchX(x, y, x, y, justCheck);
        else
            return CheckTileMatchX4(x, y, x, y, justCheck);
    }

    internal bool CheckTileMatchX(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;

        if ((gdesc[x2, y2] != (int)TileType.NoTile) &&
            (gdesc[x2, y2] != (int)TileType.BlockedTile) &&
            (gdesc[x1, y1] != (int)TileType.BlockedTile) &&
            (gdesc[x2, y2] != (int)TileType.TodoStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoSuperStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoSuperStrong) &&
            PlayingPieces[x2, y2] != null &&
            PlayingPieces[x2, y2].Piece != null)
        {
            if (((x2 + 2) < columns) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 + 2, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 + 2, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 + 1, y2].Moving &&
                    !PlayingPieces[x2 + 2, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 + 2, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((x2 + 1) < columns) && ((x2 - 1) >= 0) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 + 1, y2].Moving &&
                    !PlayingPieces[x2 - 1, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((x2 - 2) >= 0) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (intGrid[x2 - 2, y2] != -1) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 2, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 - 1, y2].Moving &&
                    !PlayingPieces[x2 - 2, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    PlayingPieces[x2 - 2, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }
        }

        return match;
    }

    internal bool CheckTileMatchX4(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;

        if ((gdesc[x2, y2] != (int)TileType.NoTile) &&
            (gdesc[x2, y2] != (int)TileType.BlockedTile) &&
            (gdesc[x1, y1] != (int)TileType.BlockedTile) &&
            (gdesc[x2, y2] != (int)TileType.TodoStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoSuperStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoSuperStrong) &&
            PlayingPieces[x2, y2] != null &&
            PlayingPieces[x2, y2].Piece != null)
        {
            if (((x2 + 3) < columns) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 + 2, y2] != -1) &&
                (intGrid[x2 + 3, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 3, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 + 3, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 + 2, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 + 3, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 + 1, y2].Moving &&
                    !PlayingPieces[x2 + 2, y2].Moving &&
                    !PlayingPieces[x2 + 3, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 + 2, y2].Selected = true;
                    PlayingPieces[x2 + 3, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((x2 + 2) < columns) && ((x2 - 1) >= 0) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 + 2, y2] != -1) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 + 2, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 + 2, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 + 1, y2].Moving &&
                    !PlayingPieces[x2 + 2, y2].Moving &&
                    !PlayingPieces[x2 - 1, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 + 2, y2].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((x2 + 1) < columns) && ((x2 - 2) >= 0) &&
                (intGrid[x2 + 1, y2] != -1) &&
                (intGrid[x2 - 2, y2] != -1) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (gdesc[x2 + 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 + 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 + 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 2, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 + 1, y2].Moving &&
                    !PlayingPieces[x2 - 2, y2].Moving &&
                    !PlayingPieces[x2 - 1, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 + 1, y2].Selected = true;
                    PlayingPieces[x2 - 2, y2].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((x2 - 3) >= 0) &&
                (intGrid[x2 - 1, y2] != -1) &&
                (intGrid[x2 - 2, y2] != -1) &&
                (intGrid[x2 - 3, y2] != -1) &&
                (gdesc[x2 - 1, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 3, y2] != (int)TileType.NoTile) &&
                (gdesc[x2 - 1, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 2, y2] != (int)TileType.BlockedTile) &&
                (gdesc[x2 - 3, y2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2 - 1, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 2, y2]) &&
                (intGrid[x2, y2] == intGrid[x2 - 3, y2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2 - 1, y2].Moving &&
                    !PlayingPieces[x2 - 2, y2].Moving &&
                    !PlayingPieces[x2 - 3, y2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2 - 1, y2].Selected = true;
                    PlayingPieces[x2 - 2, y2].Selected = true;
                    PlayingPieces[x2 - 3, y2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }
        }

        return match;
    }

    internal bool CheckTileMatchY(int x, int y, bool justCheck)
    {
        if (!isMatch4)
            return CheckTileMatchY(x, y, x, y, justCheck);
        else
            return CheckTileMatchY4(x, y, x, y, justCheck);
    }

    internal bool CheckTileMatchY(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;

        if ((gdesc[x2, y2] != (int)TileType.NoTile) &&
            (gdesc[x2, y2] != (int)TileType.BlockedTile) &&
            (gdesc[x1, y1] != (int)TileType.BlockedTile) &&
            (gdesc[x2, y2] != (int)TileType.TodoStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoSuperStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoSuperStrong) &&
            PlayingPieces[x2, y2] != null &&
            PlayingPieces[x2, y2].Piece != null)
        {
            if (((y2 + 2) < rows) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 + 2] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 + 1].Moving &&
                    !PlayingPieces[x2, y2 + 2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 + 2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((y2 + 1) < rows) && ((y2 - 1) >= 0) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 + 1].Moving &&
                    !PlayingPieces[x2, y2 - 1].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((y2 - 2) >= 0) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (intGrid[x2, y2 - 2] != -1) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 2]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 - 1].Moving &&
                    !PlayingPieces[x2, y2 - 2].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    PlayingPieces[x2, y2 - 2].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }
        }

        return match;
    }

    internal bool CheckTileMatchY4(int x1, int y1, int x2, int y2, bool justCheck)
    {
        bool match = false;
        if (x1 < 0 || x1 > columns - 1 || y1 < 0 || y1 > rows - 1 || x2 < 0 || x2 > columns - 1 || y2 < 0 || y2 > rows - 1)
            return false;
        FillIntGrid();
        if (intGrid[x1, y1] == -1 || intGrid[x2, y2] == -1)
            return false;
        int z = intGrid[x1, y1];
        intGrid[x1, y1] = intGrid[x2, y2];
        intGrid[x2, y2] = z;

        if ((gdesc[x2, y2] != (int)TileType.NoTile) &&
            (gdesc[x2, y2] != (int)TileType.BlockedTile) &&
            (gdesc[x1, y1] != (int)TileType.BlockedTile) &&
            (gdesc[x2, y2] != (int)TileType.TodoStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoExtraStrong) &&
            (gdesc[x2, y2] != (int)TileType.TodoSuperStrong) &&
            (gdesc[x1, y1] != (int)TileType.TodoSuperStrong) &&
            PlayingPieces[x2, y2] != null &&
            PlayingPieces[x2, y2].Piece != null)
        {
            if (((y2 + 3) < rows) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 + 2] != -1) &&
                (intGrid[x2, y2 + 3] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 3] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 + 3] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 2]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 3]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 + 1].Moving &&
                    !PlayingPieces[x2, y2 + 2].Moving &&
                    !PlayingPieces[x2, y2 + 3].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 + 2].Selected = true;
                    PlayingPieces[x2, y2 + 3].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((y2 + 2) < rows) && ((y2 - 1) >= 0) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 + 2] != -1) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 + 2] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 2]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 + 1].Moving &&
                    !PlayingPieces[x2, y2 + 2].Moving &&
                    !PlayingPieces[x2, y2 - 1].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 + 2].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((y2 + 1) < rows) && ((y2 - 2) >= 0) &&
                (intGrid[x2, y2 + 1] != -1) &&
                (intGrid[x2, y2 - 2] != -1) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (gdesc[x2, y2 + 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 + 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 + 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 2]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 + 1].Moving &&
                    !PlayingPieces[x2, y2 - 2].Moving &&
                    !PlayingPieces[x2, y2 - 1].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 + 1].Selected = true;
                    PlayingPieces[x2, y2 - 2].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }

            if (((y2 - 3) >= 0) &&
                (intGrid[x2, y2 - 1] != -1) &&
                (intGrid[x2, y2 - 2] != -1) &&
                (intGrid[x2, y2 - 3] != -1) &&
                (gdesc[x2, y2 - 1] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 3] != (int)TileType.NoTile) &&
                (gdesc[x2, y2 - 1] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 2] != (int)TileType.BlockedTile) &&
                (gdesc[x2, y2 - 3] != (int)TileType.BlockedTile) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 1]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 2]) &&
                (intGrid[x2, y2] == intGrid[x2, y2 - 3]))
            {
                if (!justCheck &&
                    !PlayingPieces[x1, y1].Moving &&
                    !PlayingPieces[x2, y2 - 1].Moving &&
                    !PlayingPieces[x2, y2 - 2].Moving &&
                    !PlayingPieces[x2, y2 - 3].Moving)
                {
                    PlayingPieces[x1, y1].Selected = true;
                    PlayingPieces[x2, y2 - 1].Selected = true;
                    PlayingPieces[x2, y2 - 2].Selected = true;
                    PlayingPieces[x2, y2 - 3].Selected = true;
                    match = true;
                }
                else
                {
                    if (hintTimer >= hintTime)
                    {
                        ActivateHint(x1, y1, x2, y2);
                    }
                    return true;
                }
            }
        }

        return match;
    }

    void OnGUI()
    {
        BoardPoints.text = ScoresManager.CurrentPoints.ToString();
        if (GamePaused && GamePausedMethod != null)
            GamePausedMethod();
        if (TimeOut && TimedOutMethod != null)
            TimedOutMethod();
        if (CleanSlate && LevelClearedMethod != null)
            LevelClearedMethod();
    }

    // Update is called once per frame
    void Update()
    {

        if (TimeOut || CleanSlate || GamePaused)
            return;
#if UNITY_IPHONE || UNITY_ANDROID
        tap = false;
        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            tap = touch.tapCount > 0 ? true : false;
            if (tap)
            {
                tapPosition = touch.position;
                Vector3 wCoord = Camera.main.ScreenToWorldPoint(new Vector3(tapPosition.x, tapPosition.y, Camera.main.transform.position.z)) * -1f;
                GridPoint mPos = GridPositions.GetGridPosition(new Vector2(wCoord.x, wCoord.y));
                if (mPos.x != -1)
                {
                    PlayingPieces[mPos.x, mPos.y].pieceScript.Clicked();
                }
            }
        }

        if (Input.GetKey(KeyCode.Escape) || Input.GetKey(KeyCode.Home) || Input.GetKey(KeyCode.Menu))
            Application.Quit();
#endif
        gameTimer += Time.deltaTime;
        hintTimer += Time.deltaTime;
        checkTimer += Time.deltaTime;
        checkMovesTimer += Time.deltaTime;

        float remainingTimeScaled = ((levelTime * 60f) - gameTimer) / (levelTime * 60f);
        if (remainingTimeScaled <= 0f)
        {
            TimeOut = true;
            return;
        }
        timeBarMesh.transform.localScale = new Vector3(remainingTimeScaled, timeBarMesh.transform.localScale.y, timeBarMesh.transform.localScale.z);
        if (ActivePiece.x != -1)
        {
            _Ap = GridPositions.GetVector(ActivePiece.x, ActivePiece.y);
            activeMarker.transform.position = new Vector3(_Ap.x, _Ap.y, zActiveEffectPosition);
            activeMarker.emit = true;
        }
        else
            activeMarker.emit = false;
        if (started && !checking && checkTimer >= 0.2f && !CleanSlate)
        {
            checking = true;
            checkTimer = 0.0f;
            _MovingPieces = false;

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < columns; x++)
                {
                    if (PlayingPieces[x, y] != null && PlayingPieces[x, y].Piece != null && PlayingPieces[x, y].Moving)
                    {
                        _MovingPieces = true;
                        y = rows;
                        break;
                    }
                }
            }

            if (destroyed && !_MovingPieces)
            {
                PlayerCanMove = false;
                destroyed = false;
                for (int x = 0; x < columns; x++)
                {
                    SlideDown(x, rows - 1);
                }
                NewPieces();
                hintTimer = 0f;
                PlayerCanMove = true;
            }
            else
            {
                if (!_MovingPieces)
                {
                    if (checkMovesTimer > 2f)
                    {
                        if (IsLevelClear())
                        {
                            CleanSlate = true;
                            return;
                        }
                        checkMovesTimer = 0f;
                        CheckMoves();
                    }
                }
            }


            if (!_MovingPieces && !destroyed)
            {
                PlayerCanMove = false;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        CheckTileMatchX(x, y, false);
                        CheckTileMatchY(x, y, false);
                    }
                }
                int specialCount = 0;
                for (int y = 0; y < rows; y++)
                {
                    for (int x = 0; x < columns; x++)
                    {
                        if (PlayingPieces[x, y] != null && PlayingPieces[x, y].Selected && PlayingPieces[x, y].Piece != null)
                        {
                            if (PlayingPieces[x, y].SpecialPiece)
                                specialCount++;
                            if (specialCount == 3)
                            {
                                specialCount = 0;
                                for (int a = 0; a < columns; a++)
                                {
                                    for (int b = 0; b < rows; b++)
                                    {
                                        if (gdesc[a, b] == (int)TileType.BlockedTile)
                                        {
                                            grid[a, b].Tile.GetComponent<Renderer>().material = tileTodoNormalMaterial;
                                            grid[a, b].Type = TileType.TodoNormal;
                                            totalBlockedTiles--;
                                            gdesc[a, b] = (int)TileType.TodoNormal;
                                            _VPosition = GridPositions.GetVector(a, b);
                                            GameObject tmpEffect = Instantiate(pieceDestroyedEffect, new Vector3(_VPosition.x, _VPosition.y, zTilePosition), Quaternion.identity) as GameObject;
                                            tmpEffect.transform.localScale = generalScale;
                                            a = rows;
                                            b = columns;
                                        }
                                    }
                                }
                            }
                            GameObject tmpEffect0 = Instantiate(pieceDestroyedEffect, PlayingPieces[x, y].pieceScript.MyTransform.position, Quaternion.identity) as GameObject;
                            tmpEffect0.transform.localScale = generalScale;
                            _CurrentPosition = GridPositions.GetVector(x, y);
                            switch (PlayingPieces[x, y].pieceScript.currentStrenght)
                            {
                                case TileType.TodoNormal: // a normal piece, just destroy it
                                    Destroy(PlayingPieces[x, y].Piece);
                                    PlayingPieces[x, y].Piece = null;
                                    PlayingPieces[x, y].Selected = false;
                                    PlayingPieces[x, y] = null;
                                    gdesc[x, y] = (int)TileType.Done;
                                    grid[x, y].Tile.GetComponent<Renderer>().material = tileDoneMaterial;
                                    ScoresManager.AddPoints(PointsNormal);
                                    break;
                                case TileType.TodoStrong:
                                    Destroy(PlayingPieces[x, y].Piece);
                                    PlayingPieces[x, y].Piece = Instantiate(piecesToUseNormal[(int)PlayingPieces[x, y].Type], new Vector3(_CurrentPosition.x, _CurrentPosition.y, zPiecePosition), Quaternion.identity) as GameObject;
                                    PlayingPieces[x, y].pieceScript.currentStrenght = TileType.TodoNormal;
                                    PlayingPieces[x, y].Selected = false;
                                    PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                                    gdesc[x, y] = (int)TileType.TodoNormal;
                                    grid[x, y].Type = TileType.TodoNormal;
                                    ScoresManager.AddPoints(PointsStrong);
                                    break;
                                case TileType.TodoExtraStrong:
                                    Destroy(PlayingPieces[x, y].Piece);
                                    PlayingPieces[x, y].Piece = Instantiate(piecesToUseStrong[(int)PlayingPieces[x, y].Type], new Vector3(_CurrentPosition.x, _CurrentPosition.y, zPiecePosition), Quaternion.identity) as GameObject;
                                    PlayingPieces[x, y].pieceScript.currentStrenght = TileType.TodoStrong;
                                    PlayingPieces[x, y].Selected = false;
                                    PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                                    gdesc[x, y] = (int)TileType.TodoStrong;
                                    grid[x, y].Type = TileType.TodoStrong;
                                    ScoresManager.AddPoints(PointsExtraStrong);
                                    break;
                                case TileType.TodoSuperStrong:
                                    Destroy(PlayingPieces[x, y].Piece);
                                    PlayingPieces[x, y].Piece = Instantiate(piecesToUseExtraStrong[(int)PlayingPieces[x, y].Type], new Vector3(_CurrentPosition.x, _CurrentPosition.y, zPiecePosition), Quaternion.identity) as GameObject;
                                    PlayingPieces[x, y].pieceScript.currentStrenght = TileType.TodoExtraStrong;
                                    PlayingPieces[x, y].Selected = false;
                                    PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                                    gdesc[x, y] = (int)TileType.TodoExtraStrong;
                                    grid[x, y].Type = TileType.TodoExtraStrong;
                                    ScoresManager.AddPoints(PointsSuperStrong);
                                    break;
                            }
                            GetComponent<AudioSource>().PlayOneShot(destroyPiece);
                            destroyed = true;
                        }
                    }
                }
                if (totalBlockedTiles == 0)
                {
                    for (int a = 0; a < columns; a++)
                    {
                        for (int b = 0; b < rows; b++)
                        {
                            if (PlayingPieces[a, b] != null && PlayingPieces[a, b].SpecialPiece)
                            {
                                Destroy(PlayingPieces[a, b].Piece);
                                PlayingPieces[a, b].Piece = null;
                                PlayingPieces[a, b].Selected = false;
                                PlayingPieces[a, b] = null;
                                destroyed = true;
                            }
                        }
                    }
                }
                PlayerCanMove = true;
            }
        }
        checking = false;
    }

    private bool IsLevelClear()
    {
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                if ((gdesc[x, y] != (int)TileType.NoTile) &&
                    (gdesc[x, y] != (int)TileType.Done))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void NewPieces()
    {
        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                if (gdesc[x, y] != (int)TileType.BlockedTile && gdesc[x, y] != (int)TileType.NoTile && PlayingPieces[x, y] == null)
                {
                    int chance = Random.Range(0, 7);
                    Vector2 v0 = GridPositions.GetVector(x, y);
                    if (totalBlockedTiles > 0 && chance == 3)
                    {
                        PlayingPieces[x, y] = new PlayingPiece(Instantiate(specialPiece, new Vector3(v0.x, v0.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, PieceColour.Special);
                        PlayingPieces[x, y].SpecialPiece = true;
                    }
                    else
                    {
                        bool again = false;
                        do
                        {
                            int t = Random.Range(0, maxPieces);
                            if (!newPiecesFromTop)
                                PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y, zPiecePosition - Random.Range(20f, 30f)), Quaternion.identity) as GameObject, (PieceColour)t);
                            else
                                PlayingPieces[x, y] = new PlayingPiece(Instantiate(piecesToUseNormal[t], new Vector3(v0.x, v0.y + Random.Range(20f, 30f), zPiecePosition), Quaternion.identity) as GameObject, (PieceColour)t);
                            if (CheckTileMatchX(x, y, true) || CheckTileMatchY(x, y, true))
                            {
                                DestroyImmediate(PlayingPieces[x, y].Piece);
                                PlayingPieces[x, y] = null;
                                again = true;
                            }
                            else
                                again = false;
                        } while (again);
                    }
                    GetComponent<AudioSource>().PlayOneShot(newPiece);
                    PlayingPieces[x, y].pieceScript.currentStrenght = TileType.TodoNormal;
                    PlayingPieces[x, y].pieceScript.MoveTo(x, y, zPiecePosition);
                    PlayingPieces[x, y].Selected = false;
                    PlayingPieces[x, y].Piece.transform.localScale = generalScale;
                }
                else if (gdesc[x, y] != (int)TileType.NoTile && PlayingPieces[x, y] != null && PlayingPieces[x, y].pieceScript.currentStrenght != TileType.TodoNormal)
                {
                    break;
                }
            }
        }
        if (ActivePiece.x != -1 && PlayingPieces[ActivePiece.x, ActivePiece.y] == null)
            ActivePiece.x = -1;
    }

    void CheckMoves()
    {
        if (gameStyle == GameStyle.Marinas && ActivePiece.x == -1)
            return;
        GridPoint gp = new GridPoint();
        if (gameStyle == GameStyle.Marinas)
        {
            for (gp.x = 0; gp.x < columns; gp.x++)
            {
                for (gp.y = 0; gp.y < rows; gp.y++)
                {
                    if (gp != ActivePiece)
                    {
                        if (!isMatch4)
                        {
                            if (CheckTileMatchX(ActivePiece.x, ActivePiece.y, gp.x, gp.y, true) ||
                                CheckTileMatchY(ActivePiece.x, ActivePiece.y, gp.x, gp.y, true) ||
                                CheckTileMatchX(gp.x, gp.y, ActivePiece.x, ActivePiece.y, true) ||
                                CheckTileMatchY(gp.x, gp.y, ActivePiece.x, ActivePiece.y, true))
                                return;
                        }
                        else
                        {
                            if (CheckTileMatchX4(ActivePiece.x, ActivePiece.y, gp.x, gp.y, true) ||
                                CheckTileMatchY4(ActivePiece.x, ActivePiece.y, gp.x, gp.y, true) ||
                                CheckTileMatchX4(gp.x, gp.y, ActivePiece.x, ActivePiece.y, true) ||
                                CheckTileMatchY4(gp.x, gp.y, ActivePiece.x, ActivePiece.y, true))
                                return;
                        }
                    }
                }
            }
        }
        else
        {
            for (gp.x = 0; gp.x < columns; gp.x++)
            {
                for (gp.y = 0; gp.y < rows; gp.y++)
                {
                    if (!isMatch4)
                    {
                        if (CheckTileMatchX(gp.x, gp.y, gp.x + 1, gp.y, true) ||
                            CheckTileMatchY(gp.x, gp.y, gp.x - 1, gp.y, true) ||
                            CheckTileMatchX(gp.x, gp.y, gp.x, gp.y + 1, true) ||
                            CheckTileMatchY(gp.x, gp.y, gp.x, gp.y - 1, true))
                            return;
                    }
                    else
                    {
                        if (CheckTileMatchX4(gp.x, gp.y, gp.x + 1, gp.y, true) ||
                            CheckTileMatchY4(gp.x, gp.y, gp.x - 1, gp.y, true) ||
                            CheckTileMatchX4(gp.x, gp.y, gp.x, gp.y + 1, true) ||
                            CheckTileMatchY4(gp.x, gp.y, gp.x, gp.y - 1, true))
                            return;
                    }
                }
            }
        }
        _Lgp.Clear();
        GridPoint gp1 = new GridPoint();
        for (int count = 0; count < ((rows * columns) / 4); count++)
        {
            gp.x = Random.Range(0, columns);
            gp.y = Random.Range(0, rows / 2);
            while ((gdesc[gp.x, gp.y] == (int)TileType.NoTile) ||
                   (PlayingPieces[gp.x, gp.y] == null) ||
                   (PlayingPieces[gp.x, gp.y].Piece == null) ||
                   _Lgp.Contains(gp) ||
                   PlayingPieces[gp.x, gp.y].pieceScript.currentStrenght != TileType.TodoNormal)
            {
                gp.x = Random.Range(0, columns);
                gp.y = Random.Range(0, rows / 2);
            }

            _Lgp.Add(gp);

            gp1.x = Random.Range(0, columns);
            gp1.y = Random.Range(5, rows / 2);
            while ((gdesc[gp1.x, gp1.y] == (int)TileType.NoTile) ||
                   (PlayingPieces[gp1.x, gp1.y] == null) ||
                   (PlayingPieces[gp1.x, gp1.y].Piece == null) ||
                   _Lgp.Contains(gp1) ||
                   PlayingPieces[gp1.x, gp1.y].pieceScript.currentStrenght != TileType.TodoNormal)
            {
                gp1.x = Random.Range(0, columns);
                gp1.y = Random.Range(rows / 2, rows);
            }
            _Lgp.Add(gp1);
            PlayingPieces[gp.x, gp.y].pieceScript.MoveTo(gp1.x, gp1.y);
            PlayingPieces[gp1.x, gp1.y].pieceScript.MoveTo(gp.x, gp.y);
            PlayingPiece tmp = PlayingPieces[gp.x, gp.y];
            PlayingPieces[gp.x, gp.y] = PlayingPieces[gp1.x, gp1.y];
            PlayingPieces[gp1.x, gp1.y] = tmp;
        }
        _Lgp.Clear();
    }

    private void SlideDown(int x, int y)
    {
        for (int y1 = y; y1 >= 1; y1--)
        {
            if (gdesc[x, y1] != (int)TileType.NoTile && gdesc[x, y1] != (int)TileType.BlockedTile && PlayingPieces[x, y1] == null)
            {
                int fy = -1;
                for (int z = y1 - 1; z >= 0; z--)
                {
                    if (gdesc[x, z] != (int)TileType.NoTile && gdesc[x, y1] != (int)TileType.BlockedTile && PlayingPieces[x, z] != null)
                    {
                        fy = z;
                        break;
                    }
                }
                if (fy != -1)
                {
                    if (PlayingPieces[x, fy].pieceScript.currentStrenght == TileType.TodoNormal)
                    {
                        PlayingPieces[x, fy].Selected = false;
                        PlayingPieces[x, fy].pieceScript.MoveTo(x, y1);
                        PlayingPieces[x, y1] = PlayingPieces[x, fy];
                        PlayingPieces[x, fy] = null;
                        GetComponent<AudioSource>().PlayOneShot(SlidePiece);
                        SlideDown(x, y1);
                        break;
                    }
                }
            }
        }
    }
}
