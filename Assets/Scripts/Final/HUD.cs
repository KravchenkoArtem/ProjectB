using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HUD : MonoBehaviour
{
    private GameOver gameOver;
    private Level level;
    [SerializeField]
    private Text remainingText;
    [SerializeField]
    private Text remainingSubText;
    [SerializeField]
    private Text targetText;
    [SerializeField]
    private Text numText;
    [SerializeField]
    private Text scoreText;
    [SerializeField]
    private Image[] stars;
    [SerializeField]
    private GameObject targetCake;
  
    [SerializeField]
    private Sprite[] srCakes;

    [SerializeField]
    private int starIndex = 0;

    private void Awake()
    {
        gameOver = GameObject.FindGameObjectWithTag("GO").GetComponent<GameOver>();
        level = GameObject.FindGameObjectWithTag("GM").GetComponent<Level>();
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

    public void SetTargetCake(Tile.CakeType cakeSprite)
    {
        targetCake.gameObject.SetActive(true);
        switch (cakeSprite)
        {
            case Tile.CakeType.BLACKCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[0];
                break;
            case Tile.CakeType.BIGCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[1];
                break;
            case Tile.CakeType.CROISSANTCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[2];
                break;
            case Tile.CakeType.PINKCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[3];
                break;
            case Tile.CakeType.REDCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[4];
                break;
            case Tile.CakeType.WHITECAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[5];
                break;
            case Tile.CakeType.YELLOWCAKE:
                targetCake.GetComponent<Image>().sprite = srCakes[6];
                break;
        }
    }

    public void SetTarget(int target)
    {
        numText.text = target.ToString();
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
        switch (type)
        {
            case Level.LevelType.SCOREMOVES:
                remainingSubText.text = "Moves Left";
                targetText.text =  string.Format("Target Score:");
                break;
            case Level.LevelType.OBSTACLEMOVES:
                remainingSubText.text = "Moves Left";
                targetText.text = string.Format("Ice Left:");
                break;
            case Level.LevelType.TARGETMOVES:
                remainingSubText.text = "Moves Left";
                targetText.text = string.Format("Cakes Left:");
                break;
            case Level.LevelType.SCORETIMER:
                remainingSubText.text = "Time Left";
                targetText.text = string.Format("Target Score:");
                break;
            case Level.LevelType.OBSTACLETIMER:
                remainingSubText.text = "Time Left";
                targetText.text = string.Format("Ice Left:");
                break;
            case Level.LevelType.TARGETTIMER:
                remainingSubText.text = "Time Left";
                targetText.text = string.Format("Cakes Left:");
                break;
        }
    }

    public void OnGameWin(int score)
    {
        gameOver.ShowWin(score, starIndex);
    }

    public void OnGameLose(int score)
    {
        gameOver.ShowLose(score);
    }
}
