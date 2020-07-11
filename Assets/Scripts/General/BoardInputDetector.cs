using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardInputDetector : MonoBehaviour
{
    int[] mouseLocation = null;
    [SerializeField] GameObject boardCoordinator;

    BoardCoordinator bc;

    public void Start()
    {
        bc = boardCoordinator.GetComponent<BoardCoordinator>();
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            bc.RightMousePressed(mouseLocation);
        }
        if (Input.GetKeyUp(KeyCode.Mouse1))
        {
            bc.RightMouseReleased(mouseLocation);
        }
    }

    public void Press()
    {
        bc.MousePressed(mouseLocation);
    }

    public void MouseCell(int[] cellCoords)
    {
        mouseLocation = cellCoords;

        String log = "MouseLocation = ";
        foreach (int n in mouseLocation)
            log += (n + ".");
    }

    public void Release()
    {
        bc.MouseReleased(mouseLocation);
    }

}
