using UnityEngine;

public class PieceScript : MonoBehaviour
{
    private GridPoint _Gp;
    private GridPoint _Gp1;
    private const float FLOAT_DragDetection = 10f;
    public bool Active = false;
    public AudioClip moveSound;

    internal TileType currentStrenght = TileType.TodoNormal;
    private bool moving = false;
    bool mouseEnterAnimation = false;
    float counter = 0f;
    Vector3 yAxis = new Vector3(0, 1, 0);
    Vector3 destination = Vector3.zero;
    Vector3 velocity = Vector3.zero;
    Vector3 prevPoint = Vector3.zero;
    Vector3 currentPoint = Vector3.zero;
    bool mouseDown = false;
    bool mouseClick = false;
    private Transform myTransform;

    internal bool Moving
    {
        get
        {
            return moving;
        }
    }

    internal Transform MyTransform
    {
        get
        {
            return myTransform;
        }
    }
    // Use this for initialization
    void Start()
    {
        myTransform = transform;
    }

    public void MoveTo(int x, int y)
    {
        moving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, myTransform.position.z);
    }

    public void MoveTo(int x, int y, float z)
    {
        moving = true;
        destination = new Vector3(GridPositions.GetVector(x, y).x, GridPositions.GetVector(x, y).y, z);
    }

    public void Clicked()
    {
        if (!Board.PlayerCanMove ||
            currentStrenght == TileType.BlockedTile ||
            currentStrenght == TileType.TodoExtraStrong ||
            currentStrenght == TileType.TodoStrong ||
            currentStrenght == TileType.TodoSuperStrong)
        {
            Debug.Log("Current Strenght=" + currentStrenght);
            return;
        }
        _Gp1 = GridPositions.GetGridPosition(new Vector2(myTransform.position.x, myTransform.position.y));
        if (_Gp1.x != -1 && Board.ActivePiece.x == -1)
        {
            Board.ActivePiece = _Gp1;
        }
        else if (_Gp1.x != -1)
        {
            if (Board.Instance.gameStyle == GameStyle.Standard && Mathf.Abs(_Gp1 - Board.ActivePiece) > 1)
            {
                Board.ActivePiece = _Gp1;
                return;
            }

            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchX(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)) ||
                  (Board.Instance.CheckTileMatchY(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchY(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)))) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp1.x, _Gp1.y, Board.ActivePiece.x, Board.ActivePiece.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(Board.ActivePiece.x, Board.ActivePiece.y, _Gp1.x, _Gp1.y, true))))
                )
            {
                MoveTo(Board.ActivePiece.x, Board.ActivePiece.y);
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y].pieceScript.MoveTo(_Gp1.x, _Gp1.y);
                Board.PlayingPieces[_Gp1.x, _Gp1.y].Active = true;
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y].Active = false;
                PlayingPiece tmp = Board.PlayingPieces[_Gp1.x, _Gp1.y];
                Board.PlayingPieces[_Gp1.x, _Gp1.y] = Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y];
                Board.PlayingPieces[Board.ActivePiece.x, Board.ActivePiece.y] = tmp;
                Board.ActivePiece = _Gp1;
                Board.PlayerCanMove = false;
                GetComponent<AudioSource>().PlayOneShot(moveSound);
                if (Board.Instance.gameStyle == GameStyle.Standard)
                    Board.ActivePiece.x = -1;
            }
            else if (Board.Instance.gameStyle == GameStyle.Standard)
            {
                Board.ActivePiece = _Gp1;
            }

        }
    }

    float dragDelay = 0f;
    void OnMouseDrag()
    {
        if (
            (!Board.PlayerCanMove || (Board.Instance.gameStyle != GameStyle.Standard)) ||
            currentStrenght == TileType.BlockedTile ||
            currentStrenght == TileType.TodoExtraStrong ||
            currentStrenght == TileType.TodoStrong ||
            currentStrenght == TileType.TodoSuperStrong
            )
            return;
        dragDelay += Time.deltaTime;
        if (dragDelay < 0.2f)
            return;
        dragDelay = 0;
        prevPoint = currentPoint;
        currentPoint = Input.mousePosition;
        _Gp = GridPositions.GetGridPosition(new Vector2(myTransform.position.x, myTransform.position.y));
        Vector3 dir = currentPoint - prevPoint;
        if (dir.x < -FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x - 1, _Gp.y, true)) ||
                 (Board.Instance.CheckTileMatchY4(_Gp.x - 1, _Gp.y, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x - 1, _Gp.y);
                Board.PlayingPieces[_Gp.x - 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x - 1, _Gp.y];
                Board.PlayingPieces[_Gp.x - 1, _Gp.y] = tmp;
                Board.ActivePiece.x = -1;
                Board.PlayerCanMove = false;
                GetComponent<AudioSource>().PlayOneShot(moveSound);
                mouseClick = false;
                mouseDown = false;
                dragDelay = 0f;
                return;
            }
        }
        if (dir.x > FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x + 1, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x + 1, _Gp.y, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x + 1, _Gp.y);
                Board.PlayingPieces[_Gp.x + 1, _Gp.y].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x + 1, _Gp.y];
                Board.PlayingPieces[_Gp.x + 1, _Gp.y] = tmp;
                Board.ActivePiece.x = -1;
                Board.PlayerCanMove = false;
                GetComponent<AudioSource>().PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
        if (dir.y > FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x, _Gp.y - 1, true)) ||
                  (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y - 1, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x, _Gp.y - 1);
                Board.PlayingPieces[_Gp.x, _Gp.y - 1].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y - 1];
                Board.PlayingPieces[_Gp.x, _Gp.y - 1] = tmp;
                Board.ActivePiece.x = -1;
                Board.PlayerCanMove = false;
                GetComponent<AudioSource>().PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
        if (dir.y < -FLOAT_DragDetection)
        {
            if (
                (!Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                  (Board.Instance.CheckTileMatchX(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                (Board.Instance.CheckTileMatchY(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true))
                  )) ||
                (Board.Instance.isMatch4 &&
                 ((Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                  (Board.Instance.CheckTileMatchX4(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y, _Gp.x, _Gp.y + 1, true)) ||
                (Board.Instance.CheckTileMatchY4(_Gp.x, _Gp.y + 1, _Gp.x, _Gp.y, true))
                  ))
                )
            {
                MoveTo(_Gp.x, _Gp.y + 1);
                Board.PlayingPieces[_Gp.x, _Gp.y + 1].pieceScript.MoveTo(_Gp.x, _Gp.y);
                PlayingPiece tmp = Board.PlayingPieces[_Gp.x, _Gp.y];
                Board.PlayingPieces[_Gp.x, _Gp.y] = Board.PlayingPieces[_Gp.x, _Gp.y + 1];
                Board.PlayingPieces[_Gp.x, _Gp.y + 1] = tmp;
                Board.ActivePiece.x = -1;
                Board.PlayerCanMove = false;
                GetComponent<AudioSource>().PlayOneShot(moveSound);
                dragDelay = 0f;
                mouseClick = false;
                mouseDown = false;
                return;
            }
        }
    }

    void OnMouseDown()
    {
        currentPoint = Input.mousePosition;
        dragDelay = 0;
        mouseDown = true;
    }

    void OnMouseUp()
    {
        if (mouseDown)
        {
            mouseClick = true;
            mouseDown = false;
        }
    }

    void OnMouseEnter()
    {
        mouseDown = false;
        mouseClick = false;
        // Only play the animation for the normal piece
        if (currentStrenght == TileType.TodoNormal)
            mouseEnterAnimation = true;
    }

    void OnMouseExit()
    {
        mouseDown = false;
        mouseClick = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (mouseClick && !moving)
        {
            mouseClick = false;
            Clicked();
        }
        if (mouseEnterAnimation)
        {
            counter += Time.deltaTime;
            if (counter <= .6f)
            {
                myTransform.Rotate(yAxis, 500f * Time.deltaTime);
            }
            else
            {
                myTransform.rotation = Quaternion.identity;
                mouseEnterAnimation = false;
                counter = 0f;
            }
        }
        if (moving)
        {
            velocity = destination - myTransform.position;
            float damping = velocity.magnitude;
            if (velocity.sqrMagnitude < 0.5f)
            {
                moving = false;
                myTransform.position = destination;
            }
            else
            {
                velocity.Normalize();
                myTransform.position = (myTransform.position + (velocity * Time.deltaTime * (damping / 0.20f)));
            }
        }
    }
}
