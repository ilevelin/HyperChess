using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordinatorSquare2D : MonoBehaviour, BoardCoordinator
{
    [SerializeField] GameObject selectedCellObject, arrowMarkerPrefab, circleMarkerPrefab, playerInterface, boardGeneratorObject;
    PlayerInfoCoordinator interfaceCoordinator;
    BoardGeneratorSquare2D boardGenerator;

    int[] pressCoords = null;
    int[] releaseCoords = null;
    int[] selectedCell = null;

    int[] rightPressCoords = null;
    int[] rightReleaseCoords = null;

    List<ArrowMarker> arrowMarkers = new List<ArrowMarker>();
    List<CircleMarker> circleMarkers = new List<CircleMarker>();
    List<SpecialMove> availableSpecials = new List<SpecialMove>();
    List<SpecialMove> specialMoves = new List<SpecialMove>();
    public List<HisotryMove> moveHistory = new List<HisotryMove>();

    PieceSquare2D[][] board;
    bool[][] movedPieces;
    bool[][] existingCells;
    bool[][] attacked;

    private void Start()
    {
        interfaceCoordinator = playerInterface.GetComponent<PlayerInfoCoordinator>();
        boardGenerator = boardGeneratorObject.GetComponent<BoardGeneratorSquare2D>();
    }

    private void Update()
    {
        if (selectedCellObject != null)
        {
            if (selectedCell == null)
                selectedCellObject.transform.position = new Vector3(-100, -100);
            else
                selectedCellObject.transform.position = new Vector3(selectedCell[0], selectedCell[1]);
        }
    }

    public void Initialize(int[] boardSize)
    {
        if (boardSize.Length != 2) return;
        {
            board = new PieceSquare2D[boardSize[0]][];
            existingCells = new bool[boardSize[0]][];
            movedPieces = new bool[boardSize[0]][];
            for (int i = 0; i < board.Length; i++)
            {
                board[i] = new PieceSquare2D[boardSize[1]];
                existingCells[i] = new bool[boardSize[1]];
                movedPieces[i] = new bool[boardSize[1]];
                for (int j = 0; j < existingCells[i].Length; j++)
                {
                    existingCells[i][j] = true;
                    movedPieces[i][j] = false;
                }
            }
        }

        playerInterface.GetComponent<PlayerInfoCoordinator>().Initialize(this);
    }

    public void CheckMoves()
    {
        List<int[]> attackedCells = new List<int[]>();
        for (int i = 0; i < board.Length; i++)
            for (int j = 0; j < board[i].Length; j++)
                if (board[i][j] != null)
                {
                    if (board[i][j].player == interfaceCoordinator.turn) // Modificar esto mas tarde para impedir al rey que se meta donde no toca.
                        board[i][j].CheckMoves(board, existingCells);
                    else
                        attackedCells.AddRange(board[i][j].GetAttacks(board, existingCells));
                }

        for (int i = 0; i < attacked.Length; i++)
            for (int j = 0; j < attacked[i].Length; j++)
                attacked[i][j] = false;

        foreach (int[] cell in attackedCells)
            attacked[cell[0]][cell[1]] = true;

        availableSpecials.Clear();
        foreach(SpecialMove move in specialMoves)
            if (move.Check(this)) availableSpecials.Add(move);
    }

    public void MousePressed(int[] location)
    {
        if (location.Length != 2) return;

        pressCoords = location;
        if (board[pressCoords[0]][pressCoords[1]] != null)
        {
            board[pressCoords[0]][pressCoords[1]].Catched();
        }

        while (arrowMarkers.Count != 0)
        {
            arrowMarkers[0].Remove();
            arrowMarkers.Remove(arrowMarkers[0]);
        }

        while (circleMarkers.Count != 0)
        {
            circleMarkers[0].Remove();
            circleMarkers.Remove(circleMarkers[0]);
        }
    }

    public void MouseReleased(int[] location)
    {
        if (location.Length != 2) return;

        releaseCoords = location;

        if (board[pressCoords[0]][pressCoords[1]] != null)
        {
            board[pressCoords[0]][pressCoords[1]].Released();
        }

        if (releaseCoords[0] != -1)
        {
            if (pressCoords.Length == releaseCoords.Length)
            {
                bool isClick = true;
                for (int i = 0; i < pressCoords.Length; i++) if (pressCoords[i] != releaseCoords[i]) isClick = false;

                if (isClick)
                {
                    if (selectedCell == null)
                    {
                        if (board[pressCoords[0]][pressCoords[1]] != null)
                            selectedCell = pressCoords;
                    }
                    else
                    {
                        board[selectedCell[0]][selectedCell[1]].HideMoves();
                        bool isSameCell = true;
                        for (int i = 0; i < pressCoords.Length; i++) if (pressCoords[i] != selectedCell[i]) isSameCell = false;

                        if (!isSameCell)
                        {
                            MovePiece(selectedCell, pressCoords);
                        }
                        selectedCell = null;
                    }

                }
                else
                {
                    board[pressCoords[0]][pressCoords[1]].HideMoves();
                    MovePiece(pressCoords, releaseCoords);
                    selectedCell = null;
                }

                pressCoords = null;
                releaseCoords = null;
            }
        }
    }

    public void RightMousePressed(int[] location)
    {
        if (location.Length != 2) return;

        rightPressCoords = location;
    }

    public void RightMouseReleased(int[] location)
    {
        if (location.Length != 2) return;
        if (rightPressCoords[0] == -1 || location[0] == -1) return;

        rightReleaseCoords = location;
        bool exists = false;


        GameObject tmp;
        if ((rightPressCoords[0] == rightReleaseCoords[0]) && (rightPressCoords[1] == rightReleaseCoords[1]))
        {
            for (int i = 0; i < circleMarkers.Count && !exists; i++)
            {
                if ((circleMarkers[i].position[0] == rightPressCoords[0]) &&
                    (circleMarkers[i].position[1] == rightPressCoords[1]))
                {
                    circleMarkers[i].Remove();
                    circleMarkers.Remove(circleMarkers[i]);
                    exists = true;
                }
            }

            if (!exists)
            {
                tmp = GameObject.Instantiate(circleMarkerPrefab);
                tmp.GetComponent<CircleMarker>().position = rightPressCoords;
                tmp.GetComponent<CircleMarker>().Initialize();

                circleMarkers.Add(tmp.GetComponent<CircleMarker>());
            }
        }
        else
        {
            for (int i = 0; i < arrowMarkers.Count && !exists; i++)
            {
                if ((arrowMarkers[i].headPosition[0] == rightReleaseCoords[0]) &&
                    (arrowMarkers[i].headPosition[1] == rightReleaseCoords[1]) &&
                    (arrowMarkers[i].tailPosition[0] == rightPressCoords[0]) &&
                    (arrowMarkers[i].tailPosition[1] == rightPressCoords[1]))
                {
                    arrowMarkers[i].Remove();
                    arrowMarkers.Remove(arrowMarkers[i]);
                    exists = true;
                }
            }

            if (!exists)
            {
                tmp = GameObject.Instantiate(arrowMarkerPrefab);
                tmp.GetComponent<ArrowMarker>().headPosition = rightReleaseCoords;
                tmp.GetComponent<ArrowMarker>().tailPosition = rightPressCoords;
                tmp.GetComponent<ArrowMarker>().Initialize();

                arrowMarkers.Add(tmp.GetComponent<ArrowMarker>());
            }
        }

    }

    public void PieceSubscription(int[] location, Piece piece)
    {
        if ((location.Length == 2) && (piece is PieceSquare2D newpiece))
        {
            board[location[0]][location[1]] = newpiece;
        }
        
    }

    public void MovePiece(int[] from, int[] to)
    {

        if (board[from[0]][from[1]] != null)
            if(board[from[0]][from[1]].player == (interfaceCoordinator.turn+1))
            {
                if (board[from[0]][from[1]].MoveTo(to))
                {
                    if (board[to[0]][to[1]] != null)
                        board[to[0]][to[1]].Captured();
                    board[to[0]][to[1]] = board[from[0]][from[1]];
                    board[from[0]][from[1]] = null;

                    moveHistory.Insert(0, new HisotryMove(interfaceCoordinator.turn, from, to));
                    interfaceCoordinator.NextTurn(true);
                    CheckMoves();
                }
                else
                {
                    foreach (SpecialMove move in availableSpecials)
                    {
                        Tuple<int[], int[]> coords = move.GetMove();
                        if(coords.Item1.Length == 2 && coords.Item2.Length == 2)
                            if(coords.Item1[0] == from[0] && coords.Item1[1] == from[1])
                                if(coords.Item2[0] == to[0] && coords.Item2[1] == to[1])
                                {
                                    move.RunMove(this);
                                    moveHistory.Insert(0, new HisotryMove(interfaceCoordinator.turn, from, to));
                                    interfaceCoordinator.NextTurn(true);
                                    CheckMoves();
                                    break;
                                }
                    }
                }

            }
    }

    public int GetScoreOfPlayer(int i)
    {
        int counter = 0;

        foreach (Piece[] row in board)
            foreach (Piece piece in row)
                if (!(piece is null))
                    if (piece is PieceSquare2D)
                        if (((PieceSquare2D)piece).player == i)
                            counter += ((PieceSquare2D)piece).value;

        return counter;
    }

    public Piece GetPieceFromCell(int[] cell)
    {
        if (cell.Length == 2)
            return board[cell[0]][cell[1]];
        else
            return null;
    }
    
    public bool? DidPieceMoved(int[] cell)
    {
        if (cell.Length == 2)
        {
            if (board[cell[0]][cell[1]] == null) return null;
            else return movedPieces[cell[0]][cell[1]];
        }
        else return null;
    }

    public void CreatePiece(int[] cell, char pieceChar)
    {
        boardGenerator.CreatePiece(cell, pieceChar, interfaceCoordinator.turn + 1);
        movedPieces[cell[0]][cell[1]] = true;
    }

    public void RemovePiece(int[] cell)
    {
        if (cell.Length == 2)
        {
            if (board[cell[0]][cell[1]] != null) board[cell[0]][cell[1]].Captured();
            board[cell[0]][cell[1]] = null;
            movedPieces[cell[0]][cell[1]] = true;
        }
    }

    public void MovePieceForced(int[] from, int[] to)
    {
        if (from.Length == 2 && to.Length == 2)
        {
            if (board[to[0]][to[1]] != null) board[to[0]][to[1]].Captured();
            board[to[0]][to[1]] = board[from[0]][from[1]];
            board[from[0]][from[1]] = null;
            movedPieces[from[0]][from[1]] = true;
            movedPieces[to[0]][to[1]] = true;
        }
    }

    public bool IsCellUnderAttack(int[] cell)
    {
        if (cell.Length == 2)
        {
            return attacked[cell[0]][cell[1]];
        }
        else return false;
    }

    public HisotryMove GetLastMove()
    {
        if (moveHistory.Count == 0) return null;
        else return moveHistory[0];
    }

    public HisotryMove GetLastMoveFromPlayer(int player)
    {
        foreach (HisotryMove move in moveHistory)
            if (move.player == player) return move;
        return null;
    }
}

public class HisotryMove
{
    public int player;
    public int[] from, to;

    public HisotryMove(int p, int[] f, int[] t)
    {
        player = p;
        from = f;
        to = t;
    }
}