using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class GameController : MonoBehaviour
{

    [SerializeField] int altoTablero = 1, anchoTablero = 1;
    [SerializeField] GameObject padreTablero, camara, objetoCasilla;
    [SerializeField] Color colorCasillasBlancas, colorCasillasNegras;

    void Start()
    {
        
        for (int i = 0; i < anchoTablero; i++) for (int j = 0; j < altoTablero; j++)
        {

            GameObject tmp = GameObject.Instantiate(objetoCasilla, new Vector3(i, j, 0), new Quaternion(0, 0, 0, 0), padreTablero.transform);
            
            if (((i + j) % 2) == 0) tmp.GetComponent<SpriteRenderer>().color = colorCasillasBlancas;
            else tmp.GetComponent<SpriteRenderer>().color = colorCasillasNegras;

        }
        
    }

    private void LateUpdate()
    {
        RecalculateCamera(); // Cuando este claro esto, se tiene que actualizar lo mínimo posible
    }

    void RecalculateCamera()
    {
        camara.transform.position = new Vector3((anchoTablero / 2.0f) - 0.5f, (altoTablero / 2.0f) - 0.5f, -10);

        float screenAspectRatio = (Screen.width * 1.0f) / Screen.height;
        float boardAspectRatio = (anchoTablero * 1.0f) / altoTablero;
        
        if (boardAspectRatio > screenAspectRatio)
            camara.GetComponent<Camera>().orthographicSize = ((anchoTablero / 2.0f) / screenAspectRatio) + 1;
        else
            camara.GetComponent<Camera>().orthographicSize = (altoTablero / 2.0f) + 1;


        /*
            camara.GetComponent<Camera>().orthographicSize = 
        */

    }

}
