using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameConfigurationCoordinator : MonoBehaviour
{
    MainLibrary library;
    string boardID;
    BoardType boardType;
    List<GameConfigurationController> playerInfoList;
    [SerializeField] private GameObject playerConfigPrefab, loadGameDataPrefab, playerConfigListObject, interfaceObject, titleBarObject;
    [SerializeField] private Button playButton;

    private void Update()
    {
        if (interfaceObject.activeSelf)
        {
            foreach (GameConfigurationController playerInfo in playerInfoList)
                if (!playerInfo.canStart)
                {
                    playButton.interactable = false;
                    return;
                }
            playButton.interactable = true;
        }
    }

    void Start()
    {
        playerInfoList = new List<GameConfigurationController>();
        library = GameObject.FindGameObjectWithTag("MainLibrary").GetComponent<MainLibrary>();
        Hide();
    }
    
    public void Hide()
    {
        foreach (Transform child in playerConfigListObject.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
        interfaceObject.SetActive(false);
    }

    public void Show(string boardID)
    {
        playerInfoList.Clear();

        BoardElement board;
        library.boardLibrary.TryGetValue(boardID, out board);
        int i = board.players.Count;
        titleBarObject.transform.localPosition = new Vector3(0, 40 * i, 0);
        interfaceObject.transform.localPosition = new Vector3(0, -20 * i, 0);
        foreach (PlayerImport player in board.players)
        {
            i--;
            GameConfigurationController tmp = GameObject.Instantiate(playerConfigPrefab, new Vector3(), new Quaternion(), playerConfigListObject.transform).GetComponent<GameConfigurationController>();
            tmp.Initialize(player.interfaceColor ?? default(Color));
            tmp.transform.localPosition = new Vector3(-30, 40*i, 0);
            playerInfoList.Add(tmp);
        }

        this.boardID = boardID;
        this.boardType = board.boardType;
        interfaceObject.SetActive(true);
    }

    public void LoadGame()
    {
        List<string> names = new List<string>();
        List<int> times = new List<int>();
        List<int> increments = new List<int>();
        List<int> delays = new List<int>();

        foreach (GameConfigurationController playerInfo in playerInfoList)
        {
            names.Add(playerInfo.playerName);
            times.Add(playerInfo.initialTime*1000);
            increments.Add(playerInfo.addedTime*1000);
            delays.Add(playerInfo.delayTime*1000);
        }
        
        LoadGameData gameData = GameObject.Instantiate(loadGameDataPrefab).GetComponent<LoadGameData>();
        gameData.Initialize(boardID, names, times, increments, delays);

        library.gameObject.GetComponent<SceneSwitcher>().LoadScene(boardType);
    }
}
