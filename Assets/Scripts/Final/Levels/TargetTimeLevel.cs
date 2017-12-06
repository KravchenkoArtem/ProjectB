using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class TargetTimeLevel : Level
{
    private Tile.CakeType cakeType;
    [SerializeField]
    private int TargetCakesLeft = 10;
    [SerializeField]
    public int NumTargetLeft { get { return TargetCakesLeft; } set { TargetCakesLeft = Mathf.Clamp(value, 0, TargetCakesLeft); } }

    public int timeInSeconds;
    private float timer;

    private void Start()
    {
        type = LevelType.TARGETTIMER;

        cakeType = RandomizeCakeType();

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

    public override void OnBombDetonate()
    {
        timeInSeconds -= 10;
    }

    private void Update()
    {
        if (!TimeOut)
        {
            timer += Time.deltaTime;
            hud.SetRemaining(string.Format("{0}:{1:00}", (int)Mathf.Max((timeInSeconds - timer) / 60, 0), (int)Mathf.Max((timeInSeconds - timer) % 60, 0)));

            if (NumTargetLeft <= 0)
            {
                currentScore += 10 * ((int)(timeInSeconds - timer) / 60);
                hud.SetScore(currentScore);
                GameWin();
                TimeOut = true;
            }

            if (timeInSeconds - timer <= 0 && NumTargetLeft > 0)
            {
                GameLose();
            }
        }
    }

    
}
