using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMoveLevel : Level
{
    public int numMoves = 0;
    public Grid.TileType ObstacleTypes;

    private int movesUsed = 0;
    [SerializeField]
    private int countObstacle;

    private void Start()
    {
        type = LevelType.OBSTACLEMOVES;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(countObstacle);
        hud.SetRemaining(numMoves);
    }

    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining(numMoves - movesUsed);

        if (numMoves - movesUsed == 0 && countObstacle > 0)
        {
            GameLose();
        }
    }

    public override void OnBombDetonate()
    {
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        if (ObstacleTypes == tile.Type)
        {
            if (tile.obstacleDurability <= 0)
            {
                countObstacle--;
                hud.SetTarget(countObstacle);

                if (countObstacle == 0)
                {
                    currentScore += 80 * (numMoves - movesUsed);
                    hud.SetScore(currentScore);
                    GameWin();
                }
            }
        }
    }
}
