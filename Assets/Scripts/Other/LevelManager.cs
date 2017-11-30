﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    public int CurLevel { get; private set; }
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

        UpdateData(PlayerPrefs.GetInt("CurLevel"), 8);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            PlayerPrefs.DeleteAll();
        }
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
        SceneManager.LoadScene(name);
    }

    public void UpdateData(int curLevel, int maxLevel)
    {
        this.CurLevel = curLevel;
        this.MaxLevel = maxLevel;
    }

    public void RestartCurrent()
    {
        string name = "Level" + CurLevel;
        SceneManager.LoadScene(name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }
}