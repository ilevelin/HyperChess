using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameData : MonoBehaviour
{
    public string boardID { get; private set; }
    public List<string> playerNames { get; private set; }
    public List<int> baseTimes { get; private set; }
    public List<int> incements { get; private set; }
    public List<int> delays { get; private set; }

    public void Initialize(string id, List<string> pNames, List<int> bTimes, List<int> incr, List<int> dels)
    {
        boardID = id;
        playerNames = pNames;
        baseTimes = bTimes;
        incements = incr;
        delays = dels;

        GameObject.DontDestroyOnLoad(gameObject);
    }
}
