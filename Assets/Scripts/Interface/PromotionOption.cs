using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionOption : MonoBehaviour
{
    PromotionController promotionController;
    char pieceChar;

    public void Initializate(PromotionController pc, char c)
    {
        promotionController = pc;
        pieceChar = c;
    }

    public void Select()
    {
        promotionController.SelectedPromotion(pieceChar);
    }
}
