using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInputDetector2DSquare : MonoBehaviour
{
    int[] pressCoords = null;
    int[] releaseCoords = null;
    int[] selectedCell = null;
    int[] mouseLocation = null;
    [SerializeField] GameObject selectedCellObject;
    Piece2DSquare[][] board = new Piece2DSquare[][] {
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8],
        new Piece2DSquare[8]
    };

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

    public void PieceSubscription(int[] location, Piece2DSquare piece)
    {
        board[location[0]][location[1]] = piece;
    }

    public void Press()
    {
        pressCoords = mouseLocation;
        if (board[pressCoords[0]][pressCoords[1]] != null)
        {
            board[pressCoords[0]][pressCoords[1]].Catched();
        }
    }

    public void MouseCell(int[] cellCoords)
    {
        mouseLocation = cellCoords;
    }

    public void Release()
    {
        releaseCoords = mouseLocation;

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


    void MovePiece(int[] from, int[] to)
    {
        if (board[to[0]][to[1]] != null)
        {
            board[to[0]][to[1]].Captured();
        }

        if (board[from[0]][from[1]] != null)
        {
            board[from[0]][from[1]].MoveTo(to);
            board[to[0]][to[1]] = board[from[0]][from[1]];
            board[from[0]][from[1]] = null;
        }
    }

}
