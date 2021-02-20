using System.Collections.Generic;
using UnityEngine;

public class PieceElement
{
    public Dictionary<string,Sprite> sprites;

    public string name;
    public string version;
    public string author;
    public BoardType boardType;

    public List<Move> moves;

    public PieceElement()
    {
        moves = new List<Move>();
        sprites = new Dictionary<string, Sprite>();
    }
}
