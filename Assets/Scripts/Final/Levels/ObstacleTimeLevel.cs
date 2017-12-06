using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleTimeLevel : Level
{
    public Grid.TileType[] ObstacleTypes;
    [SerializeField]
    private int countObstacle;

    public int timeInSeconds;
    private float timer;

    private void Start()
    {
        type = LevelType.OBSTACLETIMER;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(countObstacle);
        hud.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds / 60, timeInSeconds % 60));
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        for (int i = 0; i < ObstacleTypes.Length; i++)
        {
            if (ObstacleTypes[i] == tile.Type)
            {
                if (tile.obstacleDurability <= 0)
                {
                    countObstacle--;
                    hud.SetTarget(countObstacle);
                }
            }
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

            if (countObstacle <= 0)
            {
                currentScore += 10 * ((int)(timeInSeconds - timer) / 60);
                hud.SetScore(currentScore);
                GameWin();
                TimeOut = true;
            }

            if (timeInSeconds - timer <= 0 && countObstacle > 0)
            {
                GameLose();
            }
        }
    }

}
