using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreMoveLevel : Level
{
    public int numMoves;
    public int targetScore;

    private int movesUsed = 0;

    private void Start()
    {
        type = LevelType.SCOREMOVES;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        hud.SetRemaining(numMoves);
    }

    public override void OnMove()
    {
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);

        if (numMoves - movesUsed == 0)
        {
            GameLose();
        }
        if (currentScore >= targetScore)
        {
            currentScore += 80 * (numMoves - movesUsed);
            GameWin();
        }
    }

    public override void OnBombDetonate()
    {
        movesUsed++;
        hud.SetRemaining(numMoves - movesUsed);
    }
}
