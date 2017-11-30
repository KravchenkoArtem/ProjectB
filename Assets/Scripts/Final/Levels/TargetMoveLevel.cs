using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMoveLevel : Level
{
    public int numMoves = 0;
    public Tile.CakeType cakeType;

    private int movesUsed = 0;
    [SerializeField]
    private int TargetCakesLeft = 10;
    [SerializeField]
    public int NumTargetLeft { get { return TargetCakesLeft; } set { TargetCakesLeft = Mathf.Clamp(value, 0, TargetCakesLeft); } }

    private void Start()
    {
        type = LevelType.TARGETMOVES;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(NumTargetLeft);
        hud.SetRemaining(numMoves);
        hud.SetTargetCake(cakeType);
    }

    public override void OnMove()
    {
        movesUsed++;

        hud.SetRemaining(numMoves - movesUsed);

        if (numMoves - movesUsed == 0 && NumTargetLeft > 0)
        {
            GameLose();
        }
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        if (cakeType == tile.Cake)
        {
            NumTargetLeft--;
            hud.SetTarget(NumTargetLeft);

            if (NumTargetLeft == 0)
            {
                currentScore += 30 * (numMoves - movesUsed);
                hud.SetScore(currentScore);
                GameWin();
            }
        }
    }
}
