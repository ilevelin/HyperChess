using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PieceLibraryObject : MonoBehaviour
{
    PieceLibraryCoordinator coordinator;
    string id;
    [SerializeField] SpriteRenderer pieceIcon;
    [SerializeField] TextMeshPro pieceName, pieceVersion, pieceAuthor;

    public void LoadPiece(string id, Sprite icon, string name, string version, string author, PieceLibraryCoordinator coord)
    {
        this.id = id;
        pieceIcon.sprite = icon;
        pieceName.text = name;
        pieceVersion.text = version;
        coordinator = coord;
        if (author.Length == 0)
            pieceAuthor.text = author;
        else
            pieceAuthor.text = "by " + author;
    }

    private void OnMouseDown()
    {
        // TODO en un futuro cuando se muestre algo mas que la lista.
    }

}
