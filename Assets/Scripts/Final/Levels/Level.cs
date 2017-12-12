using System.Collections;
using UnityEngine;
using Random = System.Random;

public class Level : MonoBehaviour
{
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
    protected XpBarScript xp;

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
    public bool TimeOut = false;

    private void Awake()
    {
        grid = GameObject.FindGameObjectWithTag("Grid").GetComponent<Grid>();
        hud = GameObject.FindGameObjectWithTag("HUD").GetComponent<HUD>();
        xp = hud.gameObject.transform.GetChild(3).GetComponent<XpBarScript>();
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

    public virtual void OnBombDetonate() { }

    public virtual void OnTileCleared(Tile tile)
    {
        xp.AddXp(tile.score / 2);
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
        Tile.CakeType[] cakes = { Tile.CakeType.BIGCAKE, Tile.CakeType.BLACKCAKE,
            Tile.CakeType.CROISSANTCAKE, Tile.CakeType.PINKCAKE, Tile.CakeType.REDCAKE,
            Tile.CakeType.WHITECAKE, Tile.CakeType.YELLOWCAKE };
        Random random = new Random();
        return cakes[random.Next(cakes.Length)];
    }
}
