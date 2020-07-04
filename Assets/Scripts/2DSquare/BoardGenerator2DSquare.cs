using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BoardGenerator2DSquare : MonoBehaviour
{

    private int boardHeight = 1, boardWidth = 1;
    [SerializeField] GameObject boardParent, gameCamera, cellPrefab, boardInputDetector, samplePiece;
    [SerializeField] Color cellFirstColor, cellSecondColor;
    int[][] board = new int[][] {
        new int[] {0, 0, 0, 0, 0, 0, 0, 2},
        new int[] {2, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, -1, -1, 0, 0, 0},
        new int[] {0, 0, 0, -1, -1, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 0},
        new int[] {0, 0, 0, 0, 0, 0, 0, 1},
        new int[] {1, 0, 0, 0, 0, 0, 0, 0},
    };

    void Start()
    {
        RenderBoard();
    }

    public void GenerateNewBoard(int height, int width)
    {
        board = new int[height][];
        for (int i = 0; i < height; i++)
        {
            board[i] = new int[width];
            for (int j = 0; j < width; j++)
            {
                board[i][j] = 0;
            }
        }
        RenderBoard();
    }

    private void LateUpdate()
    {
        RecalculateCamera(); // Cuando este claro esto, se tiene que actualizar lo mínimo posible
    }

    void RecalculateCamera()
    {
        gameCamera.transform.position = new Vector3((boardWidth / 2.0f) - 0.5f, (boardHeight / 2.0f) - 0.5f, -10);

        float screenAspectRatio = (Screen.width * 1.0f) / Screen.height;
        float boardAspectRatio = (boardWidth * 1.0f) / boardHeight;
        
        if (boardAspectRatio > screenAspectRatio)
            gameCamera.GetComponent<Camera>().orthographicSize = ((boardWidth / 2.0f) / screenAspectRatio) + 1;
        else
            gameCamera.GetComponent<Camera>().orthographicSize = (boardHeight / 2.0f) + 1;

    }

    private void RenderBoard()
    {
        for (int i = boardParent.transform.childCount; i > 0; i--)
            GameObject.Destroy(boardParent.transform.GetChild(0));

        boardHeight = board.Length;
        boardWidth = board[0].Length;

        for (int i = 0; i < boardWidth; i++) for (int j = 0; j < boardHeight; j++)
            {
                if (board[i][j] != -1)
                {
                    GameObject tmp = GameObject.Instantiate(cellPrefab, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), boardParent.transform);

                    if (((i + j) % 2) == 0) tmp.GetComponent<SpriteRenderer>().color = cellFirstColor;
                    else tmp.GetComponent<SpriteRenderer>().color = cellSecondColor;

                    tmp.GetComponent<CellInputDetector>().Initialize(boardInputDetector, new int[] { i, j });

                    if (board[boardHeight - 1 - j][i] != 0)
                    {
                        tmp = GameObject.Instantiate(samplePiece, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0));
                        Color tmpColor;
                        if (board[boardHeight - 1 - j][i] == 1) tmpColor = new Color(1.0f, 0.5f, 0.5f);
                        else tmpColor = new Color(0.5f, 0.5f, 1.0f);
                        tmp.GetComponent<Piece2DSquare>().Initialize(new int[] { i, j }, boardInputDetector, board[boardHeight - 1 - j][i], board[boardHeight - 1 - j][i]+100, tmpColor);
                    }

                }

            }
    }

}
