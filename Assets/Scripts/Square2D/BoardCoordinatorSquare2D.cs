using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordinatorSquare2D : MonoBehaviour, BoardCoordinator
{
    [SerializeField] GameObject selectedCellObject, arrowMarkerPrefab, circleMarkerPrefab, playerInterface;
    PlayerInfoCoordinator interfaceCoordinator;

    int[] pressCoords = null;
    int[] releaseCoords = null;
    int[] selectedCell = null;

    int[] rightPressCoords = null;
    int[] rightReleaseCoords = null;

    List<ArrowMarker> arrowMarkers = new List<ArrowMarker>();
    List<CircleMarker> circleMarkers = new List<CircleMarker>();

    PieceSquare2D[][] board;

    private void Start()
    {
        interfaceCoordinator = playerInterface.GetComponent<PlayerInfoCoordinator>();
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
            for (int i = 0; i < board.Length; i++) board[i] = new PieceSquare2D[boardSize[1]];
        }

        playerInterface.GetComponent<PlayerInfoCoordinator>().Initialize(this);
    }

    public void CheckMoves()
    {
        for (int i = 0; i < board.Length; i++)
            for (int j = 0; j < board[i].Length; j++)
                if (board[i][j] != null)
                    board[i][j].CheckMoves(board);
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

                    interfaceCoordinator.NextTurn(true);

                    CheckMoves();
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
}
