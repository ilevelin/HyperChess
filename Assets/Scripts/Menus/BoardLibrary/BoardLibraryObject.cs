using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardLibraryObject : MonoBehaviour
{
    [SerializeField] SpriteRenderer boardIcon;
    [SerializeField] TextMeshPro boardName, boardVersion, boardAuthor;
    BoardLibraryCoordinator coordinator;

    public void LoadBoard(Sprite icon, string name, string version, string author, BoardLibraryCoordinator coord)
    {
        boardIcon.sprite = icon;
        boardName.text = name;
        boardVersion.text = version;
        coordinator = coord;
        if (author.Length == 0)
            boardAuthor.text = author;
        else
            boardAuthor.text = "by " + author;
    }

    private void OnMouseDown()
    {
        coordinator.SelectElement(boardName.text);
    }

}
