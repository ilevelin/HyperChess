using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PromotionOption : MonoBehaviour
{
    PromotionController promotionController;
    char pieceChar;
    Image image;

    public void Initializate(PromotionController pc, char c, Sprite s)
    {
        image = GetComponent<Image>();

        promotionController = pc;
        pieceChar = c;
        image.sprite = s;
    }

    public void Select()
    {
        promotionController.SelectedPromotion(pieceChar);
    }
}
