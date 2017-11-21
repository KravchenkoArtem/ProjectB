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

    AltetrnativeGameBoard GB;

    private bool isSelected = false;

    private void Awake()
    {
        GB = AltetrnativeGameBoard.instance;
    }

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

    public bool CheckMatch()
    {
        AlternativeTile[] allTile = FindObjectsOfType(typeof(AlternativeTile)) as AlternativeTile[];
        AlternativeTile sel = select.gameObject.GetComponent<AlternativeTile>();
        AlternativeTile mov = moveTo.gameObject.GetComponent<AlternativeTile>();

        int countU = 0;
        int countD = 0;
        int countR = 0;
        int countL = 0;

        // left select
        for (int l = sel.x - 1; l >= 0; l--)
        {
            if (GB.board[l, sel.y] == sel.ID)
            {
                countL++;
            }
            if (GB.board[l, sel.y] != sel.ID)
            {
                break;
            }
        }

        // right select
        for (int r = sel.x; r < GB.board.GetLength(0); r++)
        {
            if (GB.board[r, sel.y] == sel.ID)
            {
                countR++;
            }
            if (GB.board[r, sel.y] != sel.ID)
            {
                break;
            }
        }
        // down select
        for (int d = sel.y - 1; d >= 0; d--)
        {
            if (GB.board[sel.x, d] == sel.ID)
            {
                countD++;
            }
            if (GB.board[sel.x, d] != sel.ID)
            {
                break;
            }
        }
        // up select
        for (int u = sel.y; u < GB.board.GetLength(1); u++)
        {
            if (GB.board[sel.x, u] == sel.ID)
            {
                countU++;
            }
            if (GB.board[sel.x, u] != sel.ID)
            {
                break;
            }
        }

        int indexerHorz = 0;
        int indexerVert = 0;
        // check row of three tile by horizontal and vertical.
        if (countL + countR >= 3 || countD + countU >= 3)
        {
            if (countL + countR >= 3)
            { // Destroy and Note empty blocks.
                for (int cl = 0; cl <= countL; cl++)
                {
                    foreach (AlternativeTile a in allTile)
                    {
                        if (a.x == sel.x - cl && a.y == sel.y)
                        {
                            if (sel.ID == GB.TargetTile)
                                indexerHorz++;

                            GB.board[a.x, a.y] = 500; // note empty tile.
                            GB.goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
                for (int cr = 0; cr < countR; cr++)
                {
                    foreach (AlternativeTile a in allTile)
                    {
                        if (a.x == sel.x + cr && a.y == sel.y)
                        {
                            if (sel.ID == GB.TargetTile)
                                indexerHorz++;

                            GB.board[a.x, a.y] = 500;
                            GB.goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
            }
            if (countD + countU >= 3)
            {
                for (int cd = 0; cd <= countD; cd++)
                {
                    foreach (AlternativeTile a in allTile)
                    {
                        if (a.x == sel.x && a.y == sel.y - cd)
                        {
                            if (sel.ID == GB.TargetTile)
                                indexerVert++;

                            GB.board[a.x, a.y] = 500;
                            GB.goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
                for (int cu = 0; cu < countU; cu++)
                {
                    foreach (AlternativeTile a in allTile)
                    {
                        if (a.x == sel.x && a.y == sel.y + cu)
                        {
                            if (sel.ID == GB.TargetTile)
                                indexerVert++;

                            GB.board[a.x, a.y] = 500;
                            GB.goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
            }
            GUIManager.instance.TargeCounter -= indexerHorz | indexerVert;
            indexerHorz = 0;
            indexerVert = 0;

            GUIManager.instance.Score += (countL + countR) | (countD + countU);
            GUIManager.instance.MoveCounter--;
            GB.Respawn();
            return true;
        }
        return false;
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

        if (select && moveTo)
        {
            if (GB.Checkifnear() == true)
            {
                GB.Swap();
                if (CheckMatch() == true)
                {
                    select = null;
                    moveTo = null;
                }
                else
                {
                    GB.Swap();
                }
            }
            else
            {
                select = null;
                moveTo = null;
            }
        }
    }
}
