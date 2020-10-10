﻿using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BoardGeneratorSquare2D : MonoBehaviour
{

    private int boardHeight = 1, boardWidth = 1;
    private MainLibrary mainLibrary;
    [SerializeField] GameObject boardParent, gameCamera, cellPrefab, boardInputDetector, boardCoordinator, samplePiece, playerInterface;
    [SerializeField] Color cellFirstColor, cellSecondColor;
    List<Tuple<char, string, int>> importList;
    string[][] board;
    List<PlayerInfo> playerList = new List<PlayerInfo>();

    Dictionary<char, List<Move>> pieces = new Dictionary<char, List<Move>>();
    Dictionary<char, Sprite> sprites = new Dictionary<char, Sprite>();
    Dictionary<char, int> values = new Dictionary<char, int>();

    public void StartAfterCoordinator()
    {
        mainLibrary = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();
        /*
        foreach (Tuple<char, string, int> import in importList)
        {
            PieceElement importedPiece;
            if (mainLibrary.pieceLibrary.TryGetValue(import.Item2, out importedPiece))
            {
                pieces.Add(import.Item1, importedPiece.moves);
                sprites.Add(import.Item1, importedPiece.image);
                values.Add(import.Item1, import.Item3);
            }
        }
        */
        LoadBoardFromLibrary("TestBoard");

        PlayerInfoCoordinator playerCoordinator = playerInterface.GetComponent<PlayerInfoCoordinator>();
        foreach(PlayerInfo player in playerList)
            playerCoordinator.AddPlayer(player);

        RenderBoard();
    }

    public void LoadBoardFromLibrary(string boardName)
    {
        try
        {
            BoardElement boardToLoad;
            mainLibrary.boardLibrary.TryGetValue(boardName, out boardToLoad);

            if (boardToLoad.boardType == BoardType.Square2D)
            {
                foreach (char character in boardToLoad.pieceIDs.Keys.ToArray())
                {
                    PieceImport pieceImport;
                    PieceElement pieceElement;
                    boardToLoad.pieceIDs.TryGetValue(character, out pieceImport);
                    mainLibrary.pieceLibrary.TryGetValue(pieceImport.ID, out pieceElement);
                    values.Add(character, pieceImport.value);
                    sprites.Add(character, pieceElement.image);
                    pieces.Add(character, pieceElement.moves);
                }

                board = ((BoardSquare2D)boardToLoad.initialState).board;

                for (int i = 0; i < boardToLoad.players.Count; i++)
                {
                    playerList.Add(new PlayerInfo(
                        1000 * 60 * 5, 5000, 0,
                        "Player " + i,
                        boardToLoad.players[i].color ?? default(Color),
                        i + 100
                        ));
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.ToString());
        }
    }

    public void GenerateNewBoard(int height, int width)
    {
        board = new string[height][];
        for (int i = 0; i < height; i++)
        {
            board[i] = new string[width];
            for (int j = 0; j < width; j++)
            {
                board[i][j] = "0";
            }
        }
        RenderBoard();
    }

    private void LateUpdate()
    {

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

        boardCoordinator.GetComponent<BoardCoordinator>().Initialize(new int[] { boardHeight, boardWidth });

        for (int i = 0; i < boardWidth; i++) for (int j = 0; j < boardHeight; j++)
            {
                if (!board[boardHeight - 1 - j][i].Equals("_"))
                {
                    GameObject tmp = GameObject.Instantiate(cellPrefab, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), boardParent.transform);

                    if (((i + j) % 2) == 0) tmp.GetComponent<SpriteRenderer>().color = cellFirstColor;
                    else tmp.GetComponent<SpriteRenderer>().color = cellSecondColor;

                    tmp.GetComponent<CellInputDetector>().Initialize(boardInputDetector, new int[] { i, j });

                    if (board[boardHeight - 1 - j][i] != "0")
                    {
                        char pieceChar = board[boardHeight - 1 - j][i][1];
                        int pieceOwner = int.Parse(board[boardHeight - 1 - j][i][0].ToString());
                        CreatePiece(new int[] { i, j }, pieceChar, pieceOwner);
                    }

                }
                else
                {
                    if (boardCoordinator.GetComponent<BoardCoordinator>() is BoardCoordinatorSquare2D)
                        ((BoardCoordinatorSquare2D)boardCoordinator.GetComponent<BoardCoordinator>()).existingCells[i][boardHeight - 1 - j] = false;
                }

            }

        RecalculateCamera();

        boardCoordinator.GetComponent<BoardCoordinator>().CheckMoves();
    }

    public PieceSquare2D CreatePiece(int[] cell, char piece, int player)
    {
        try
        {
            if (cell.Length == 2)
            {
                GameObject newPiece = GameObject.Instantiate(samplePiece, new Vector3(cell[0], cell[1], 0), new Quaternion(0, 0, 0, 0));

                List<Move> pieceMoves;
                int pieceValue;
                Sprite pieceSprite;
                pieces.TryGetValue(piece, out pieceMoves);
                values.TryGetValue(piece, out pieceValue);
                sprites.TryGetValue(piece, out pieceSprite);

                newPiece.GetComponent<PieceSquare2D>().Initialize(
                    new int[] { cell[0], cell[1] },
                    boardCoordinator,
                    player,
                    playerList[player - 1].team,
                    playerList[player - 1].color,
                    pieceMoves,
                    pieceValue,
                    piece,
                    pieceSprite
                    );

                return newPiece.GetComponent<PieceSquare2D>();
            }
            else return null;
        }
        catch
        {
            return null;
        }
    }

}
