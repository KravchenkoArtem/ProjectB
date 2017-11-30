using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreTimeLevel : Level
{
    public int timeInSeconds;
    public int targetScore;

    private float timer;
    private bool timeOut = false;

    private void Start()
    {
        type = LevelType.SCORETIMER;

        hud.SetLevelType(type);
        hud.SetScore(currentScore);
        hud.SetTarget(targetScore);
        hud.SetRemaining(string.Format("{0}:{1:00}", timeInSeconds / 60, timeInSeconds % 60));
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

            if (timeInSeconds - timer <= 0)
            {
                if (currentScore >= targetScore)
                {
                    currentScore += 20 * (int)(timeInSeconds - timer);
                    GameWin();
                }
                else
                {
                    GameLose();
                }
            }
        }
    }
}
