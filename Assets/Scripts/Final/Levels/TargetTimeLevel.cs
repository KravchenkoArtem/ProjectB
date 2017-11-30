using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetTimeLevel : Level
{
    public Tile.CakeType cakeType;
    [SerializeField]
    private int TargetCakesLeft = 10;
    [SerializeField]
    public int NumTargetLeft { get { return TargetCakesLeft; } set { TargetCakesLeft = Mathf.Clamp(value, 0, TargetCakesLeft); } }

    public int timeInSeconds;
    private float timer;
    private bool timeOut = false;

    private void Start()
    {
        type = LevelType.TARGETTIMER;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(NumTargetLeft);
        hud.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds / 60, timeInSeconds % 60));
        hud.SetTargetCake(cakeType);
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        if (cakeType == tile.Cake)
        {
            NumTargetLeft--;
            hud.SetTarget(NumTargetLeft);
        }
    }

    public override void OnMove()
    {
        timeInSeconds -= 10;
    }

    private void Update()
    {
        if (!timeOut)
        {
            timer += Time.deltaTime;
            hud.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((timeInSeconds - timer) / 60, 0), (int)Mathf.Max((timeInSeconds - timer) % 60, 0)));

            if (NumTargetLeft <= 0)
            {
                currentScore += 30 * (int)(timeInSeconds - timer);
                hud.SetScore(currentScore);
                GameWin();
            }

            if (timeInSeconds - timer <= 0 && NumTargetLeft > 0)
            {
                GameLose();
            }
        }
    }
}
