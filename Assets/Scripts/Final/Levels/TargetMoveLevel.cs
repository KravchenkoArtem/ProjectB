using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetMoveLevel : Level
{
    public int numMoves = 0;
    private Tile.CakeType cakeType;

    private int movesUsed = 0;
    [SerializeField]
    private int targetCakesLeft = 10;
    [SerializeField]
    public int NumTargetLeft { get { return targetCakesLeft; } set { targetCakesLeft = Mathf.Clamp(value, 0, targetCakesLeft); } }

    private void Start()
    {
        type = LevelType.TARGETMOVES;

        cakeType = RandomizeCakeType();

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

    public override void OnBombDetonate()
    {
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);
    }

    public override void OnTileCleared(Tile tile)
    {
        base.OnTileCleared(tile);

        if (!TimeOut)
        {
            if (cakeType == tile.Cake)
            {
                NumTargetLeft--;
                hud.SetTarget(NumTargetLeft);

                if (NumTargetLeft == 0)
                {
                    currentScore += 80 * (numMoves - movesUsed);
                    hud.SetScore(currentScore);
                    GameWin();
                    TimeOut = true;
                }
            }
        }
    }
}
