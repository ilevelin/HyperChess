using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class BoardLibraryCoordinator : MonoBehaviour
{
    private string selectedElementID = null;
    private MainLibrary mainLibrary;
    [SerializeField] private GameConfigurationCoordinator playerInfoCoordinator;
    [SerializeField] private Image boardImage;
    [SerializeField] private GameObject placeholderText;
    [SerializeField] private TextMeshProUGUI boardTitle;
    [SerializeField] private GameObject gameDataPrefab;
    [SerializeField] private GameObject detailsTab;

    private void Start()
    {
        try
        {
            mainLibrary = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();
            selectedElementID = "";
            detailsTab.SetActive(false);
        }
        catch(Exception e)
        {
            Debug.LogError($"Loading scene without library? : {e}");
        }
    }

    public void SelectElement(string elementID)
    {
        placeholderText.SetActive(false);
        selectedElementID = elementID;
        BoardElement boardElement = GetSelectedElement();
        boardImage.sprite = boardElement.image;
        boardTitle.text = elementID;
        detailsTab.SetActive(true);
    }

    public string GetSelectedID()
    {
        return selectedElementID;
    }

    public BoardElement GetSelectedElement()
    {
        try
        {
            BoardElement result;
            mainLibrary.boardLibrary.TryGetValue(selectedElementID, out result);
            return result;
        }
        catch
        {
            return null;
        }
    }

    public void LaunchGame()
    {
        playerInfoCoordinator.Show(selectedElementID);
    }
}
