using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceElement
{
    public string name;
    public string version;
    public string author;
    public BoardType boardType;

    public List<Move> moves;

    public PieceElement(string n, string v, string a, BoardType bt, List<Move> m)
    {
        name = n;
        version = v;
        author = a;
        boardType = bt;
        moves = m;
    }
}
