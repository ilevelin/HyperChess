using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceSquare2D : MonoBehaviour, Piece
{

    int[] position = null;
    public char character;
    public int player;
    public int team;
    public int value;
    public int direction;
    bool onHold = false, isSelected = false;
    public List<Move> moves = new List<Move>();
    public List<int[]> avaliableMoves = new List<int[]>();
    public List<int[]> avaliableSpecials = new List<int[]>();
    [SerializeField] GameObject possibleMovePrefab;
    GameObject possibleMoveParent;
    public PieceType type;

    private void Update()
    {
        if (onHold)
            transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10));
    }

    public void Initialize(int[] initialPosition, GameObject coordinator, int owner, int ownerTeam, Color ownerColor, List<Move> pieceMoves, int dir, int val, char charac, PieceType piecetype, Sprite image)
    {
        direction = dir;
        position = initialPosition;
        coordinator.GetComponent<BoardCoordinator>().PieceSubscription(initialPosition, this);
        player = owner;
        team = ownerTeam;
        GetComponent<SpriteRenderer>().sprite = image;
        GetComponent<SpriteRenderer>().color = ownerColor;
        moves = pieceMoves;
        value = val;
        possibleMoveParent = GameObject.FindWithTag("PossibleMoves");
        character = charac;
        type = piecetype;
        RenderPiece();
    }

    public void CheckMoves(PieceSquare2D[][] board, bool[][] existingCells)
    {
        avaliableMoves.Clear();
        foreach (Move baseMove in moves)
        {
            Move move = new Move(new int[baseMove.move.Length], baseMove.style, baseMove.type);
            for (int i = 0; i < baseMove.move.Length; i++)
            {
                int newindex = i + Math.Abs(direction) - 1;
                if (newindex >= baseMove.move.Length) newindex -= baseMove.move.Length;
                if (direction >= 0)
                    move.move[newindex] = baseMove.move[i];
                else
                    move.move[newindex] = -baseMove.move[i];
            }

            int x = position[0] + move.move[0];
            int y = position[1] + move.move[1];
            if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
            {
                PieceSquare2D objectiveCell = board[x][y];
                switch (move.style)
                {
                    case Style.FINITE: // FINITE ====================================
                        int minorAxis = 0;
                        if (Math.Abs(move.move[1]) < Math.Abs(move.move[0]))
                            minorAxis = 1;
                        int mayorAxis = 1 - minorAxis;

                        bool blocked = false;
                        if (move.move[minorAxis] != 0)
                        {
                            for (int i = 1; i < Math.Abs(move.move[minorAxis]) && !blocked; i++)
                                if ((move.move[mayorAxis] * i) % move.move[minorAxis] == 0)
                                {
                                    int tmpX = position[0] + ((move.move[0] / Math.Abs(move.move[minorAxis])) * i);
                                    int tmpY = position[1] + ((move.move[1] / Math.Abs(move.move[minorAxis])) * i);
                                    if (!(board[tmpX][tmpY] is null))
                                    {
                                        blocked = true;
                                    }
                                }
                        }
                        else
                        {
                            for (int i = 1; i < Math.Abs(move.move[mayorAxis]) && !blocked; i++)
                            {
                                int tmpX = i + position[0];
                                int tmpY = i + position[1];
                                if (!(board[tmpX][tmpY] is null))
                                {
                                    blocked = true;
                                }

                            }
                        }

                        if (!blocked)
                            switch (move.type)
                            {
                                case Type.BOTH:
                                    if ((objectiveCell == null) || (objectiveCell.team != this.team))
                                        avaliableMoves.Add(new int[] { x, y });
                                    break;
                                case Type.CAPTURE:
                                    if (objectiveCell != null) if (objectiveCell.team != this.team)
                                            avaliableMoves.Add(new int[] { x, y });
                                    break;
                                case Type.MOVE:
                                    if (objectiveCell == null)
                                        avaliableMoves.Add(new int[] { x, y });
                                    break;
                            }
                        break;

                    case Style.INFINITE: // INFINITE ====================================
                        int minorAxis1 = 0;
                        if (Math.Abs(move.move[1]) < Math.Abs(move.move[0]))
                            minorAxis1 = 1;
                        int mayorAxis1 = 1 - minorAxis1;

                        int[] tmpPosition2 = position;
                        bool possible2 = true;

                        while (possible2)
                        {
                            bool blocked1 = false;
                            if (move.move[minorAxis1] != 0)
                            {
                                for (int i = 1; i < Math.Abs(move.move[minorAxis1]) && !blocked1; i++)
                                    if ((move.move[mayorAxis1] * i) % move.move[minorAxis1] == 0)
                                    {
                                        int tmpX = tmpPosition2[0] + ((move.move[0] / Math.Abs(move.move[minorAxis1])) * i);
                                        int tmpY = tmpPosition2[1] + ((move.move[1] / Math.Abs(move.move[minorAxis1])) * i);
                                        if (!(board[tmpX][tmpY] is null))
                                        {
                                            blocked1 = true;
                                        }
                                    }
                            }
                            else
                            {
                                for (int i = 1; i < Math.Abs(move.move[mayorAxis1]) && !blocked1; i++)
                                {
                                    int tmpX = tmpPosition2[0] + i;
                                    int tmpY = tmpPosition2[1] + i;
                                    if (!(board[tmpX][tmpY] is null))
                                    {
                                        blocked1 = true;
                                    }

                                }
                            }

                            if (!blocked1)
                            {
                                switch (move.type)
                                {
                                    case Type.BOTH:
                                        if (objectiveCell == null)
                                            avaliableMoves.Add(new int[] { x, y });
                                        else if (objectiveCell.team != this.team)
                                        {
                                            avaliableMoves.Add(new int[] { x, y });
                                            possible2 = false;
                                        }
                                        else
                                            possible2 = false;
                                        break;
                                    case Type.CAPTURE:
                                        if (objectiveCell != null) if (objectiveCell.team != this.team)
                                                avaliableMoves.Add(new int[] { x, y });
                                            else
                                                possible2 = false;
                                        break;
                                    case Type.MOVE:
                                        if (objectiveCell == null)
                                            avaliableMoves.Add(new int[] { x, y });
                                        else
                                            possible2 = false;
                                        break;
                                }
                            }
                            else
                                possible2 = false;

                            if (possible2 && move.type != Type.CAPTURE)
                            {
                                tmpPosition2 = new int[] { x, y };
                                x = tmpPosition2[0] + move.move[0];
                                y = tmpPosition2[1] + move.move[1];
                                if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
                                {
                                    objectiveCell = board[x][y];
                                }
                                else
                                    possible2 = false;

                            }
                            else
                                possible2 = false;
                        }
                        break;

                    case Style.INFINITEJUMP: // INFINITE JUMP ====================================
                        bool possible1 = true;
                        int[] tmpPosition1 = position;
                        while (possible1)
                        {
                            switch (move.type)
                            {
                                case Type.BOTH:
                                    if (objectiveCell == null)
                                    {
                                        avaliableMoves.Add(new int[] { x, y });
                                    }
                                    else if (objectiveCell.team != this.team)
                                    {
                                        avaliableMoves.Add(new int[] { x, y });
                                        possible1 = false;
                                    }
                                    else
                                        possible1 = false;
                                    break;
                                case Type.CAPTURE:
                                    if (objectiveCell != null) if (objectiveCell.team != this.team)
                                        {
                                            avaliableMoves.Add(new int[] { tmpPosition1[0] + move.move[0], tmpPosition1[1] + move.move[1] });
                                        }
                                    break;
                                case Type.MOVE:
                                    if (objectiveCell == null)
                                    {
                                        avaliableMoves.Add(new int[] { x, y });
                                    }
                                    else
                                        possible1 = false;
                                    break;
                            }

                            if (possible1 && move.type != Type.CAPTURE)
                            {
                                tmpPosition1 = new int[] { x, y };
                                x = tmpPosition1[0] + move.move[0];
                                y = tmpPosition1[1] + move.move[1];
                                if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
                                    objectiveCell = board[x][y];
                                else
                                    possible1 = false;
                            }
                            else
                                possible1 = false;

                        }
                        break;

                    case Style.JUMP: // JUMP ====================================
                        switch (move.type)
                        {
                            case Type.BOTH:
                                if ((objectiveCell == null) || (objectiveCell.team != this.team)) avaliableMoves.Add(new int[] { x, y });
                                break;
                            case Type.CAPTURE:
                                if (objectiveCell != null) if (objectiveCell.team != this.team) avaliableMoves.Add(new int[] { x, y });
                                break;
                            case Type.MOVE:
                                if (objectiveCell == null) avaliableMoves.Add(new int[] { x, y });
                                break;
                        }
                        break;
                }
            }
        }
    }

    public void ForceMoveTo(int[] newPosition)
    {
        position = newPosition;
        RenderPiece();
    }

    public List<int[]> GetAttacks(PieceSquare2D[][] board, bool[][] existingCells)
    {
        List<int[]> attacking = new List<int[]>();
        foreach (Move move in moves)
        {
            if (move.type == Type.MOVE) continue;
            int x = position[0] + move.move[0];
            int y = position[1] + move.move[1];
            if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
            {
                PieceSquare2D objectiveCell = board[x][y];
                switch (move.style)
                {
                    case Style.FINITE: // FINITE ====================================
                        int minorAxis = 0;
                        if (Math.Abs(move.move[1]) < Math.Abs(move.move[0]))
                            minorAxis = 1;
                        int mayorAxis = 1 - minorAxis;

                        bool blocked = false;
                        if (move.move[minorAxis] != 0)
                        {
                            for (int i = 1; i < Math.Abs(move.move[minorAxis]) && !blocked; i++)
                                if ((move.move[mayorAxis] * i) % move.move[minorAxis] == 0)
                                {
                                    int tmpX = position[0] + ((move.move[0] / Math.Abs(move.move[minorAxis])) * i);
                                    int tmpY = position[1] + ((move.move[1] / Math.Abs(move.move[minorAxis])) * i);
                                    if (!(board[tmpX][tmpY] is null))
                                    {
                                        blocked = true;
                                    }
                                }
                        }
                        else
                        {
                            for (int i = 1; i < Math.Abs(move.move[mayorAxis]) && !blocked; i++)
                            {
                                int tmpX = i + position[0];
                                int tmpY = i + position[1];
                                if (!(board[tmpX][tmpY] is null))
                                {
                                    blocked = true;
                                }

                            }
                        }

                        if (!blocked)
                            switch (move.type)
                            {
                                case Type.BOTH:
                                    if ((objectiveCell == null) || (objectiveCell.team != this.team))
                                        attacking.Add(new int[] { x, y });
                                    break;
                                case Type.CAPTURE:
                                    if (objectiveCell != null) if (objectiveCell.team != this.team)
                                            attacking.Add(new int[] { x, y });
                                    break;
                            }
                            break;

                    case Style.INFINITE: // INFINITE ====================================
                        int minorAxis1 = 0;
                        if (Math.Abs(move.move[1]) < Math.Abs(move.move[0]))
                            minorAxis1 = 1;
                        int mayorAxis1 = 1 - minorAxis1;

                        int[] tmpPosition2 = position;
                        bool possible2 = true;

                        while (possible2)
                        {
                            bool blocked1 = false;
                            if (move.move[minorAxis1] != 0)
                            {
                                for (int i = 1; i < Math.Abs(move.move[minorAxis1]) && !blocked1; i++)
                                    if ((move.move[mayorAxis1] * i) % move.move[minorAxis1] == 0)
                                    {
                                        int tmpX = tmpPosition2[0] + ((move.move[0] / Math.Abs(move.move[minorAxis1])) * i);
                                        int tmpY = tmpPosition2[1] + ((move.move[1] / Math.Abs(move.move[minorAxis1])) * i);
                                        if (!(board[tmpX][tmpY] is null))
                                        {
                                            blocked1 = true;
                                        }
                                    }
                            }
                            else
                            {
                                for (int i = 1; i < Math.Abs(move.move[mayorAxis1]) && !blocked1; i++)
                                {
                                    int tmpX = tmpPosition2[0] + i;
                                    int tmpY = tmpPosition2[1] + i;
                                    if (!(board[tmpX][tmpY] is null))
                                    {
                                        blocked1 = true;
                                    }

                                }
                            }

                            if (!blocked1)
                            {
                                switch (move.type)
                                {
                                    case Type.BOTH:
                                        if (objectiveCell == null)
                                            attacking.Add(new int[] { x, y });
                                        else if (objectiveCell.team != this.team)
                                        {
                                            attacking.Add(new int[] { x, y });
                                            possible2 = false;
                                        }
                                        else
                                            possible2 = false;
                                        break;
                                    case Type.CAPTURE:
                                        if (objectiveCell != null) if (objectiveCell.team != this.team)
                                                avaliableMoves.Add(new int[] { x, y });
                                            else
                                                possible2 = false;
                                        break;
                                }
                            }
                            else
                                possible2 = false;

                            if (possible2 && move.type != Type.CAPTURE)
                            {
                                tmpPosition2 = new int[] { x, y };
                                x = tmpPosition2[0] + move.move[0];
                                y = tmpPosition2[1] + move.move[1];
                                if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
                                {
                                    objectiveCell = board[x][y];
                                }
                                else
                                    possible2 = false;

                            }
                            else
                                possible2 = false;
                        }
                        break;

                    case Style.INFINITEJUMP: // INFINITE JUMP ====================================
                        bool possible1 = true;
                        int[] tmpPosition1 = position;
                        while (possible1)
                        {
                            switch (move.type)
                            {
                                case Type.BOTH:
                                    if (objectiveCell == null)
                                    {
                                        attacking.Add(new int[] { x, y });
                                    }
                                    else if (objectiveCell.team != this.team)
                                    {
                                        attacking.Add(new int[] { x, y });
                                        possible1 = false;
                                    }
                                    else
                                        possible1 = false;
                                    break;
                                case Type.CAPTURE:
                                    if (objectiveCell != null) if (objectiveCell.team != this.team)
                                        {
                                            attacking.Add(new int[] { tmpPosition1[0] + move.move[0], tmpPosition1[1] + move.move[1] });
                                        }
                                    break;
                            }

                            if (possible1 && move.type != Type.CAPTURE)
                            {
                                tmpPosition1 = new int[] { x, y };
                                x = tmpPosition1[0] + move.move[0];
                                y = tmpPosition1[1] + move.move[1];
                                if ((x >= 0) && (x < board.Length) && (y >= 0) && (y < board[0].Length) && existingCells[x][y])
                                    objectiveCell = board[x][y];
                                else
                                    possible1 = false;
                            }
                            else
                                possible1 = false;

                        }
                        break;

                    case Style.JUMP: // JUMP ====================================
                        switch (move.type)
                        {
                            case Type.BOTH:
                                if ((objectiveCell == null) || (objectiveCell.team != this.team)) attacking.Add(new int[] { x, y });
                                break;
                            case Type.CAPTURE:
                                if (objectiveCell != null) if (objectiveCell.team != this.team) attacking.Add(new int[] { x, y });
                                break;
                        }
                        break;
                }
            }
        }
        return attacking;
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
            RenderPiece();
            return false;
        }
    }

    public void Captured()
    {
        HideMoves();
        GameObject.Destroy(gameObject);
    }

    public void Catched()
    {
        onHold = true;
        ShowMoves();
    }

    public void Released()
    {
        onHold = false;
        RenderPiece();
    }

    public void ShowMoves()
    {
        HideMoves();
        foreach (int[] move in avaliableMoves)
        {
            GameObject.Instantiate(possibleMovePrefab, new Vector3(move[0], move[1]), new Quaternion(), possibleMoveParent.transform);
        }
        foreach (int[] move in avaliableSpecials)
        {
            GameObject.Instantiate(possibleMovePrefab, new Vector3(move[0], move[1]), new Quaternion(), possibleMoveParent.transform);
        }
    }

    public void HideMoves()
    {
        foreach (Transform child in possibleMoveParent.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    private void RenderPiece()
    {
        transform.position = new Vector3(position[0], position[1]);
    }



    public char GetCharacter()
    {
        return character;
    }

    public int GetPlayer()
    {
        return player;
    }
}
