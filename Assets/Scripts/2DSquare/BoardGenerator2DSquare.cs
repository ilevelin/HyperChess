using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BoardGenerator2DSquare : MonoBehaviour
{

    private int boardHeigth = 1, boardWidth = 1;
    [SerializeField] GameObject boardParent, gameCamera, cellPrefab, boardInputDetector;
    [SerializeField] Color cellFirstColor, cellSecondColor;
    int[][] board = new int[][] {
        new int[] {0, -1, 0, 0},
        new int[] {0, 0, 0, 0},
        new int[] {0, 0, 0, 0},
        new int[] {-1, 0, 0, -1},
    };

    void Start()
    {

        boardHeigth = board.Length;
        boardWidth = board[0].Length;

        for (int i = 0; i < boardWidth; i++) for (int j = 0; j < boardHeigth; j++)
        {
            if (board[boardHeigth-1-j][i] != -1) { 
                GameObject tmp = GameObject.Instantiate(cellPrefab, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), boardParent.transform);

                if (((i + j) % 2) == 0) tmp.GetComponent<SpriteRenderer>().color = cellFirstColor;
                else tmp.GetComponent<SpriteRenderer>().color = cellSecondColor;

                tmp.GetComponent<CellInputDetector>().Initialize(boardInputDetector, new int[] { j , i });
            }

        }
        
    }

    private void LateUpdate()
    {
        RecalculateCamera(); // Cuando este claro esto, se tiene que actualizar lo mínimo posible
    }

    void RecalculateCamera()
    {
        gameCamera.transform.position = new Vector3((boardWidth / 2.0f) - 0.5f, (boardHeigth / 2.0f) - 0.5f, -10);

        float screenAspectRatio = (Screen.width * 1.0f) / Screen.height;
        float boardAspectRatio = (boardWidth * 1.0f) / boardHeigth;
        
        if (boardAspectRatio > screenAspectRatio)
            gameCamera.GetComponent<Camera>().orthographicSize = ((boardWidth / 2.0f) / screenAspectRatio) + 1;
        else
            gameCamera.GetComponent<Camera>().orthographicSize = (boardHeigth / 2.0f) + 1;


        /*
            camara.GetComponent<Camera>().orthographicSize = 
        */

    }

}
