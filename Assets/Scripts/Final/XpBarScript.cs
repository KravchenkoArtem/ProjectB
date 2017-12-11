using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XpBarScript : MonoBehaviour
{
    public RectTransform XpTransform;
    private float cachedY;
    private float minXValue;
    private float maxXValue;
    //private int currentXP;
    public int maxXp;
    public int CurrentXP;
    //{
    //    get { return currentXP; }
    //    set
    //    {
    //        currentXP = Mathf.Clamp(value, 0, maxXp);
    //        HandleXP();
    //    }
    //}

    //public Text XpText;

    private void Start()
    {
        cachedY = XpTransform.position.y;
        maxXValue = XpTransform.position.x;
        minXValue = XpTransform.position.x - XpTransform.rect.width;
        CurrentXP = maxXp;
        HandleXP();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            CurrentXP -= 1;
        }
        if (Input.GetKey(KeyCode.X))
        {
            CurrentXP += 1;
        }
    }

    private void HandleXP()
    {
        //XpText.text = "Xp: " + currentXP;

        float currentXValue = MapValues(CurrentXP, 0, maxXp, minXValue, maxXValue);

        XpTransform.position = new Vector3(currentXValue, cachedY);
    }

    private float MapValues(float x, float inMin, float inMax, float outMin, float outMax)
    {
        return (x - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }

}
