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
    public Board board;
    public List<Color> colorList;
    public bool hasCustomColorPlacement;
    public List<SpecialMove> specials;

    public BoardElement()
    {
        image = null;
        version = null;
        author = null;
        boardType = BoardType.NULL;

        colorList = new List<Color>();
        players = new List<PlayerImport>();
        pieceIDs = new Dictionary<char, PieceImport>();
        specials = new List<SpecialMove>();
    }
}

public class PlayerImport
{
    public int? team;
    public Color? color;
    public Color? interfaceColor;
    public int? direction;
    public string pieceVariant;

    public PlayerImport(int t, Color c, int d)
    {
        team = t;
        color = c;
        interfaceColor = c;
        direction = d;
        pieceVariant = "default";
    }

    public PlayerImport(int t, Color c, int d, string pv)
    {
        team = t;
        color = c;
        interfaceColor = c;
        direction = d;
        pieceVariant = pv;
    }

    public PlayerImport(int t, Color c, Color ic, int d, string pv)
    {
        team = t;
        color = c;
        interfaceColor = ic;
        direction = d;
        pieceVariant = pv;
    }

    public PlayerImport(int t, Color c, Color ic, int d)
    {
        team = t;
        color = c;
        interfaceColor = ic;
        direction = d;
        pieceVariant = "default";
    }

    public PlayerImport()
    {
        team = null;
        color = null;
        interfaceColor = null;
        direction = null;
        pieceVariant = null;
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