using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class BoardGeneratorSquare2D : MonoBehaviour
{

    private int boardHeight = 1, boardWidth = 1;
    private MainLibrary mainLibrary;
    [SerializeField] GameObject boardParent, gameCamera, cellPrefab, boardInputDetector, boardCoordinator, samplePiece, playerInterface;
    [SerializeField] Color cellFirstColor, cellSecondColor;
    List<Tuple<char, string, int>> importList = new List<Tuple<char, string, int>>()
    {
        new Tuple<char, string, int>('K', "ile.03", 50),
        new Tuple<char, string, int>('B', "ile.02", 3),
        new Tuple<char, string, int>('R', "ile.01", 5),
    };
    string[][] board = new string[][] {
        new string[] {"4K", "4R", "0", "4B", "_", "3B", "0", "3R", "3K"},
        new string[] {"4R", "0", "0", "0", "_", "0", "0", "0", "3R"},
        new string[] {"0", "0", "0", "0", "0", "0", "0", "0", "0"},
        new string[] {"4B", "0", "0", "0", "0", "0", "0", "0", "3B"},
        new string[] {"_", "_", "0", "0", "0", "0", "0", "_", "_"},
        new string[] {"1B", "0", "0", "0", "0", "0", "0", "0", "2B"},
        new string[] {"0", "0", "0", "0", "0", "0", "0", "0", "0"},
        new string[] {"1R", "0", "0", "0", "_", "0", "0", "0", "2R"},
        new string[] {"1K", "1R", "0", "1B", "_", "2B", "0", "2R", "2K"},
    };
    List<PlayerInfo> playerList = new List<PlayerInfo>();

    Dictionary<char, List<Move>> pieces = new Dictionary<char, List<Move>>();
    Dictionary<char, Sprite> sprites = new Dictionary<char, Sprite>();
    Dictionary<char, int> values = new Dictionary<char, int>();

    void Start()
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
        RenderBoard();

        PlayerInfoCoordinator playerCoordinator = playerInterface.GetComponent<PlayerInfoCoordinator>();
        foreach(PlayerInfo player in playerList)
            playerCoordinator.AddPlayer(player);
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
                        boardToLoad.players[i].color ?? default(Color)
                        ));
                }
                Debug.Log("Loaded Players = " + playerList.Count);
            }
        }
        catch
        {

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
        //RecalculateCamera(); // Cuando este claro esto, se tiene que actualizar lo mínimo posible
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
                        tmp = GameObject.Instantiate(samplePiece, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0));
                        Color tmpColor = playerList[int.Parse(board[boardHeight - 1 - j][i][0].ToString()) - 1].color;
                        List<Move> pieceMoves;
                        int pieceValue;
                        Sprite pieceSprite;
                        pieces.TryGetValue(board[boardHeight - 1 - j][i][1], out pieceMoves);
                        values.TryGetValue(board[boardHeight - 1 - j][i][1], out pieceValue);
                        sprites.TryGetValue(board[boardHeight - 1 - j][i][1], out pieceSprite);
                        tmp.GetComponent<PieceSquare2D>().Initialize(
                            new int[] { i, j }, boardCoordinator, 
                            int.Parse(board[boardHeight - 1 - j][i][0].ToString()), 
                            int.Parse(board[boardHeight - 1 - j][i][0].ToString())+100, 
                            tmpColor, 
                            pieceMoves, 
                            pieceValue,
                            pieceSprite
                            );
                    }

                }
                else
                {
                    if (boardCoordinator.GetComponent<BoardCoordinator>() is BoardCoordinatorSquare2D)
                        ((BoardCoordinatorSquare2D)boardCoordinator.GetComponent<BoardCoordinator>()).existingCells[boardHeight - 1 - j][i] = false;
                }

            }

        RecalculateCamera();

        boardCoordinator.GetComponent<BoardCoordinator>().CheckMoves();
    }

}
