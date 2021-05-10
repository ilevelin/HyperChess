using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class VictoryScreenCoordinator : MonoBehaviour
{
    [SerializeField] GameObject victoryScreenObject;
    [SerializeField] TextMeshProUGUI winnerText;

    private void Start()
    {
        Hide();
    }

    public void EndGame(string winnerTeam)
    {
        winnerText.text = winnerTeam;
        Show();
    }
    
    public void Hide()
    {
        victoryScreenObject.SetActive(false);
    }

    public void Show()
    {
        victoryScreenObject.SetActive(true);
    }
}
