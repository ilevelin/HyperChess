using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class BoardGenerator2DSquare : MonoBehaviour
{

    [SerializeField] int boardHeigth = 1, boardWidth = 1;
    [SerializeField] GameObject boardParent, gameCamera, cellPrefab;
    [SerializeField] Color cellFirstColor, cellSecondColor;

    void Start()
    {
        
        for (int i = 0; i < boardWidth; i++) for (int j = 0; j < boardHeigth; j++)
        {

            GameObject tmp = GameObject.Instantiate(cellPrefab, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), boardParent.transform);
            
            if (((i + j) % 2) == 0) tmp.GetComponent<SpriteRenderer>().color = cellFirstColor;
            else tmp.GetComponent<SpriteRenderer>().color = cellSecondColor;

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
