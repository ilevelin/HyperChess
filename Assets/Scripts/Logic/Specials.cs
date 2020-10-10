using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* MAIN CLASS */

public class SpecialMove
{
    public List<SpecialCondition> conditions;
    public List<SpecialResult> results;

    public bool Check(BoardCoordinator board)
    {
        foreach(SpecialCondition cond in conditions)
            if (!cond.Check(board)) return false;
        return true;
    }
    public void RunMove(BoardCoordinator board)
    {
        foreach (SpecialResult res in results)
            res.Modify(board);
    }
    public Tuple<int[],int[]> GetMove()
    {
        foreach (SpecialResult res in results)
            if (res is SpecialResultMovePiece)
            {
                SpecialResultMovePiece move = (SpecialResultMovePiece)res;
                return new Tuple<int[], int[]>(move.from, move.to);
            }

        return null;
    }
}

/* CONDITIONS */

public interface SpecialCondition
{
    bool Check(BoardCoordinator board);
}

public enum SpecialConditionCheckType
{
    HASMOVED, HASNOTMOVED, ISPIECE, ISPLAYER, ISEMPTY, ISNOTEMPTY, ISATTACKED, ISSAFE, NULL
}

public class SpecialConditionCheck : SpecialCondition
{
    SpecialConditionCheckType type = SpecialConditionCheckType.NULL;
    char piece = '_'; // Only used if "type = ISPIECE"
    int player = 0; // Only used if "type = ISPLAYER"
    int[] cell = null;

    public SpecialConditionCheck(SpecialConditionCheckType t, int[] c)
    {
        type = t;
        cell = c;
    }

    public SpecialConditionCheck(SpecialConditionCheckType t, int[] c, char p)
    {
        type = t;
        cell = c;
        piece = p;
    }

    public SpecialConditionCheck(SpecialConditionCheckType t, int[] c, int p)
    {
        type = t;
        cell = c;
        player = p;
    }

    public bool Check(BoardCoordinator board)
    {
        switch (type)
        {
            case SpecialConditionCheckType.HASMOVED:
            case SpecialConditionCheckType.HASNOTMOVED:
                bool? tmp = board.DidPieceMoved(cell);
                if (type == SpecialConditionCheckType.HASNOTMOVED) tmp = !tmp;
                return tmp ?? false;

            case SpecialConditionCheckType.ISPIECE:
                return board.GetPieceFromCell(cell).GetCharacter() == piece;

            case SpecialConditionCheckType.ISPLAYER:
                return board.GetPieceFromCell(cell).GetPlayer() == player;

            case SpecialConditionCheckType.ISEMPTY:
                return board.GetPieceFromCell(cell) == null;

            case SpecialConditionCheckType.ISNOTEMPTY:
                return board.GetPieceFromCell(cell) != null;

            case SpecialConditionCheckType.ISATTACKED:
                return board.IsCellUnderAttack(cell);

            case SpecialConditionCheckType.ISSAFE:
                return !board.IsCellUnderAttack(cell);

            default:
                return false;
        }
    }
}

public class SpecialConditionLastMove : SpecialCondition
{
    int[] from, to;
    int player = 0;

    public bool Check(BoardCoordinator board)
    {
        HisotryMove lastMove;
        if (player != 0)
            lastMove = board.GetLastMoveFromPlayer(player);
        else
            lastMove = board.GetLastMove();

        bool result = true;
        if (from.Length == lastMove.from.Length && to.Length == lastMove.to.Length)
        {
            for (int i = 0; i < from.Length && result; i++)
            {
                if (from[i] != lastMove.from[i]) result = false;
                if (to[i] != lastMove.to[i]) result = false;
            }

            return result;
        }
        else return false;
    }

    public SpecialConditionLastMove(int[] f, int[] t)
    {
        from = f;
        to = t;
    }
    public SpecialConditionLastMove(int[] f, int[] t, int p)
    {
        from = f;
        to = t;
        player = p;
    }
}

/* RESULTS */

public interface SpecialResult
{
    void Modify(BoardCoordinator board);
}

public class SpecialResultMovePiece : SpecialResult
{
    public int[] from, to;

    public void Modify(BoardCoordinator board)
    {
        board.MovePieceForced(from, to);
    }

    public SpecialResultMovePiece(int[] f, int[] t)
    {
        from = f;
        to = t;
    }
}

public class SpecialResultCreatePiece : SpecialResult
{
    int[] where;
    char what;

    public void Modify(BoardCoordinator board)
    {
        board.CreatePiece(where, what);
    }

    public SpecialResultCreatePiece(int[] c, char p)
    {
        where = c;
        what = p;
    }
}

public class SpecialResultRemovePiece : SpecialResult
{
    int[] where;

    public void Modify(BoardCoordinator board)
    {
        board.RemovePiece(where);
    }

    public SpecialResultRemovePiece(int[] c)
    {
        where = c;
    }
}