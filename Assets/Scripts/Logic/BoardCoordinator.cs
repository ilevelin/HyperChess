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

    Piece GetPieceFromCell(int[] cell);
    bool? DidPieceMoved(int[] cell);
    void CreatePiece(int[] cell, char pieceChar);
    void RemovePiece(int[] cell);
    void MovePieceForced(int[] from, int[] to);
    bool IsCellUnderAttack(int[] cell);

    HisotryMove GetLastMove();
    HisotryMove GetLastMoveFromPlayer(int player);
}
