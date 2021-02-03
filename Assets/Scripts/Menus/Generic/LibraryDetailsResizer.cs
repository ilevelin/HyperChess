using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibraryDetailsResizer : MonoBehaviour
{
    private const float screenRatio = 0.4f;
    RectTransform rectTransform;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.sizeDelta = new Vector2(Screen.width * screenRatio, rectTransform.sizeDelta.y);
    }
}
