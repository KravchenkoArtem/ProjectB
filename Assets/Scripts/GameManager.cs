using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public bool GameOver = false;
    public Image faderImg;

    public float fadeSpeed = 0.02f;
    //private Color fadeTransparency = new Color(0, 0, 0, 0.04f);
    private string curScene;
    private AsyncOperation async;

    private void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = GetComponent<GameManager>();
        }
    }
}
