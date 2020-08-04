using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Piece
{

    void Initialize(int[] initialPosition, GameObject inputDetector, int owner, int ownerTeam, Color onwerColor, List<Move> pieceMoves, int val, Sprite image);
    bool MoveTo(int[] newPosition);
    void Captured();
    void Catched();
    void Released();
    void ShowMoves();
    void HideMoves();

}
