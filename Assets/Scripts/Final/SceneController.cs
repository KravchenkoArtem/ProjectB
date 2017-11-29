using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneController : SingletoneAsComponent<SceneController>
{
    public static SceneController Instance
    {
        get { return ((SceneController)_Instance); }
        set { _Instance = value; }
    }
    private AudioManager audioManager;

    private bool levelSelect = false;

    private Animator animatorCanvasMainMenu;

    [SerializeField]
    private GameObject tutorialPanel;

    private void Awake()
    {
        animatorCanvasMainMenu = GameObject.FindGameObjectWithTag("HUD").GetComponent<Animator>();
        DontDestroyOnLoad(gameObject);
    }

    //private void Start()
    //{
    //    audioManager = AudioManager.Instance;
    //    if (audioManager == null)
    //    {
    //        Debug.LogError("AudioManager not found!");
    //    }
    //}

    public void ClickPlay()
    {
        if (animatorCanvasMainMenu)
        {
            levelSelect = true;
            animatorCanvasMainMenu.SetBool("SelectLevel", true);
        }
    }

    public void StartLevel1()
    {
        SceneManager.LoadScene(1);
    }

    public void ClickBack()
    {
        if (animatorCanvasMainMenu)
        {
            if (levelSelect)
            {
                animatorCanvasMainMenu.SetBool("SelectLevel", false);
                levelSelect = false;
            }
            else
            {
                animatorCanvasMainMenu.SetBool("Options", false);
            }
        }
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ShowTutorial()
    {
        tutorialPanel.SetActive(true);
    }

    public void HideTutorial()
    {
        tutorialPanel.SetActive(false);
    }

    public void ClickOptions()
    {
        if (animatorCanvasMainMenu)
        {
            levelSelect = false;
            animatorCanvasMainMenu.SetBool("Options", true);
        }
    }

    public void ClickQuit()
    {
        if (Application.isEditor)
        {
            EditorApplication.isPlaying = false;
        }
        else
        {
            Application.Quit();
        }
    }
}
