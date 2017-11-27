using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    [SerializeField]
    GameOver gameOver;
    public Level level;
    [SerializeField]
    private Text remainingText;
    [SerializeField]
    private Text remainingSubText;
    [SerializeField]
    private Text targetText;
    [SerializeField]
    private Text targetSubText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Image[] stars;

    [SerializeField]
    private int starIndex = 0;

    private void Start()
    {
        for (int i = 1; i < stars.Length; i++)
        {
            if (i == starIndex)
            {
                stars[i].enabled = true;
            }
            else
            {
                stars[i].enabled = false;
            }
        }
    }

    public void SetScore(int score)
    {
        scoreText.text = score.ToString();

        if (score >= level.score1Star && score < level.score2Star)
        {
            starIndex = 1;
            stars[0].enabled = true;
        }
        else if (score >= level.score2Star && score < level.score3Star)
        {
            starIndex = 2;
            stars[1].enabled = true;
        }
        else if (score >= level.score3Star)
        {
            starIndex = 3;
            stars[2].enabled = true;
        }
    }

    public void SetTarget(int target)
    {
        targetText.text = target.ToString();
    }

    public void SetRemaining(string remaining)
    {
        remainingText.text = remaining;
    }

    public void SetRemaining(int remaining)
    {
        remainingText.text = remaining.ToString();
    }

    public void SetLevelType(Level.LevelType type)
    {
        switch(type)
        {
            case Level.LevelType.MOVES:
                remainingSubText.text = "Moves Left";
                targetSubText.text = "Target Score";
                break;
            case Level.LevelType.OBSTACLE:
                remainingSubText.text = "Moves Left";
                targetSubText.text = "Ice Remaining";
                break;
            case Level.LevelType.TARGETCAKE:
                remainingSubText.text = "Moves Left";
                targetSubText.text = "Cakes Remaining";
                break;
            case Level.LevelType.TIMER:
                remainingSubText.text = "Time Left";
                targetSubText.text = "Target Score";
                break;
        }
    }

    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score, starIndex);
        if (starIndex > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, 0)) // starIndex
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, starIndex);
        }
    }

    public void OnGameLose(int score)
    {
        gameOver.ShowLose(score);
    }
}
