using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{
    public GameObject MainPanel;
    public GameObject GameOverPanel;
    public Image[] Stars;
    public Text WinLoseText;
    public Text ScoreText;
    public Text BestScoreText;
    public Image GameOverTitle;
    public Button playButton;

    private void Start()
    {
        GameOverPanel.SetActive(false);

        for (int i = 0; i < Stars.Length; i++)
        {
            Stars[i].enabled = false;
        }
    }

    public void ShowLose(int score)
    {
        MainPanel.SetActive(false);
        GameOverPanel.SetActive(true);
        WinLoseText.text = "Stage is failed";
        GameOverTitle.color = Color.red;
        playButton.gameObject.SetActive(false);

        BestScoreText.text = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, score).ToString();

        ScoreText.text = score.ToString();
    }

    public void ShowWin(int score, int starCount)
    {
        MainPanel.SetActive(false);
        GameOverPanel.SetActive(true);
        WinLoseText.text = "Stage is done";
        GameOverTitle.color = Color.white;
        playButton.gameObject.SetActive(true);

        ScoreText.text = score.ToString();
        if (score > PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, score))
        {
            PlayerPrefs.SetInt(SceneManager.GetActiveScene().name, score);
        }

        BestScoreText.text = PlayerPrefs.GetInt(SceneManager.GetActiveScene().name, score).ToString();

        BestScoreText.enabled = false;
        ScoreText.enabled = false;

        StartCoroutine(ShowWinCoroutine(starCount));
    }

    private IEnumerator ShowWinCoroutine(int starCount)
    {
        yield return new WaitForSeconds(0.5f);

        if (starCount <= Stars.Length)
        {
            for (int i = 0; i < starCount; i++)
            {
                Stars[i].enabled = true;
                yield return new WaitForSeconds(0.5f);
            }
        }
        ScoreText.enabled = true;
        BestScoreText.enabled = true;
    }

    public void ClickReplay()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ClickMoveNextLevel()
    {
        //  SceneManager.LoadScene("cur level + 1");
    }

    public void ClickBack()
    {
        //SceneManager.LoadScene("level select");
    }
}
