using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [System.Serializable]
    public struct LevelSelectInfo
    {
        public Text BestScoreText;
        public Transform[] StarCount;
    };

    [SerializeField]
    private LevelSelectInfo[] levelSelectInfo;

    private AudioManager audioManager;
    private LevelManager levelManger;

    private bool levelSelect = false;

    private Animator animatorCanvasMainMenu;

    [SerializeField]
    private GameObject tutorialPanel;
    private Transform levels;

    private void Awake()
    {
        animatorCanvasMainMenu = GameObject.FindGameObjectWithTag("HUD").GetComponent<Animator>();
        levels = GameObject.FindGameObjectWithTag("LS").GetComponent<Transform>();
    }

    private void Start()
    {
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
        }
        levelManger = LevelManager.Instance;
        if (levelManger == null)
        {
            Debug.LogError("AudionManager not found!");
        }
        UnlockLevel();
    }

    public void ClickPlay()
    {
        if (animatorCanvasMainMenu)
        {
            levelSelect = true;
            animatorCanvasMainMenu.SetBool("SelectLevel", true);
        }
    }

    public void StartLevel(int num)
    {
        levelManger.StartLevel(num);
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

    public void UnlockLevel()
    {
        if (levels != null)
        {
            for (int i = 0; i < levelManger.CurLevel; i++)
            {
                Transform Child = levels.transform.GetChild(i);
                Child.gameObject.SetActive(true);
                levelSelectInfo[i].BestScoreText.text = PlayerPrefs.GetInt("Level" + (i + 1)).ToString();
                for (int s = 0; s < PlayerPrefs.GetInt(("Level" + (i + 1)) + 1); s++)
                {
                    levelSelectInfo[i].StarCount[s].GetComponent<Image>().enabled = true;
                }
            }
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
