using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardElement
{
    public string version;
    public string author;
    public BoardType boardType;

    public List<Player> players;
    public Dictionary<char, string> pieceIDs;
    public Board initialState;
    // TODO Añadir Especiales

    public BoardElement(string v, string a, BoardType bt, List<Player> pl, Dictionary<char, string> pi, Board ini)
    {
        version = v;
        author = a;
        boardType = bt;
        players = pl;
        pieceIDs = pi;
        initialState = ini;
    }
}

public class Player
{
    int team;
    Color color;
    int[] direction;

    public Player(int t, Color c, int[] d)
    {
        team = t;
        color = c;
        direction = d;
    }
}
