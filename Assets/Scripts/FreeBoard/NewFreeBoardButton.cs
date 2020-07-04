using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NewFreeBoardButton : MonoBehaviour
{
    GameObject boardGenerator;
    [SerializeField] TextMeshProUGUI width, height;

    public void GenerateNewBoard()
    {
        BoardGenerator2DSquare generator = boardGenerator.GetComponent<BoardGenerator2DSquare>();

        generator.GenerateNewBoard(int.Parse(height.text), int.Parse(width.text));

    }
}
