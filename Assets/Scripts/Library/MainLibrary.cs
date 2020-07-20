using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BoardType
{
    Square2D
}

public class MainLibrary : MonoBehaviour
{
    Dictionary<string, PieceElement> pieceLibrary;
    Dictionary<string, BoardElement> boardLibrary;

    void Start()
    {
        GameObject.DontDestroyOnLoad(gameObject);
    }

    public PieceElement GetPiece(string id)
    {
        PieceElement element;
        pieceLibrary.TryGetValue(id, out element);
        return element;
    }

    public BoardElement GetBoard(string id)
    {
        BoardElement element;
        boardLibrary.TryGetValue(id, out element);
        return element;
    }
}
