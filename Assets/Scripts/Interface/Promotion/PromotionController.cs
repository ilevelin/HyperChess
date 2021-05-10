using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PromotionController : MonoBehaviour
{
    Dictionary<char, Sprite> posiblePromotions = new Dictionary<char, Sprite>();
    [SerializeField] GameObject rowContainer;
    [SerializeField] GameObject blurGameObject;
    [SerializeField] GameObject rowPrefab;
    [SerializeField] GameObject piecePrefab;
    [SerializeField] RectTransform backgoundImage;
    [SerializeField] RectTransform promotionOptionsObject;
    [SerializeField] Sprite placeholderSprite;
    BoardCoordinator boardCoordinator;
    int counter = 0;
    bool showing = true;

    public void Initialize()
    {
        boardCoordinator = GameObject.FindGameObjectWithTag("BoardCoordinator").GetComponent<BoardCoordinator>();
        posiblePromotions = boardCoordinator.GetPromotablePieces();

        int maxh = Mathf.FloorToInt(Screen.height * 0.8f);
        int maxw = Mathf.FloorToInt(Screen.width * 0.8f);

        int squaresize = (maxw / 10) + 10;

        int rows = 1;
        while (Mathf.CeilToInt(posiblePromotions.Keys.Count / (rows * 1f)) * squaresize > maxw) rows++;

        int piecesPerRow = Mathf.CeilToInt(posiblePromotions.Keys.Count / (rows * 1f));
        int w = piecesPerRow * squaresize;
        int h = rows * squaresize;

        promotionOptionsObject.sizeDelta = new Vector2(w, h);
        backgoundImage.sizeDelta = new Vector2(h, Screen.width);

        foreach (Transform child in rowContainer.transform)
            GameObject.Destroy(child.gameObject);

        char[] pieceChars = posiblePromotions.Keys.ToArray();
        int j = 0;
        RectTransform row, piece;
        for (int i = 0; i < rows; i++)
        {
            row = GameObject.Instantiate(rowPrefab, rowContainer.transform).GetComponent<RectTransform>();
            row.offsetMax = new Vector2(0, -(5 + ((squaresize) * i))); // Top-Right corner
            row.offsetMin = new Vector2(0, 5 - ((squaresize) * (i + 1))); // Bot-Left corner

            for (; (j < posiblePromotions.Keys.Count) && (j < piecesPerRow * (i + 1)); j++)
            {
                piece = GameObject.Instantiate(piecePrefab, row.transform).GetComponent<RectTransform>();
                piece.localScale = new Vector3((squaresize - 10) / 100f, (squaresize - 10) / 100f);
                piece.anchoredPosition = new Vector3(5 + (squaresize * (j - (i * piecesPerRow))), 0);

                Sprite promotionSprite;
                posiblePromotions.TryGetValue(pieceChars[j], out promotionSprite);
                piece.gameObject.GetComponent<PromotionOption>().Initializate(this, pieceChars[j], promotionSprite);
            }
        }

        Hide();
    }

    public void PromotePiece()
    {
        if (!showing) Show();
    }

    public void Show()
    {
        rowContainer.SetActive(true);
        blurGameObject.SetActive(true);
        backgoundImage.gameObject.SetActive(true);
        showing = true;
    }

    public void Hide()
    {
        rowContainer.SetActive(false);
        blurGameObject.SetActive(false);
        backgoundImage.gameObject.SetActive(false);
        showing = false;
    }

    public void SelectedPromotion(char c)
    {
        Hide();
        boardCoordinator.AfterPromotionEndTurn(c);
    }
}
