using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BoardCoordinator
{
    void Initialize(int[] boardSize);
    void CheckMoves();

    void MousePressed(int[] location);
    void MouseReleased(int[] location);
    void MovePiece(int[] from, int[] to);

    void PieceSubscription(int[] location, Piece piece);

    void RightMousePressed(int[] location);
    void RightMouseReleased(int[] location);

    int GetScoreOfPlayer(int i);
}
