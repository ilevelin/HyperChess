using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece2DSquare : MonoBehaviour
{

    int[] position = null;
    int player;
    int team;
    bool onHold = false, isSelected = false;
    public List<Move> moves = new List<Move>();
    public List<int[]> avaliableMoves = new List<int[]>();

    private void Update()
    {
        if (onHold)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    public void Initialize(int[] initialPosition, GameObject inputDetector, int owner, int ownerTeam)
    {
        position = initialPosition;
        inputDetector.GetComponent<BoardInputDetector2DSquare>().PieceSubscription(initialPosition, this);
        player = owner;
        team = ownerTeam;
        RenderPiece();
    }

    public void checkMoves(Piece2DSquare[][] board)
    {
        avaliableMoves.Clear();
        foreach (Move move in moves)
        {
            Piece2DSquare objectiveCell = board[position[0] + move.move[0]][position[1] + move.move[1]];
            switch (move.style)
            {
                case Style.FINITE:
                    switch (move.type)
                    {
                        case Type.BOTH:
                            break;
                        case Type.CAPTURE:
                            break;
                        case Type.MOVE:
                            break;
                    }
                    break;

                case Style.INFINITE:
                    switch (move.type)
                    {
                        case Type.BOTH:
                            break;
                        case Type.CAPTURE:
                            break;
                        case Type.MOVE:
                            break;
                    }
                    break;

                case Style.INFINITEJUMP:
                    switch (move.type)
                    {
                        case Type.BOTH:
                            break;
                        case Type.CAPTURE:
                            break;
                        case Type.MOVE:
                            break;
                    }
                    break;

                case Style.JUMP:
                    switch (move.type)
                    {
                        case Type.BOTH:
                            if (objectiveCell.team != this.team) avaliableMoves.Add(new int[] { position[0] + move.move[0], position[1] + move.move[1] });
                            break;
                        case Type.CAPTURE:
                            if (objectiveCell != null) if (objectiveCell.team != this.team) avaliableMoves.Add(new int[] { position[0] + move.move[0], position[1] + move.move[1] });
                            break;
                        case Type.MOVE:
                            if (objectiveCell == null) avaliableMoves.Add(new int[] { position[0] + move.move[0], position[1] + move.move[1] });
                            break;
                    }
                    break;
            }
        }
    }

    public bool MoveTo(int[] newPosition)
    {
        bool contains = false;
        for (int i = 0; i < avaliableMoves.Count && !contains; i++) {
            bool compare = true;
            for (int j = 0; j < newPosition.Length; j++) if (avaliableMoves[i][j] != newPosition[j]) compare = false;
            contains = compare;
        }
        if (contains)
        {
            position = newPosition;
            RenderPiece();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Captured()
    {
        GameObject.Destroy(gameObject);
    }

    public void Catched()
    {
        onHold = true;
    }

    public void Released()
    {
        onHold = false;
        RenderPiece();
    }

    private void RenderPiece()
    {
        transform.position = new Vector3(position[0], position[1]);
    }

}
