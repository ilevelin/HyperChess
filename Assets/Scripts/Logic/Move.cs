using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Style
{
    INFINITE, FINITE, JUMP, INFINITEJUMP
}
public enum Type
{
    MOVE, CAPTURE, BOTH
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
}
