using System.Collections.Generic;
using UnityEngine;

public class PieceElement
{
    public Sprite image;

    public string name;
    public string version;
    public string author;
    public BoardType boardType;

    public List<Move> moves;

    public PieceElement()
    {
        moves = new List<Move>();
    }
}
