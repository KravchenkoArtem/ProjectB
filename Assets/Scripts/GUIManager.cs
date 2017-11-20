using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GUIManager : MonoBehaviour
{
    public static GUIManager instance;
    public Text yourScoreTxt;
    public Text highScoreTxt;

    public Text scoreTxt;
    public Text moveCounterTxt;
    public Text targetTileTxt;
    [SerializeField]
    private Image TargetTileImage;

    [SerializeField]
    private int score;
    [SerializeField]
    private int moveCounter;
    [SerializeField]
    private int targetCounter;

    public int TargeCounter
    {
        get { return targetCounter; }
        set { targetCounter = value; targetTileTxt.text = targetCounter.ToString(); }
    }

    public int Score
    {
        get { return score; }
        set { score = value; scoreTxt.text = score.ToString(); }
    }

    public int MoveCounter
    {
        get { return moveCounter; }
        set { moveCounter = value; moveCounterTxt.text = moveCounter.ToString(); }
    }

    void Awake()
    {
        instance = GetComponent<GUIManager>();
    }

    private void Start()
    {
        TargetTileImage.sprite = AltetrnativeGameBoard.instance.Tiles[AltetrnativeGameBoard.instance.TargetTile].GetComponent<SpriteRenderer>().sprite;
        moveCounter = 20;
        moveCounterTxt.text = moveCounter.ToString();
        targetCounter = 40;
        targetTileTxt.text = targetCounter.ToString();
    }

    public void GameOver()
    {
        if (score > PlayerPrefs.GetInt("HighScore"))
        {
            PlayerPrefs.SetInt("HighScore", score);
            highScoreTxt.text = "New Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        else
        {
            highScoreTxt.text = "Best: " + PlayerPrefs.GetInt("HighScore").ToString();
        }
        yourScoreTxt.text = score.ToString();
    }

}

