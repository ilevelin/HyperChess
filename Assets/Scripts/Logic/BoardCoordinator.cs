﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BoardCoordinator
{
    void Initialize(int[] boardSize);
    void EndTurn();
    void AfterPromotionEndTurn(char selectedPromotion);
    void KingSuscription(Piece newKing);

    bool CheckPromotions();
    void CheckMoves();
    void CheckKing();
    void PlayerResign();
    void EndGame(int winnerTeam);

    void MousePressed(int[] location);
    void MouseReleased(int[] location);
    void MovePiece(int[] from, int[] to);

    void PieceSubscription(int[] location, Piece piece);

    void RightMousePressed(int[] location);
    void RightMouseReleased(int[] location);

    int GetScoreOfPlayer(int i);
    Dictionary<char, Sprite> GetPromotablePieces();
    Piece GetPieceFromCell(int[] cell);
    bool? DidPieceMoved(int[] cell);

    void CreatePiece(int[] cell, char pieceChar);
    void RemovePiece(int[] cell);
    void MovePieceForced(int[] from, int[] to);

    void InitSimulation();
    void EndSimulation();
    void SimulateCreatePiece(int[] cell, char pieceChar);
    void SimulateRemovePiece(int[] cell);
    void SimulateMovePiece(int[] from, int[] to);

    bool IsCellUnderAttack(int[] cell);

    List<HistoryMove> GetLastMoves();
    HistoryMove GetLastMoveFromPlayer(int player);
}
