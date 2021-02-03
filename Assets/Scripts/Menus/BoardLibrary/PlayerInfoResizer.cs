using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoResizer : MonoBehaviour
{
    [SerializeField] private RectTransform parentRectTrans, iconRectTrans, titleRectTrans;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    
    void LateUpdate()
    {
        float objectiveX = parentRectTrans.sizeDelta.x - 50f;
        float objectiveY = Screen.height - iconRectTrans.sizeDelta.y - titleRectTrans.sizeDelta.y - 105f;

        rectTransform.sizeDelta = new Vector2(objectiveX, objectiveY);
    }
}
