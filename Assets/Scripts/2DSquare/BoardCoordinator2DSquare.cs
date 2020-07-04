using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardCoordinato2DSquare : MonoBehaviour, BoardCoordinator
{
    
    int[] pressCoords = null;
    int[] releaseCoords = null;
    int[] selectedCell = null;

    Piece2DSquare[][] board;

    public bool Initialize(int[] boardSize)
    {
        if (board.Length == 2)
        {
            board = new Piece2DSquare[boardSize[0]][];
            for (int i = 0; i < board.Length; i++) board[i] = new Piece2DSquare[boardSize[1]];
            return true;
        }
        else return false;
    }

    public void MousePressed(int[] location)
    {
        // TODO
    }

    public void MouseReleased(int[] location)
    {
        // TODO
    }
}
