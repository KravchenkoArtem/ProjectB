using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class AltetrnativeGameBoard : MonoBehaviour
{
    public static AltetrnativeGameBoard instance;
    public static int[,] board;
    public List<Transform> Tiles = new List<Transform>();
    private List<Transform> goTiles = new List<Transform>();

    private List<Vector3> test = new List<Vector3>();

    public Transform[,] DelTiles;

    [Range(0, 6)]
    public int TargetTile;
    [Range(0,7)]
    [SerializeField]
    private int xSize = 7;
    [Range(0,8)]
    [SerializeField]
    private int ySize = 8;
    [SerializeField]
    private int[] changingGridX = new int[] { 2, 3, 4, 5 };
    [SerializeField]
    private int[] changingGridY = new int[] { 0, 6 };
    [SerializeField]
    private bool changingGrid = false;


    [HideInInspector]
    public bool IsShifting = false;

    private void Awake()
    {
        instance = GetComponent<AltetrnativeGameBoard>();
        CreateBoard(xSize, ySize, changingGridX, changingGridY, changingGrid);
    }

    private void Update()
    {
        if (AlternativeTile.select && AlternativeTile.moveTo)
        {
            if (Checkifnear() == true)
            {
                Swap();
                if (CheckMatch() == true)
                {
                    AlternativeTile.select = null;
                    AlternativeTile.moveTo = null;
                }
                else
                {
                    Swap();
                    AlternativeTile.select = null;
                    AlternativeTile.moveTo = null;
                }
            }
            else
            {
                AlternativeTile.select = null;
                AlternativeTile.moveTo = null;
            }
        }
    }

    public bool Checkifnear()
    {
        AlternativeTile sel = AlternativeTile.select.gameObject.GetComponent<AlternativeTile>();
        AlternativeTile mov = AlternativeTile.moveTo.gameObject.GetComponent<AlternativeTile>();
        if (sel.x - 1 == mov.x && sel.y == mov.y)
        {
            //left
            return true;
        }
        if (sel.x + 1 == mov.x && sel.y == mov.y)
        {
            //right
            return true;
        }
        if (sel.x == mov.x && sel.y + 1 == mov.y)
        {
            //up
            return true;
        }
        if (sel.x == mov.x && sel.y - 1 == mov.y)
        {
            //down
            return true;
        }
        return false;
    }

    // Here continue
    public void Swap()
    {
        AlternativeTile sel = AlternativeTile.select.gameObject.GetComponent<AlternativeTile>();
        AlternativeTile mov = AlternativeTile.moveTo.gameObject.GetComponent<AlternativeTile>();

        Vector3 tempPos = sel.transform.position;
        int tempX = sel.x;
        int tempY = sel.y;

        //StartCoroutine(SwapPos(mov.transform, sel.transform, 0.2f));

        sel.transform.position = mov.transform.position;
        mov.transform.position = tempPos;

        sel.x = mov.x;
        sel.y = mov.y;

        mov.x = tempX;
        mov.y = tempY;

        board[sel.x, sel.y] = sel.ID;
        board[mov.x, mov.y] = mov.ID;

    }

    IEnumerator SwapPos(Transform obj1, Transform obj2, float time)
    {
        Vector3 tempPos1;
        Vector3 tempPos2;
        tempPos1 = obj1.position;
        tempPos2 = obj2.position;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / time;
            obj1.position = Vector3.Lerp(obj1.position, tempPos2, t);
            obj2.position = Vector3.Lerp(obj2.position, tempPos1, t);
            yield return null;
        }
        obj1 = obj2.transform;
        obj2 = obj1.transform;
    }


    public void ClearGrid()
    {
        for (int x = 0; x < goTiles.Count; x++)
        {
            Destroy(goTiles[x].gameObject);
        }
        goTiles.Clear();
        //CreateBoard(xSize, ySize, changingGridX, changingGridY, changingGrid);
    }

    public void ShuffleTile()
    {
        for (int i = 0; i < goTiles.Count; i++)
        {
            Vector3 tempPos = goTiles[i].transform.position;
            int randomIndex = Random.Range(0, goTiles.Count);

            AlternativeTile a = goTiles[i].GetComponent<AlternativeTile>();
            AlternativeTile b = goTiles[randomIndex].GetComponent<AlternativeTile>();
            int tempX = a.x;
            int tempY = a.y;

            //StartCoroutine(SwapPos(tempPos, b.transform, 0.3f));

            goTiles[i].transform.position = goTiles[randomIndex].transform.position;
            goTiles[randomIndex].transform.position = tempPos;

            a.x = b.x;
            a.y = b.y;

            b.x = tempX;
            b.y = tempY;

            board[a.x, a.y] = a.ID;
            board[b.x, b.y] = b.ID;
        }
    }

    void CreateBoard(int xOffset, int yOffset, int[] changingOffsetX, int[] changingOffsetY, bool changingGrid = false)
    {
        board = new int[xOffset, yOffset];
        int[] changingArrayX = changingOffsetX;
        int[] changingArrayY = changingOffsetY;
        DelTiles = new Transform[xOffset, yOffset];

        Transform[] previousLeft = new Transform[ySize];
        Transform previousBelow = null;

        Vector3 objectPos = Vector3.zero;

        for (int x = 0; x < xOffset; x++)
        {
            for (int y = 0; y < yOffset; y++)
            {
                if (changingGrid)
                {
                    if (In(x, changingArrayY))
                    {
                        if (In(y, changingArrayX))
                            continue;
                    }
                }
                int randomNumberId = Random.Range(0, Tiles.Count); // Id
                Transform obj = Instantiate(Tiles[randomNumberId].transform, new Vector3(x, y, 0), Quaternion.identity) as Transform;

                Vector3 localScale = obj.localScale;
                objectPos = new Vector3(x * (localScale.x), y * (localScale.y));
                obj.transform.parent = gameObject.transform;
                obj.transform.localPosition = objectPos;

                List<Transform> posibiliteTile = new List<Transform>();
                posibiliteTile.AddRange(Tiles);

                posibiliteTile.Remove(previousLeft[y]);
                posibiliteTile.Remove(previousBelow);
                Transform newTransform = Tiles[Random.Range(0, posibiliteTile.Count)];
                newTransform = obj;
                previousLeft[y] = newTransform;
                previousBelow = newTransform;

                AlternativeTile a = obj.gameObject.AddComponent<AlternativeTile>();
                goTiles.Add(a.transform);
                a.x = x;
                a.y = y;
                a.ID = randomNumberId;
                a.curSpriteTile = a.GetComponent<SpriteRenderer>().sprite;
                board[x, y] = randomNumberId;
            }
        }
        gameObject.transform.position = new Vector3(-(objectPos.x / 2.0f), -(objectPos.y / 2.0f));
    }

    public bool CheckMatch()
    {
        AlternativeTile[] allSel = FindObjectsOfType(typeof(AlternativeTile)) as AlternativeTile[];
        AlternativeTile sel = AlternativeTile.select.gameObject.GetComponent<AlternativeTile>();

        int countU = 0;
        int countD = 0;
        int countR = 0;
        int countL = 0;

        // left select & moveTo
        for (int l = sel.x - 1; l >= 0; l--)
        {
            if (board[l, sel.y] == sel.ID)
            {
                countL++;
            }
            if (board[l, sel.y] != sel.ID)
            {
                break;
            }
        }

        // right select & moveTo
        for (int r = sel.x; r < board.GetLength(0); r++)
        {
            if (board[r, sel.y] == sel.ID)
            {
                countR++;
            }
            if (board[r, sel.y] != sel.ID)
            {
                break;
            }
        }
        // down select & moveTo
        for (int d = sel.y - 1; d >= 0; d--)
        {
            if (board[sel.x, d] == sel.ID)
            {
                countD++;
            }
            if (board[sel.x, d] != sel.ID)
            {
                break;
            }
        }
        // up select & moveTo
        for (int u = sel.y; u < board.GetLength(1); u++)
        {
            if (board[sel.x, u] == sel.ID)
            {
                countU++;
            }
            if (board[sel.x, u] != sel.ID)
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
                    foreach (AlternativeTile a in allSel)
                    {
                        if (a.x == sel.x - cl && a.y == sel.y)
                        {
                            if (sel.ID == TargetTile)
                                indexerHorz++;

                            board[a.x, a.y] = 500; // note empty tile.
                            goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
                for (int cr = 0; cr < countR; cr++)
                {
                    foreach (AlternativeTile a in allSel)
                    {
                        if (a.x == sel.x + cr && a.y == sel.y)
                        {
                            if (sel.ID == TargetTile)
                                indexerHorz++;

                            board[a.x, a.y] = 500;
                            goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
            }
            if (countD + countU >= 3)
            {
                for (int cd = 0; cd <= countD; cd++)
                {
                    foreach (AlternativeTile a in allSel)
                    {
                        if (a.x == sel.x && a.y == sel.y - cd)
                        {
                            if (sel.ID == TargetTile)
                                indexerVert++;

                            board[a.x, a.y] = 500;
                            goTiles.Remove(a.transform);
                            Destroy(a.gameObject);
                        }
                    }
                }
                for (int cu = 0; cu < countU; cu++)
                {
                    foreach (AlternativeTile a in allSel)
                    {
                        if (a.x == sel.x && a.y == sel.y + cu)
                        {
                            if (sel.ID == TargetTile)
                                indexerVert++;

                            board[a.x, a.y] = 500;
                            goTiles.Remove(a.transform);
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
            Respawn();
            return true;
        }
        return false;
    }

    public static bool In<T>(T x, params T[] values)
    {
        return ArrayUtility.Contains<T>(values, x);
    }

    [SerializeField]
    GameObject gizmos;

    private void Respawn()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (board[x, y] == 500) // spawn on destroyed cell
                {
                    int randomNumberID = Random.Range(0, Tiles.Count); // Id
                    Transform obj = (Instantiate(Tiles[randomNumberID].transform, new Vector3(x, y, 0), Quaternion.identity));

                    CenteredTile(obj, x, y);

                    obj.parent = transform;
                    AlternativeTile a = obj.gameObject.AddComponent<AlternativeTile>();
                    goTiles.Add(a.transform);
                    a.ID = randomNumberID;
                    a.x = x;
                    a.y = y;
                    board[x, y] = randomNumberID;
                }
            }
        }
    }
    // Перенести все передвижения в AlternativeTiles.
    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .03f)
    {
        IsShifting = true;
        List<Transform> _ts = new List<Transform>();
        int nullCount = 0;

        for (int y = yStart; y < ySize; y++)
        {
            Transform _t = DelTiles[x, y];
            if (_t == null)
            {
                nullCount++;
            }
            _ts.Add(_t);
        }
        for (int i = 0; i < nullCount; i++)
        {
            yield return new WaitForSeconds(shiftDelay);
            for (int k = 0; k < _ts.Count - 1; k++)
            {
                _ts[k] = _ts[k + 1];
                _ts[k + 1] = null;
            }
        }

        IsShifting = false;
    }
    private void CenteredTile(Transform obj, int x, int y)
    {
        Vector3 localScale = obj.localScale;
        Vector3 objectPos = new Vector3(x * (localScale.x), y * (localScale.y));
        obj.transform.parent = gameObject.transform;
        obj.transform.localPosition = objectPos;
    }
}
