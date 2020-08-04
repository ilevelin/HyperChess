using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PieceLibraryObject : MonoBehaviour
{

    [SerializeField] SpriteRenderer pieceIcon;
    [SerializeField] TextMeshPro pieceName, pieceVersion, pieceAuthor;

    public void LoadPiece(Sprite icon, string name, string version, string author)
    {
        pieceIcon.sprite = icon;
        pieceName.text = name;
        pieceVersion.text = version;
        if (author.Length == 0)
            pieceAuthor.text = author;
        else
            pieceAuthor.text = "by " + author;
    }

}
