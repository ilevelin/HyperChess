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
    [SerializeField] private Image boardImage;
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

        List<string> names = new List<string>();
        List<int> times = new List<int>();
        List<int> increments = new List<int>();
        List<int> delays = new List<int>();

        // Temporal //
        for (int i = 1; i <= GetSelectedElement().players.Count; i++)
        {
            names.Add($"JUGADOR-{i}");
            times.Add(60000 * i);
            increments.Add(1000 * i);
            delays.Add(1000 * i);
        }

        GameObject tmp = GameObject.Instantiate(gameDataPrefab);
        tmp.GetComponent<LoadGameData>().Initialize(
            selectedElementID,
            names,
            times,
            increments,
            delays
            );

        mainLibrary.gameObject.GetComponent<SceneSwitcher>().LoadScene(GetSelectedElement().boardType);
    }
}
