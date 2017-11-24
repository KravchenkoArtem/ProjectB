using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEditor;

public class AltetrnativeGameBoard : MonoBehaviour
{
    public static AltetrnativeGameBoard instance;
    public int[,] board;
    public List<Transform> Tiles = new List<Transform>();
    public List<Transform> goTiles = new List<Transform>();

    public Transform[,] DelTiles;

    [Range(0, 6)]
    public int TargetTile;
    [Range(0, 7)]
    [SerializeField]
    private int xSize = 7;
    [Range(0, 8)]
    [SerializeField]
    private int ySize = 8;
    [SerializeField]
    private int[] changingGridX = new int[] { 2, 3, 4, 5 };
    [SerializeField]
    private int[] changingGridY = new int[] { 0, 6 };
    [SerializeField]
    private bool changingGrid = false;



    public bool IsShifting = false;

    private void Awake()
    {
        instance = GetComponent<AltetrnativeGameBoard>();
        CreateBoard(xSize, ySize, changingGridX, changingGridY, changingGrid);
    }


    public bool Checkifnear()
    {
        AlternativeTile sel = AlternativeTile.select.gameObject.GetComponent<AlternativeTile>();
        AlternativeTile mov = AlternativeTile.moveTo.gameObject.GetComponent<AlternativeTile>();
        return (sel.X == mov.X && (int)Mathf.Abs(sel.Y - mov.Y) == 1)
            || (sel.Y == mov.Y && (int)Mathf.Abs(sel.X - mov.X) == 1);
    }

    // Here continue
    public void Swap()
    {
        AlternativeTile sel = AlternativeTile.select.gameObject.GetComponent<AlternativeTile>();
        AlternativeTile mov = AlternativeTile.moveTo.gameObject.GetComponent<AlternativeTile>();

        Vector3 tempPos = sel.transform.position;
        int tempX = sel.X;
        int tempY = sel.Y;

        //StartCoroutine(SwapPos(mov.transform, sel.transform, 0.2f));

        sel.transform.position = mov.transform.position;
        mov.transform.position = tempPos;

        sel.X = mov.X;
        sel.Y = mov.Y;

        mov.X = tempX;
        mov.Y = tempY;

        board[sel.X, sel.Y] = sel.ID;
        board[mov.X, mov.Y] = mov.ID;
        sel = null;
        mov = null;
    }

    public void ShuffleTile()
    {
        for (int i = 0; i < goTiles.Count; i++)
        {
            Vector3 tempPos = goTiles[i].transform.position;
            int randomIndex = Random.Range(0, goTiles.Count);

            AlternativeTile a = goTiles[i].GetComponent<AlternativeTile>();
            AlternativeTile b = goTiles[randomIndex].GetComponent<AlternativeTile>();
            int tempX = a.X;
            int tempY = a.Y;

            goTiles[i].transform.position = goTiles[randomIndex].transform.position;
            goTiles[randomIndex].transform.position = tempPos;

            a.X = b.X;
            a.Y = b.Y;

            b.X = tempX;
            b.Y = tempY;

            board[a.X, a.Y] = a.ID;
            board[b.X, b.Y] = b.ID;
        }
    }

    public AlternativeTile GetTileByGrid(int x, int y)
    {
        AlternativeTile[] allTile = FindObjectsOfType(typeof(AlternativeTile)) as AlternativeTile[];

        foreach (AlternativeTile a in allTile)
        {
            if (a.X == x && a.Y == y)
            {
                return a;
            }
        }
        return null;
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
                Transform obj = Instantiate(Tiles[randomNumberId].transform, new Vector3(x, y), Quaternion.identity) as Transform;

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
                a.X = x;
                a.Y = y;
                a.ID = randomNumberId;
                a.curSpriteTile = a.GetComponent<SpriteRenderer>().sprite;
                board[x, y] = randomNumberId;
            }
        }
        gameObject.transform.position = new Vector3(-(objectPos.x / 2.0f), -(objectPos.y / 2.0f));
    }

    public void SpawnNewTile(int x, int y)
    {
        int randomTileId = Random.Range(0, Tiles.Count);
        Transform obj = Instantiate(Tiles[randomTileId].transform, new Vector3(x, y), Quaternion.identity) as Transform;
        List<Transform> posibiliteTile = new List<Transform>();
        posibiliteTile.AddRange(Tiles);

        CenteredTile(obj, x, y);

        AlternativeTile a = obj.gameObject.AddComponent<AlternativeTile>();
        goTiles.Add(a.transform);
        a.X = x;
        a.Y = y;
        a.ID = randomTileId;
        a.curSpriteTile = a.GetComponent<SpriteRenderer>().sprite;
        board[x, y] = randomTileId;
    }

    public static bool In<T>(T x, params T[] values)
    {
        return ArrayUtility.Contains<T>(values, x);
    }


    public void Respawn()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (board[x, y] == 500) // spawn on destroyed cell
                {
                    StartCoroutine(ShiftTilesDown(x, y));
                }
            }
        }
    }

    private IEnumerator ShiftTilesDown(int x, int yStart, float shiftDelay = .06f)
    {

        IsShifting = true;
        List<AlternativeTile> _ts = new List<AlternativeTile>();
        for (int y = yStart; y < ySize; y++)
        {
            AlternativeTile _t = GetTileByGrid(x, y);
            _ts.Add(_t);
        }

        yield return new WaitForSeconds(shiftDelay);
        for (int k = 0; k < _ts.Count; k++)
        {
            if (_ts[k] != null)
            {
                _ts[k].Y--;
                _ts[k].transform.position = new Vector3(_ts[k].transform.position.x, _ts[k].transform.position.y - 1);
                board[_ts[k].X, _ts[k].Y] = _ts[k].ID;
            }
        }
        _ts.Clear();
        IsShifting = false;
    }

    private void CenteredTile(Transform obj, int x, int y)
    {
        Vector3 localScale = obj.localScale;
        Vector3 objectPos = new Vector3(x * (localScale.x), y * (localScale.y));
        obj.transform.parent = gameObject.transform;
        obj.transform.localPosition = objectPos;
    }

    public void ClearTile(int x, int y)
    {
        Destroy(GetTileByGrid(x, y).gameObject);
    }

    public void ClearRow(int row)
    {
        for (int x = 0; x < xSize; x++)
        {
            ClearTile(x, row);
        }
    }

    public void ClearColumn(int column)
    {
        for (int y = 0; y < ySize; y++)
        {
            ClearTile(column, y);
        }
    }

    public void ClearTileByID(int id)
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (GetTileByGrid(x, y).ID == id)
                {
                    ClearTile(x, y);
                }
            }
        }
    }
}
