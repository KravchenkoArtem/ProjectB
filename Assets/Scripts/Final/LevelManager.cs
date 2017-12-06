using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [HideInInspector]
    public int CurLevel;
    public int MaxLevel { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        
        CurLevel = PlayerPrefs.GetInt("CurLevel");
        MaxLevel = 8;
        if (CurLevel <= 0)
        {
            PlayerPrefs.SetInt("CurLevel", 1);
        }
        UpdateData(PlayerPrefs.GetInt("CurLevel"), 8);
    }

    public void GoToNext()
    {
        if (CurLevel < MaxLevel)
        {
            CurLevel++;
            string name = "Level" + CurLevel;
            PlayerPrefs.SetInt("CurLevel", CurLevel);
            UpdateData(CurLevel, MaxLevel);
            SceneManager.LoadScene(name);
        }
        else
        {
            Debug.Log("Last Level");
        }
    }

    public void StartLevel(int num)
    {
        string name = "Level" + num;
        PlayerPrefs.SetInt("CurLevel", num);
        SceneManager.LoadScene(name);
    }

    public void UpdateData(int curLevel, int maxLevel)
    {
        this.CurLevel = curLevel;
        this.MaxLevel = maxLevel;
    }

    public void RestartCurrent()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
