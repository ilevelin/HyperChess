using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadScene(GameScene scene)
    {
        if (scene == GameScene.EXIT_PROGRAM)
        {
            /*if (Application.isEditor)
                UnityEditor.EditorApplication.isPlaying = false;
            else*/
                Application.Quit();
        }
        SceneManager.LoadScene((int)scene);
    }

    public void LoadScene(BoardType type)
    {
        GameScene boardScene;

        switch (type)
        {
            case BoardType.Square2D:
                boardScene = GameScene.GAME_SQUARE_2D;
                break;
            default:
                return;
        }
        
        SceneManager.LoadScene((int)boardScene);
    }
}

public enum GameScene
{
    EXIT_PROGRAM = -1,
    MAIN_MENU = 0,
    PIECE_LIBRARY = 1,
    BOARD_LIBRARY = 2,
    GAME_SQUARE_2D = 3,
}
