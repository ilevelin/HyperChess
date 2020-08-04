using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Style
{
    NULL, INFINITE, FINITE, JUMP, INFINITEJUMP
}
public enum Type
{
    NULL, MOVE, CAPTURE, BOTH
}

public class Move
{
    public int[] move { get; set; }
    public Style style { get; set; }
    public Type type { get; set; }

    public Move(int[] move, Style style, Type type)
    {
        this.move = move;
        this.style = style;
        this.type = type;
    }

    public Move() { }
}
