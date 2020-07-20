using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfo
{
    // Times in miliseconds
    public int startingTime, incrementTime, delayTime;
    public string name;
    public Color color;
    public bool alive = true;

    public PlayerInfo(int st, int it, int dt, string n, Color c)
    {
        startingTime = st;
        incrementTime = it;
        delayTime = dt;
        name = n;
        color = c;
    }

}
