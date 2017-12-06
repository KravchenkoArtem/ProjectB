using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTimeLevel : Level
{
    public int timeInSeconds;
    public int targetScore;

    private float timer;

    private void Start()
    {
        type = LevelType.SCORETIMER;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        hud.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds / 60, timeInSeconds % 60));
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

            if (currentScore >= targetScore)
            {
                GameWin();
                currentScore += 10 * ((int)(timeInSeconds - timer) / 60);
                TimeOut = true;
            }

            if (timeInSeconds - timer <= 0 && currentScore < targetScore)
            {
                GameLose();
            }
        }
    }
}
