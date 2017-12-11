using UnityEngine;
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
    [SerializeField]

    private LevelManager levelManger;
    private bool levelSelect = false;

    private Animator animatorCanvasMainMenu;

    [SerializeField]
    private GameObject tutorialPanel;
    private Transform levels;

    [SerializeField]
    private Slider sliderMusic;
    [SerializeField]
    private Slider sliderSFX;
    [SerializeField]
    private Toggle toggleSound;

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
            Debug.LogError("LevelManager not found!");
        }
        audioManager.StopSound(2);
        audioManager.PlaySound(1);
        OptionsSettings.Instance.SetValueUI(sliderMusic, sliderSFX, toggleSound);
        UnlockLevel();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    public void ClickPlay()
    {
        audioManager.PlaySound(4);
        if (animatorCanvasMainMenu)
        {
            levelSelect = true;
            animatorCanvasMainMenu.SetBool("SelectLevel", true);
        }
    }

    public void ClickBack()
    {
        audioManager.PlaySound(4);
        if (animatorCanvasMainMenu)
        {
            OptionsSettings.Instance.GetValueUI(sliderMusic.value, sliderSFX.value, toggleSound.isOn);
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
        audioManager.PlaySound(4);
        if (animatorCanvasMainMenu)
        {
            levelSelect = false;
            animatorCanvasMainMenu.SetBool("Options", true);
        }
    }

    public void ClickStartLevel(int num)
    {
        audioManager.PlaySound(4);
        levelManger.StartLevel(num);
    }

    public void UnlockLevel()
    {
        if (levels != null)
        {
            for (int i = 0; i < levelManger.CurLevel; i++)
            {
                Transform Child = levels.transform.GetChild(i);
                Child.gameObject.SetActive(true);
                levelSelectInfo[i].BestScoreText.text = PlayerPrefs.GetInt("Level" + (1 + i)).ToString();
                for (int s = 0; s < PlayerPrefs.GetInt(("Level" + (i + 1)) + 1); s++)
                {
                    levelSelectInfo[i].StarCount[s].GetComponent<Image>().enabled = true;
                }
            }
        }
    }

    public void OnSFXValue(float value)
    {
        OptionsSettings.Instance.OnSFXValue(value);
    }

    public void OnMusicValue(float value)
    {
        OptionsSettings.Instance.OnMusicValue(value);
    }

    public void OnSoundToggle()
    {
        OptionsSettings.Instance.OnSoundToggle();
    }

    public void ClickQuit()
    {
        audioManager.PlaySound(4);
        Application.Quit();
    }
}
