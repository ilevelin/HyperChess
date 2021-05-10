using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoCoordinator : MonoBehaviour
{
    [SerializeField] GameObject playerInfoPrefab, playerTurnArrow;
    List<PlayerInfoController> controllers = new List<PlayerInfoController>();
    public List<PlayerInfo> players = new List<PlayerInfo>();
    List<int> times = new List<int>();
    List<bool> infiniteTimes = new List<bool>();
    BoardCoordinator boardCoordinator;
    public int turn { private set; get; } = 0;
    public int lastTurn { private set; get; }
    int delay = 0;

    bool ready = false;
    private bool gameRunning = true;

    public int GetPlayerAmmount()
    {
        return players.Count;
    }

    public void AddPlayer(int startingTime, int incrementTime, int delayTime, string name, Color color, int team)
    {
        AddPlayer(startingTime, incrementTime, delay, name, color, color, team);
    }

    public void AddPlayer(int startingTime, int incrementTime, string name, Color color, int team)
    {
        AddPlayer(startingTime, incrementTime, 0, name, color, color, team);
    }

    public void AddPlayer(int startingTime, int incrementTime, string name, Color color, Color interfaceColor, int team)
    {
        AddPlayer(startingTime, incrementTime, 0, name, color, interfaceColor, team);
    }

    public void AddPlayer(int startingTime, int incrementTime, int delayTime, string name, Color color, Color interfaceColor, int team)
    {
        AddPlayer(new PlayerInfo(startingTime, incrementTime, delayTime, name, color, interfaceColor, team));
    }

    public void AddPlayer(PlayerInfo player)
    {
        bool isInfinite = player.startingTime == 0 && player.incrementTime == 0 && player.delayTime == 0;
        if (players.Count == 0) delay = player.delayTime;
        players.Add(player);
        times.Add(player.startingTime);
        infiniteTimes.Add(isInfinite);
        GameObject tmp = GameObject.Instantiate(playerInfoPrefab, new Vector3((Screen.width) / 2, (Screen.height) - (10 + (50 * controllers.Count))), new Quaternion(), transform);
        controllers.Add(tmp.GetComponent<PlayerInfoController>());
        controllers[controllers.Count - 1].Initialize(player.interfaceColor, player.name, player.startingTime, isInfinite);
    }

    public void EliminatePlayer(int player)
    {
        players[player].alive = false;
        controllers[player].EliminatePlayer();

        List<int> aliveTeams = new List<int>();
        foreach (PlayerInfo playerInfo in players)
            if (playerInfo.alive)
                if (!aliveTeams.Contains(playerInfo.team))
                    aliveTeams.Add(playerInfo.team);

        if (aliveTeams.Count == 1)
        {
            turn = -1;
            gameRunning = false;
            boardCoordinator.EndGame(aliveTeams[0]);
        }
        else if (player == turn) boardCoordinator.EndTurn();
    }

    public void EliminateActualPlayer()
    {
        EliminatePlayer(turn);
    }

    internal void Initialize(BoardCoordinator bc)
    {
        boardCoordinator = bc;
        ready = true;
    }

    void FixedUpdate()
    {
        if (!ready || !gameRunning) return;
        if (infiniteTimes[turn]) return;
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
                EliminateActualPlayer();
        }
        else
            NextTurn(false);
    }

    private void Update()
    {
        if (!ready || !gameRunning) return;
        playerTurnArrow.transform.position = Vector3.Lerp(playerTurnArrow.transform.position, new Vector3(0, (Screen.height) - (10 + (50 * turn))), 0.1f);
        playerTurnArrow.GetComponent<Image>().color = Color.Lerp(playerTurnArrow.GetComponent<Image>().color, players[turn].interfaceColor, 0.1f);
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
