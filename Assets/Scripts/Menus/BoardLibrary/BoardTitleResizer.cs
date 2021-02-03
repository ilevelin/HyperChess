using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTitleResizer : MonoBehaviour
{
    private const float heightRatio = 1f / 3.5f;
    RectTransform rectTransform, parentRectTransform, tabRectTransform;

    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        tabRectTransform = rectTransform.parent.parent.GetComponent<RectTransform>();
    }
    
    void Update()
    {
        rectTransform.sizeDelta = new Vector2(tabRectTransform.sizeDelta.x - 30, Mathf.Min(parentRectTransform.sizeDelta.y * heightRatio, 100f));
    }
}
