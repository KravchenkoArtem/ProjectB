using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class Level : MonoBehaviour
{
    // Target - Score, Obstacle, TargetCake. // Сonstraints - Moves, Timer.
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

    public int Score1Star;
    public int Score2Star;
    public int Score3Star;

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

    public virtual void OnMove() { }

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

    public Tile.CakeType RandomizeCakeType()
    {
        Tile.CakeType[] cakes = { Tile.CakeType.BIGCAKE, Tile.CakeType.BLACKCAKE, Tile.CakeType.CROISSANTCAKE, Tile.CakeType.PINKCAKE, Tile.CakeType.REDCAKE, Tile.CakeType.WHITECAKE, Tile.CakeType.YELLOWCAKE };
        Random random = new Random();
        return cakes[random.Next(cakes.Length)];
    }
}
