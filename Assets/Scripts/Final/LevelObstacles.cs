using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelObstacles : Level
{
    public int numMoves = 0;
    public Grid.TileType[] obstacleTypes;

    private int movesUsed = 0;
    private int numObstacleLeft;
    [HideInInspector]
    public int NumObstacleLeft { get { return numObstacleLeft; } set { numObstacleLeft = Mathf.Clamp(value, 0, numObstacleLeft); } }

    private void Start()
    {
        type = LevelType.OBSTACLE;

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            NumObstacleLeft += grid.GetTilesOfType(obstacleTypes[i]).Count;
        }

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(NumObstacleLeft);
        hud.SetRemaining(numMoves);
    }

    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining(numMoves - movesUsed);

        if (numMoves - movesUsed == 0 && NumObstacleLeft > 0)
        {
            GameLose();
        }
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        for (int i = 0; i < obstacleTypes.Length; i++)
        {
            if (obstacleTypes[i] == tile.Type)
            {
                if (tile.obstacleDurability <= 0)
                {
                    NumObstacleLeft--;
                    hud.SetTarget(NumObstacleLeft);

                    if (numObstacleLeft == 0)
                    {
                        currentScore += 120 * (numMoves - movesUsed);
                        hud.SetScore(currentScore);
                        GameWin();
                    }
                }
            }
        }
    }
}
