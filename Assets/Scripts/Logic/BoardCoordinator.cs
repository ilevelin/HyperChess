using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface BoardCoordinator
{

    bool Initialize(int[] boardSize);
    void MousePressed(int[] location);
    void MouseReleased(int[] location);
        
}
