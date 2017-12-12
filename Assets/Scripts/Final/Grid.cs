using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    private AudioManager audioManager;
    private Matching matching;

    public enum TileType
    {
        EMPTY,
        NORMAL,
        ICE,
        BOOMB,
        GRIDSPACE
    };

    [System.Serializable]
    public struct TilePrefab
    {
        public TileType type;
        public GameObject prefab;
    };

    [System.Serializable]
    public struct TilePosition
    {
        public TileType type;
        public int x;
        public int y;
    };

    [Header("Grid Size")]
    public int XSize;
    public int YSize;

    [Space]
    [Header("Grid Empty Space")]
    [SerializeField]
    private int[] changingGridX = new int[] { 2, 3, 4, 5 };
    [SerializeField]
    private int[] changingGridY = new int[] { 0, 6 };
    [SerializeField]
    private bool changingGrid;

    [Space]
    [Header("Grid Obstacle Space")]
    [SerializeField]
    private int[] addObstacleGridX;
    [SerializeField]
    private int[] addObstacleGridY;
    [SerializeField]
    private bool obstacleGrid;

    [Space]
    [Header("Grid World Position Offset")]
    [SerializeField]
    private float gridOffsetXPosition;
    [SerializeField]
    private float gridOffsetYPosition;

    [Space]
    [SerializeField]
    [Range(0, 80)]
    private int probabilitySpawnBomb = 0;

    [Space]
    public float FillTime;

    [HideInInspector]
    public Level Level;

    public TilePrefab[] TilePrefabs;
    public GameObject BackgroundPrefab;
    private Dictionary<TileType, GameObject> tilePrefabDict;
    public Tile[,] Tiles;
    [HideInInspector]
    public List<Tile> GoTile = new List<Tile>();

    [HideInInspector]
    public Tile SelectedTile;
    [HideInInspector]
    public Tile MovingTile;

    [HideInInspector]
    public bool GameOver = false;
    private bool inverse = false;
    public bool IsFilling = false;

    private List<GameObject> goNormal = new List<GameObject>();

    private void Awake()
    {
        Level = GameObject.FindGameObjectWithTag("GM").GetComponent<Level>();
        Instance = GetComponent<Grid>();
        matching = GetComponent<Matching>();
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
        }
    }

    private void Start()
    {
        CreateGrid();
    }

    private void CreateGrid()
    {
        goNormal.Clear();
        PrefabPoolingSystem.Reset();
        tilePrefabDict = new Dictionary<TileType, GameObject>();

        for (int i = 0; i < TilePrefabs.Length; i++)
        {
            if (!tilePrefabDict.ContainsKey(TilePrefabs[i].type))
            {
                tilePrefabDict.Add(TilePrefabs[i].type, TilePrefabs[i].prefab);
            }
        }

        Tiles = new Tile[XSize, YSize];

        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (obstacleGrid)
                {
                    if (In(x, addObstacleGridY))
                    {
                        if (In(y, addObstacleGridX))
                        {
                            SpawnNewTile(x, y, TileType.ICE);
                        }
                    }
                }

                if (changingGrid)
                {
                    if (In(x, changingGridY))
                    {
                        if (In(y, changingGridX))
                        {
                            SpawnNewTile(x, y, TileType.GRIDSPACE);
                            continue;
                        }
                    }
                }
                GameObject obj = Instantiate(BackgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                obj.transform.parent = transform;
                if (Tiles[x, y] == null)
                {
                    SpawnNewTile(x, y, TileType.EMPTY);
                }
            }
        }
        PrefabPoolingSystem.Prespawn(tilePrefabDict[TileType.NORMAL], 20);
        StartCoroutine(Fill());
    }

    public IEnumerator Fill()
    {
        bool needsRefill = true;
        IsFilling = true;
        while (needsRefill)
        {
            yield return new WaitForSeconds(FillTime);

            while (FillStep())
            {
                inverse = !inverse;
                yield return new WaitForSeconds(FillTime);
            }
            needsRefill = ClearAllValidMatches();
        }
        IsFilling = false;
    }

    public bool FillStep()
    {
        bool movedTile = false;

        for (int y = YSize - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < XSize; loopX++)
            {
                int x = loopX;
                if (inverse)
                {
                    x = XSize - 1 - loopX;
                }
                Tile tile = Tiles[x, y];
                if (tile.IsMovable)
                {
                    Tile tileBelow = Tiles[x, y + 1];
                    if (tileBelow.Type == TileType.EMPTY)
                    {
                        Destroy(tileBelow.gameObject);

                        tile.Move(x, y + 1, FillTime);
                        Tiles[x, y + 1] = tile;
                        SpawnNewTile(x, y, TileType.EMPTY);
                        movedTile = true;
                    }
                    else
                    {
                        for (int diag = -1; diag <= 1; diag++)
                        {
                            if (diag != 0)
                            {
                                int diagX = x + diag;

                                if (inverse)
                                {
                                    diagX = x - diag;
                                }

                                if (diagX >= 0 && diagX < XSize)
                                {
                                    Tile diagonalTile = Tiles[diagX, y + 1];

                                    if (diagonalTile.Type == TileType.EMPTY)
                                    {
                                        bool hasTileAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            Tile tileAbove = Tiles[diagX, aboveY];

                                            if (tileAbove.IsMovable)
                                            {
                                                break;
                                            }
                                            else if (!tileAbove.IsMovable && tileAbove.Type != TileType.EMPTY)
                                            {
                                                hasTileAbove = false;
                                                break;
                                            }
                                        }

                                        if (!hasTileAbove)
                                        {
                                            Destroy(diagonalTile.gameObject);
                                            tile.Move(diagX, y + 1, FillTime);
                                            Tiles[diagX, y + 1] = tile;
                                            SpawnNewTile(x, y, TileType.EMPTY);
                                            movedTile = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        for (int x = 0; x < XSize; x++)
        {
            Tile tileBelow = Tiles[x, 0];
            if (tileBelow.Type == TileType.EMPTY)
            {
                Destroy(tileBelow.gameObject);

                if (Random.Range(0, probabilitySpawnBomb) == 1)
                {
                    GameObject boombTile = Instantiate(tilePrefabDict[TileType.BOOMB], GetWorldPosition(x, -1), Quaternion.identity);
                    boombTile.transform.parent = transform;

                    GoTile.Add(boombTile.GetComponent<Tile>());

                    Tiles[x, 0] = boombTile.GetComponent<Tile>();
                    Tiles[x, 0].Init(x, -1, this, TileType.BOOMB);
                    Tiles[x, 0].Move(x, 0, FillTime);
                    movedTile = true;
                }
                else
                {
                    GameObject newTile = SpawnObject(tilePrefabDict[TileType.NORMAL], goNormal, GetWorldPosition(x, -1));
                    newTile.transform.parent = transform;

                    GoTile.Add(newTile.GetComponent<Tile>());

                    Tiles[x, 0] = newTile.GetComponent<Tile>();
                    Tiles[x, 0].Init(x, -1, this, TileType.NORMAL);
                    Tiles[x, 0].Move(x, 0, FillTime);
                    Tiles[x, 0].SetCake((Tile.CakeType)Random.Range(0, Tiles[x, 0].NumCakes));
                    movedTile = true;
                }
            }
        }
        return movedTile;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for (int y = 0; y < YSize; y++)
        {
            for (int x = 0; x < XSize; x++)
            {
                if (Tiles[x, y].IsClearable)
                {
                    List<Tile> match = matching.GetMatch(Tiles[x, y], x, y);

                    if (match != null)
                    {
                        if (match.Count >= 4)
                        {
                            ClearCake(match[0].Cake);
                            needsRefill = true;
                        }

                        for (int i = 0; i < match.Count; i++)
                        {
                            if (ClearTile(match[i].X, match[i].Y, goNormal))
                            {
                                needsRefill = true;
                            }
                        }
                    }
                }
            }
        }
        return needsRefill;
    }


    public bool ClearTile(int x, int y, List<GameObject> list)
    {
        if (Tiles[x, y].IsClearable && !Tiles[x, y].IsBeginCleared)
        {
            GoTile.Remove(Tiles[x, y]);
            Tiles[x, y].Clear(list);
            SpawnNewTile(x, y, TileType.EMPTY);

            ClearSpecialTile(x, y);
            return true;
        }
        return false;
    }

    public void ClearSpecialTile(int x, int y)
    {
        for (int NearX = x - 1; NearX <= x + 1; NearX++)
        {
            if (NearX != x && NearX >= 0 && NearX < XSize)
            {
                if (Tiles[NearX, y].IsClearable)
                {
                    if (Tiles[NearX, y].Type == TileType.ICE)
                    {
                        Tiles[NearX, y].obstacleDurability -= 1;
                        Tiles[NearX, y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        if (Tiles[NearX, y].obstacleDurability <= 0)
                        {
                            Destroy(Tiles[NearX, y].gameObject);
                            Tiles[NearX, y].Clear();
                            SpawnNewTile(NearX, y, TileType.EMPTY);
                        }
                    }
                    if (Tiles[NearX, y].Type == TileType.BOOMB)
                    {
                        Level.OnBombDetonate();
                        audioManager.PlaySound(3);
                        Tiles[NearX, y].Clear();
                        GoTile.Remove(Tiles[NearX, y]);
                        SpawnNewTile(NearX, y, TileType.EMPTY);
                    }
                }
            }
        }

        for (int NearY = y - 1; NearY <= y + 1; NearY++)
        {
            if (NearY != y && NearY >= 0 && NearY < YSize)
            {
                if (Tiles[x, NearY].IsClearable)
                {
                    if (Tiles[x, NearY].Type == TileType.ICE)
                    {
                        Tiles[x, NearY].obstacleDurability -= 1;
                        Tiles[x, NearY].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        if (Tiles[x, NearY].obstacleDurability <= 0)
                        {
                            Tiles[x, NearY].Clear();
                            GoTile.Remove(Tiles[x, NearY]);
                            SpawnNewTile(x, NearY, TileType.EMPTY);
                        }
                    }
                    if (Tiles[x, NearY].Type == TileType.BOOMB)
                    {
                        Level.OnBombDetonate();
                        audioManager.PlaySound(3);
                        Tiles[x, NearY].Clear();
                        GoTile.Remove(Tiles[x, NearY]);
                        SpawnNewTile(x, NearY, TileType.EMPTY);
                    }
                }
            }
        }
    }

    public void ClearCake(Tile.CakeType cake)
    {
        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (Tiles[x, y].IsCake && (Tiles[x, y].Cake == cake))
                {
                    ClearTile(x, y, goNormal);
                }
            }
        }
    }

    public void SelectTile(Tile tile)
    {
        SelectedTile = tile;
    }

    public void MoveTile(Tile tile)
    {
        MovingTile = tile;
    }

    public void MovedTile()
    {
        if (matching.TileIsNear(SelectedTile, MovingTile) && (matching.GetMatch(SelectedTile, MovingTile.X, MovingTile.Y) != null
            || matching.GetMatch(MovingTile, SelectedTile.X, SelectedTile.Y) != null))
        {
            SwapTile(SelectedTile, MovingTile);
        }
        else
        {
            SelectedTile.Deselect();
        }
    }

    public void SwapTile(Tile sel, Tile mov)
    {
        if (GameOver)
            return;

        if (sel.IsMovable && mov.IsMovable)
        {
            Tiles[sel.X, sel.Y] = mov;
            Tiles[mov.X, mov.Y] = sel;

            if (matching.GetMatch(sel, mov.X, mov.Y) != null || matching.GetMatch(mov, sel.X, sel.Y) != null)
            {
                int selX = sel.X;
                int selY = sel.Y;

                sel.Move(mov.X, mov.Y, FillTime);
                mov.Move(selX, selY, FillTime);

                ClearAllValidMatches();

                sel.Deselect();
                mov.Deselect();

                StartCoroutine(Fill());

                Level.OnMove();
            }
            else
            {
                Tiles[sel.X, sel.Y] = sel;
                Tiles[mov.X, mov.Y] = sel;
            }
        }
    }

    public Tile SpawnNewTile(int x, int y, TileType type, List<GameObject> list)
    {
        GameObject newTile = SpawnObject(tilePrefabDict[type], list, GetWorldPosition(x, y));
        newTile.transform.parent = transform;

        Tiles[x, y] = newTile.GetComponent<Tile>();
        Tiles[x, y].Init(x, y, this, type);
        return Tiles[x, y];
    }

    public Tile SpawnNewTile(int x, int y, TileType type)
    {
        GameObject newTile = Instantiate(tilePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newTile.transform.parent = transform;

        Tiles[x, y] = newTile.GetComponent<Tile>();
        Tiles[x, y].Init(x, y, this, type);
        return Tiles[x, y];
    }

    public Vector2 GetWorldPosition(int x, int y)
    {
        return new Vector2((transform.position.x - XSize / 2.0f + x) - gridOffsetXPosition,
            (transform.position.y + YSize / 2.0f - y) - gridOffsetYPosition);
    }

    public static bool In<T>(T x, params T[] values)
    {
        return ArrayUtility.Contains<T>(values, x);
    }

    public List<Tile> GetTilesOfType(TileType type)
    {
        List<Tile> tilesOfType = new List<Tile>();

        for (int x = 0; x < XSize; x++)
        {
            for (int y = 0; y < YSize; y++)
            {
                if (Tiles[x, y].Type == type)
                {
                    tilesOfType.Add(Tiles[x, y]);
                }
            }
        }
        return tilesOfType;
    }

    private GameObject SpawnObject(GameObject prefab, List<GameObject> list, Vector2 vec)
    {
        GameObject obj = PrefabPoolingSystem.Spawn(prefab, vec, Quaternion.identity);
        list.Add(obj);
        return obj;
    }
}

