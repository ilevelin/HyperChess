using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInputDetector : MonoBehaviour
{
    int[] pressCoords = null;
    int[] releaseCoords = null;
    int[] selectedCell = null;
    int[] mouseLocation = null;
    
    public void Press()
    {
        pressCoords = mouseLocation;

        string logText = "Press detected: (";
        for (int i = 0; i < mouseLocation.Length; i++)
        {
            logText = logText + mouseLocation[i];
            if (i + 1 != mouseLocation.Length) logText = logText + ",";
        }
        logText = logText + ")";

        Debug.Log(logText);
    }

    public void MouseCell(int[] cellCoords)
    {
        mouseLocation = cellCoords;
    }

    public void Release()
    {
        releaseCoords = mouseLocation;

        string logText = "Release detected: (";
        for (int i = 0; i < mouseLocation.Length; i++)
        {
            logText = logText + mouseLocation[i];
            if (i + 1 != mouseLocation.Length) logText = logText + ",";
        }
        logText = logText + ")";

        Debug.Log(logText);

        if (pressCoords.Length == releaseCoords.Length)
        {
            bool tmp = true;
            for (int i = 0; i < pressCoords.Length; i++) if (pressCoords[i] != releaseCoords[i]) tmp = false;

            if (tmp)
            {
                if (selectedCell == null) {
                    selectedCell = pressCoords;
                    Debug.Log("Awaiting new click");
                }
                else
                {
                    MovePiece(selectedCell, pressCoords);
                    selectedCell = null;
                }

            }
            else
            {
                MovePiece(pressCoords, releaseCoords);
                selectedCell = null;
            }

            pressCoords = null;
            releaseCoords = null;
        }
    }


    void MovePiece(int[] from, int[] to)
    {
        string logText = "Move detected: (";
        for (int i = 0; i < from.Length; i++)
        {
            logText = logText + from[i];
            if (i + 1 != from.Length) logText = logText + ",";
        }
        logText = logText + ") -> (";
        for (int i = 0; i < to.Length; i++)
        {
            logText = logText + to[i];
            if (i + 1 != to.Length) logText = logText + ",";
        }
        logText = logText + ")";

        Debug.Log(logText);
    }

}
