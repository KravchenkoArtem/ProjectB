using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public enum LevelType
    {
        TIMER,
        OBSTACLE,
        MOVES,
        TARGETCAKE
    };

    public Grid grid;
    public HUD hud;

    public int score1Star;
    public int score2Star;
    public int score3Star;

    protected LevelType type;
    public LevelType Type
    {
        get { return type; }
    }

    protected int currentScore;
    protected bool didWin;

    private void Start()
    {
        hud.SetScore(currentScore);
    }

    public void GameWin()
    {
        grid.GameOver = true;
        didWin = true;
        StartCoroutine(WaitForGridFill());
    }

    public void GameLose()
    {
        grid.GameOver = true;
        didWin = false;
        StartCoroutine(WaitForGridFill());
    }

    public virtual void OnMove()
    {

    }

    public virtual void OnTileCleared(Tile tile)
    {
        currentScore += tile.score;
        hud.SetScore(currentScore);
    }
    protected virtual IEnumerator WaitForGridFill()
    {
        while (grid.IsFilling)
        {
            yield return 0;
        }

        if (didWin)
        {
            hud.OnGameWin(currentScore);
        }
        else
        {
            hud.OnGameLose(currentScore);
        }
    }
}
