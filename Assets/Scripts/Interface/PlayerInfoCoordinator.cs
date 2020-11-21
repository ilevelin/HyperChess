using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoCoordinator : MonoBehaviour
{
    [SerializeField] GameObject playerInfoPrefab, playerTurnArrow;
    List<PlayerInfoController> controllers = new List<PlayerInfoController>();
    List<PlayerInfo> players = new List<PlayerInfo>();
    List<int> times = new List<int>();
    BoardCoordinator boardCoordinator;
    public int turn { private set; get; } = 0;
    public int lastTurn { private set; get; }
    int delay = 0;

    bool ready = false;

    public int GetPlayerAmmount()
    {
        return players.Count;
    }

    public void AddPlayer(int startingTime, int incrementTime, int delayTime, string name, Color color, int team)
    {
        if (players.Count == 0) delay = delayTime;
        players.Add(new PlayerInfo(startingTime, incrementTime, delayTime, name, color, team));
        times.Add(startingTime);
        GameObject tmp = GameObject.Instantiate(playerInfoPrefab, new Vector3((Screen.width) / 2, (Screen.height)-(10 + (50 * controllers.Count))), new Quaternion(), transform);
        controllers.Add(tmp.GetComponent<PlayerInfoController>());
        controllers[controllers.Count - 1].Initialize(color, name, startingTime);
    }

    public void AddPlayer(int startingTime, int incrementTime, string name, Color color, int team)
    {
        players.Add(new PlayerInfo(startingTime, incrementTime, 0, name, color, team));
        times.Add(startingTime);
        GameObject tmp = GameObject.Instantiate(playerInfoPrefab, new Vector3((Screen.width) / 2, (Screen.height) - (10 + (50 * controllers.Count))), new Quaternion(), transform);
        controllers.Add(tmp.GetComponent<PlayerInfoController>());
        controllers[controllers.Count - 1].Initialize(color, name, startingTime);
    }

    public void AddPlayer(PlayerInfo player)
    {
        if (players.Count == 0) delay = player.delayTime;
        players.Add(player);
        times.Add(player.startingTime);
        GameObject tmp = GameObject.Instantiate(playerInfoPrefab, new Vector3((Screen.width) / 2, (Screen.height) - (10 + (50 * controllers.Count))), new Quaternion(), transform);
        controllers.Add(tmp.GetComponent<PlayerInfoController>());
        controllers[controllers.Count - 1].Initialize(player.color, player.name, player.startingTime);
    }
    
    internal void Initialize(BoardCoordinator bc)
    {
        boardCoordinator = bc;
        ready = true;
    }

    void FixedUpdate()
    {
        if (!ready) return;
        if (players[turn].alive)
        {
            if (delay > 0)
            {
                if (delay >= Time.deltaTime)
                    delay -= (int)(Time.deltaTime * 1000);
                else
                {
                    int tmp = (int)(Time.deltaTime * 1000);
                    tmp -= delay;
                    delay = 0;
                    times[turn] -= tmp;
                    controllers[turn].UpdateTimer(times[turn]);
                }
            }
            else
            {
                times[turn] -= (int)(Time.deltaTime * 1000);
                controllers[turn].UpdateTimer(times[turn]);
            }

            if (times[turn] <= 0)
            {
                players[turn].alive = false;
                controllers[turn].EliminatePlayer();
                NextTurn(false);
            }
        }
        else
            NextTurn(false);
    }

    private void Update()
    {
        if (!ready) return;
        playerTurnArrow.transform.position = Vector3.Lerp(playerTurnArrow.transform.position, new Vector3(0, (Screen.height) - (10 + (50 * turn))), 0.1f);
        playerTurnArrow.GetComponent<Image>().color = Color.Lerp(playerTurnArrow.GetComponent<Image>().color, players[turn].color, 0.1f);
    }

    public void NextTurn(bool incrementLastPlayer)
    {
        if (incrementLastPlayer)
        {
            times[turn] += players[turn].incrementTime;
            controllers[turn].UpdateTimer(times[turn]);
        }

        lastTurn = turn;
        int tmp = turn;
        bool able = true;

        while (able)
        {
            tmp++;
            if (tmp >= players.Count) tmp = 0;

            if (tmp == turn)
            {
                delay = players[turn].delayTime;
                able = false;
            }
            else if (players[tmp].alive)
            {
                turn = tmp;
                delay = players[turn].delayTime;
                able = false;
            }
        }

        List<int> tmpAdvs = new List<int>();
        int minScore = -1;

        for (int i = 1; i <= players.Count; i++)
        {
            tmpAdvs.Add(boardCoordinator.GetScoreOfPlayer(i));
            if (players[i - 1].alive)
            {
                if (minScore == -1) minScore = i - 1;
                else if (tmpAdvs[i - 1] < tmpAdvs[minScore]) minScore = i - 1;
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            if (tmpAdvs[i] == 0)
            {
                if (players[i].alive)
                {
                    players[i].alive = false;
                    controllers[i].EliminatePlayer();
                }
            }
            else if (players[i].alive)
                controllers[i].UpdateAdvantaje(tmpAdvs[i] - tmpAdvs[minScore]);
        }

    }
}
