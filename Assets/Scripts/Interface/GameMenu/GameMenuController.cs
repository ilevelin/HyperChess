using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] menuComponents;
    [SerializeField] private GameObject resignButton, confirmButton;
    private SceneSwitcher sceneSwitcher;
    private BoardCoordinator coordinator;

    private void Start()
    {
        sceneSwitcher = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<SceneSwitcher>();
        coordinator = GameObject.FindGameObjectWithTag("BoardCoordinator").GetComponent<BoardCoordinator>();
    }

    public void ContinueGame()
    {
        foreach (GameObject component in menuComponents)
            component.SetActive(false);

        resignButton.SetActive(true);
        confirmButton.SetActive(false);
    }

    public void Resign(int fase)
    {
        switch (fase)
        {
            case 1:
                resignButton.SetActive(false);
                confirmButton.SetActive(true);
                break;
            case 2:
                ContinueGame();
                coordinator.PlayerResign();
                break;
            default:
                throw new Exception("What the fuck are you doing here?");
        }
    }

    public void ExitGame()
    {
        sceneSwitcher.LoadScene(GameScene.MAIN_MENU);
    }

    public void ShowMenu()
    {
        foreach (GameObject component in menuComponents)
            component.SetActive(true);
    }
}
