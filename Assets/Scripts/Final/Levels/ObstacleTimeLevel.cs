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
    private bool timeOut = false;

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

            if (countObstacle <= 0)
            {
                currentScore += 30 * (int)(timeInSeconds - timer);
                hud.SetScore(currentScore);
                GameWin();
            }

            if (timeInSeconds - timer <= 0 && countObstacle > 0)
            {
                GameLose();
            }
        }
    }

}
