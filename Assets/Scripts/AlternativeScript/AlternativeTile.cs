using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternativeTile : MonoBehaviour
{
    public int ID;
    public int x;
    public int y;
    public GameObject selector;
    public Sprite curSpriteTile;

    private Vector3 myScale;
    private float timer;
    [SerializeField]
    private float timeChangeScale = 1.0f;

    public static Transform select;
    public static Transform moveTo;
    public static AlternativeTile previousSelected = null;

    private bool isSelected = false;


    private void Start()
    {
        myScale = transform.localScale;
        selector = gameObject.transform.GetChild(0).gameObject;
        timer = Time.time;
    }

    private void OnMouseOver()
    {
        if (AltetrnativeGameBoard.instance.IsShifting)
            return;

        transform.localScale = new Vector3(myScale.x + 0.2f, myScale.y + 0.2f, myScale.z + 0.2f);
        if (Input.GetMouseButtonDown(0))
        {
            
            if (!select)
            {
                select = transform;
            }
            else if (select != transform && !moveTo)
            {
                moveTo = transform;
            }
            if (isSelected)
            {
                Deselect();
            }
            else
            {
                if (previousSelected == null)
                {
                    Select();
                }
                else
                {
                    previousSelected.Deselect();
                }
            }
        }
    }

    private void Select()
    {
        isSelected = true;
        selector.SetActive(true);
        transform.localScale = new Vector3(myScale.x + 0.2f, myScale.y + 0.2f, myScale.z + 0.2f);
        previousSelected = gameObject.GetComponent<AlternativeTile>();
    }

    private void Deselect()
    {
        isSelected = false;
        selector.SetActive(false);
        transform.localScale = myScale;
        previousSelected = null;
    }

    private void OnMouseExit()
    {
        transform.localScale = myScale;
    }

    private void Update()
    {
        if (Time.time - timer < timeChangeScale)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, myScale, (Time.time - timer) / timeChangeScale);
        }
    }
}
