using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PromotionController : MonoBehaviour
{
    [SerializeField] char[] posiblePromotions;
    [SerializeField] GameObject rowContainer;
    [SerializeField] GameObject blurGameObject;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject piecePrefab;
    [SerializeField] RectTransform background;
    [SerializeField] Sprite placeholderSprite;
    BoardCoordinator boardCoordinator;
    int counter = 0;
    bool showing = true;
    bool hasBeenSelected = false;
    char selectedPromotion = '_';

    void Start()
    {
        int maxh = Mathf.FloorToInt(Screen.height * 0.8f);
        int maxw = Mathf.FloorToInt(Screen.width * 0.8f);

        int squaresize = (maxw / 10) + 10;

        int rows = 1;
        while (Mathf.CeilToInt(posiblePromotions.Length / (rows * 1f)) * squaresize > maxw) rows++;

        int piecesPerRow = Mathf.CeilToInt(posiblePromotions.Length / (rows * 1f));
        int w = piecesPerRow * squaresize;
        int h = rows * squaresize;

        background.sizeDelta = new Vector2(w, h);

        foreach (Transform child in rowContainer.transform)
            GameObject.Destroy(child.gameObject);

        int j = 0;
        RectTransform row, piece;
        for (int i = 0; i < rows; i++)
        {
            row = GameObject.Instantiate(rowPrefab, rowContainer.transform).GetComponent<RectTransform>();
            row.offsetMax = new Vector2(0, -(5 + ((squaresize) * i))); // Top-Right corner
            row.offsetMin = new Vector2(0, 5 - ((squaresize) * (i + 1))); // Bot-Left corner
            for (; (j < posiblePromotions.Length) && (j < piecesPerRow * (i + 1)); j++)
            {
                piece = GameObject.Instantiate(piecePrefab, row.transform).GetComponent<RectTransform>();
                piece.localScale = new Vector3((squaresize - 10) / 100f, (squaresize - 10) / 100f);
                piece.anchoredPosition = new Vector3(5 + (squaresize * (j - (i * piecesPerRow))), 0);
                piece.gameObject.GetComponent<PromotionOption>().Initializate(this, posiblePromotions[j]);
            }
        }

        Hide();
    }

    public bool PromotePiece(BoardCoordinator coordinator)
    {
        boardCoordinator = coordinator;
        if (hasBeenSelected) return true;

        if (!showing) Show();
        return false;
    }
    /*
    public char GetPromotedPiece()
    {
        hasBeenSelected = false;
        return selectedPromotion;
    }
    */
    public void Show()
    {
        rowContainer.SetActive(true);
        blurGameObject.SetActive(true);
        showing = true;
    }

    public void Hide()
    {
        rowContainer.SetActive(false);
        blurGameObject.SetActive(false);
        showing = false;
    }

    public void SelectedPromotion(char c)
    {
        /*hasBeenSelected = true;
        selectedPromotion = c;*/
        Hide();
        boardCoordinator.AfterPromotionEndTurn(c);
    }
}
