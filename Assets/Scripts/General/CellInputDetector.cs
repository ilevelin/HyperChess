using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInputDetector : MonoBehaviour
{

    int[] cellCoords;
    BoardInputDetector board;

    public void Initialize(GameObject boardDetector, int[] coords)
    {
        board = boardDetector.GetComponent<BoardInputDetector>();
        cellCoords = coords;
    }

    private void OnMouseDown()
    {
        board.Press();
    }

    private void OnMouseUp()
    {
        board.Release();
    }

    private void OnMouseOver()
    {
        board.MouseCell(cellCoords);
    }

    private void OnMouseExit()
    {
        board.MouseCell(new int[] { -1, -1 });
    }

}
