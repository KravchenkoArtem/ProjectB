using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBarScript : MonoBehaviour
{
    public RectTransform XpTransform;
    private float cachedX;
    private float minYValue;
    private float maxYValue;
    private float currentYValue;
    [SerializeField]
    private int currentXP;
    [SerializeField]
    private int curLevel;
    public int maxXp;
    public int CurrentXP
    {
        get { return currentXP; }
        set
        {
            currentXP = Mathf.Clamp(value, 0, maxXp);
            HandleXpAndLevel();
        }
    }

    [SerializeField]
    private Text healthText;
    [SerializeField]
    private Text levelText;

    private void Awake()
    {
        transform.SetSiblingIndex(3);
    }

    private void Start()
    {
        cachedX = XpTransform.position.x;
        maxYValue = XpTransform.position.y;
        minYValue = XpTransform.position.y - XpTransform.rect.width / 1.5f;
        
        if (PlayerPrefs.GetInt("curLvl") <= 0)
        {
            UpdatePlayerPrefs(curLevel, maxXp);
        }
        maxXp = PlayerPrefs.GetInt("maxXp");
        CurrentXP = PlayerPrefs.GetInt("curXp");
        curLevel = PlayerPrefs.GetInt("curLvl");

        HandleXpAndLevel();
        XpTransform.position = new Vector3(cachedX, currentYValue);
    }

    public void AddXp(int xpToAdd)
    {
        CurrentXP += xpToAdd;
        PlayerPrefs.SetInt("curXp", CurrentXP);
        HandleXpAndLevel();
    }

    private void LevelUp()
    {
        XpTransform.position = new Vector3(cachedX, minYValue);
        CurrentXP = 0;
        curLevel++;
        maxXp = (int)(maxXp * 1.2f);
    }

    private void HandleXpAndLevel()
    {
        healthText.text = string.Format("XP: {0}/{1}", CurrentXP, maxXp);
        levelText.text = "Level: " + curLevel;

        currentYValue = MapValues(CurrentXP, 0, maxXp, minYValue, maxYValue);

        StartCoroutine(MoveBar(0.6f));

        if (currentXP >= maxXp)
        {
            LevelUp();
            PlayerPrefs.SetInt("curXp", CurrentXP);
            UpdatePlayerPrefs(curLevel, maxXp);
        }
    }

    private IEnumerator MoveBar(float time)
    {
        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            XpTransform.position = Vector2.MoveTowards(XpTransform.position, new Vector2(cachedX, currentYValue), t / time);
            yield return 0;
        }
    }

    private void UpdatePlayerPrefs(int curLvl, int maxXp)
    {
        PlayerPrefs.SetInt("curLvl", curLvl);
        PlayerPrefs.SetInt("maxXp", maxXp);
    }

    private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }
}
