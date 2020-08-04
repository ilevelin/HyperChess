using System.Collections.Generic;
using UnityEngine;

public class BoardElement
{
    public Sprite image;

    public string version;
    public string author;
    public BoardType boardType;

    public List<PlayerImport> players;
    public Dictionary<char, PieceImport> pieceIDs;
    public Board initialState;
    // TODO Añadir Especiales

    public BoardElement()
    {
        image = null;
        version = null;
        author = null;
        boardType = BoardType.NULL;

        players = new List<PlayerImport>();
        pieceIDs = new Dictionary<char, PieceImport>();
    }
}

public class PlayerImport
{
    public int? team;
    public Color? color;
    public int? direction;

    public PlayerImport(int t, Color c, int d)
    {
        team = t;
        color = c;
        direction = d;
    }

    public PlayerImport()
    {
        team = null;
        color = null;
        direction = null;
    }
}

public enum PieceType
{
    NONE, PAWN, KING, UPGRADE
}

public class PieceImport
{
    public string ID;
    public int value;
    public PieceType type;

    public PieceImport()
    {
        ID = null;
        value = -1;
        type = PieceType.NONE;
    }
}
