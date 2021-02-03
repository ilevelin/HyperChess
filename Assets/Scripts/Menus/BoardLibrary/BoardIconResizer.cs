using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardIconResizer : MonoBehaviour
{
    private const float verticalLimit = .3f;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }
    
    void Update() // Cambiar por Start
    {
        float objectiveX = Mathf.Min(630f, (Screen.width * .44f) - 40f);
        float objectiveY = Mathf.Min((objectiveX * 9f) / 16f, Screen.height * verticalLimit);
        objectiveX = (objectiveY * 16f) / 9f;

        rectTransform.sizeDelta = new Vector2(objectiveX, objectiveY);
    }
}
