using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    // Target - Score,Obstacle,TargetCake // Сonstraints - Moves, Timer.
    public enum LevelType
    {
        OBSTACLETIMER,
        SCORETIMER,
        TARGETTIMER,
        OBSTACLEMOVES,
        SCOREMOVES,
        TARGETMOVES
    };

    protected Grid grid;
    protected HUD hud;

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

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
    }

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
