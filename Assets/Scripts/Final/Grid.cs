using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public static Grid Instance;

    private AudioManager audioManager;

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

    public int XSize;
    public int YSize;
    [SerializeField]
    private int[] changingGridX = new int[] { 2, 3, 4, 5 };
    [SerializeField]
    private int[] changingGridY = new int[] { 0, 6 };
    [SerializeField]
    private bool changingGrid = false;

    [SerializeField]
    private int[] addObstacleGridX;
    [SerializeField]
    private int[] addObstacleGridY;
    [SerializeField]
    private bool obstacleGrid = false;

    [SerializeField]
    private float gridOffsetXPosition;
    [SerializeField]
    private float gridOffsetYPosition;


    public float FillTime; // время заполнения

    [HideInInspector]
    public Level Level;

    public TilePrefab[] TilePrefabs;
    public GameObject BackgroundPrefab;
    private Dictionary<TileType, GameObject> tilePrefabDict;
    private Tile[,] tiles;
    private List<Tile> goTile = new List<Tile>();

    [HideInInspector]
    public Tile SelectedTile;
    [HideInInspector]
    public Tile MovingTile;

    [HideInInspector]
    public bool GameOver = false;
    private bool inverse = false;
    public bool IsFilling = false;

    [SerializeField]
    List<GameObject> goNormal = new List<GameObject>();

    private void Awake()
    {
        Level = GameObject.FindGameObjectWithTag("GM").GetComponent<Level>();
        Instance = GetComponent<Grid>();
        audioManager = AudioManager.Instance;
        if (audioManager == null)
        {
            Debug.LogError("AudioManager not found!");
        }
    }

    private void Start()
    {
        goNormal.Clear();
        CreateGrid(changingGridX, changingGridY, changingGrid);
        PrefabPoolingSystem.Prespawn(tilePrefabDict[TileType.NORMAL], 1);
    }

    private void CreateGrid(int[] changingOffsetX, int[] changingOffsetY, bool changingGrid = false)
    {
        tilePrefabDict = new Dictionary<TileType, GameObject>();

        for (int i = 0; i < TilePrefabs.Length; i++)
        {
            if (!tilePrefabDict.ContainsKey(TilePrefabs[i].type))
            {
                tilePrefabDict.Add(TilePrefabs[i].type, TilePrefabs[i].prefab);
            }
        }

        tiles = new Tile[XSize, YSize];

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
                    if (In(x, changingOffsetY))
                    {
                        if (In(y, changingOffsetX))
                        {
                            SpawnNewTile(x, y, TileType.GRIDSPACE);
                        }
                    }
                }
                GameObject obj = Instantiate(BackgroundPrefab, GetWorldPosition(x, y), Quaternion.identity);
                obj.transform.parent = transform;
                if (tiles[x, y] == null)
                {
                    SpawnNewTile(x, y, TileType.EMPTY);
                }
            }
        }
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
                Tile tile = tiles[x, y];
                if (tile.IsMovable)
                {
                    Tile tileBelow = tiles[x, y + 1];
                    if (tileBelow.Type == TileType.EMPTY)
                    {
                        Destroy(tileBelow.gameObject);
                        //ClearTile(tileBelow.X, tileBelow.Y, goEmpty);
                        //tiles[tileBelow.X, tileBelow.Y].Clear(goEmpty);

                        tile.Move(x, y + 1, FillTime);
                        tiles[x, y + 1] = tile;
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
                                    Tile diagonalTile = tiles[diagX, y + 1];

                                    if (diagonalTile.Type == TileType.EMPTY)
                                    {
                                        bool hasTileAbove = true;

                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            Tile tileAbove = tiles[diagX, aboveY];

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
                                            //ClearTile(diagonalTile.X, diagonalTile.Y, goEmpty);
                                            //tiles[tileBelow.X, tileBelow.Y].Clear(goEmpty);
                                            tile.Move(diagX, y + 1, FillTime);
                                            tiles[diagX, y + 1] = tile;
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
            Tile tileBelow = tiles[x, 0];
            if (tileBelow.Type == TileType.EMPTY)
            {
                Destroy(tileBelow.gameObject);
                //ClearTile(tileBelow.X, tileBelow.Y, goEmpty);
                //tiles[tileBelow.X, tileBelow.Y].Clear(goEmpty);

                if (Random.Range(0, 50) == 1)
                {
                    GameObject boombTile = Instantiate(tilePrefabDict[TileType.BOOMB], GetWorldPosition(x, -1), Quaternion.identity);
                    boombTile.transform.parent = transform;

                    goTile.Add(boombTile.GetComponent<Tile>());

                    tiles[x, 0] = boombTile.GetComponent<Tile>();
                    tiles[x, 0].Init(x, -1, this, TileType.BOOMB);
                    tiles[x, 0].Move(x, 0, FillTime);
                    movedTile = true;
                }
                else
                {
                    GameObject newTile = SpawnObject(tilePrefabDict[TileType.NORMAL], goNormal, GetWorldPosition(x, -1)); // Instantiate(tilePrefabDict[TileType.NORMAL], GetWorldPosition(x, -1), Quaternion.identity);
                    newTile.transform.parent = transform;

                    goTile.Add(newTile.GetComponent<Tile>());

                    tiles[x, 0] = newTile.GetComponent<Tile>();
                    tiles[x, 0].Init(x, -1, this, TileType.NORMAL);
                    tiles[x, 0].Move(x, 0, FillTime);
                    tiles[x, 0].SetCake((Tile.CakeType)Random.Range(0, tiles[x, 0].NumCakes));
                    movedTile = true;
                }
            }
        }
        return movedTile;
    }

    public List<Tile> GetMatch(Tile tile, int newX, int newY)
    {
        if (tile.IsCake)
        {
            Tile.CakeType cake = tile.Cake;
            List<Tile> horizontalTiles = new List<Tile>();
            List<Tile> verticaltiles = new List<Tile>();
            List<Tile> matchingTiles = new List<Tile>();

            horizontalTiles.Add(tile);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int xOffset = 1; xOffset < XSize; xOffset++)
                {
                    int x;

                    if (dir == 0)
                    { // Left
                        x = newX - xOffset;
                    }
                    else
                    { // Right
                        x = newX + xOffset;
                    }

                    if (x < 0 || x >= XSize)
                    {
                        break;
                    }

                    if (tiles[x, newY].IsCake && tiles[x, newY].Cake == cake)
                    {
                        horizontalTiles.Add(tiles[x, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (horizontalTiles.Count >= 3)
            {
                for (int i = 0; i < horizontalTiles.Count; i++)
                {
                    matchingTiles.Add(horizontalTiles[i]);
                }
            }

            if (horizontalTiles.Count >= 3)
            {
                for (int i = 0; i < horizontalTiles.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int yOffset = 1; yOffset < YSize; yOffset++)
                        {
                            int y;

                            if (dir == 0)
                            { // Up
                                y = newY - yOffset;
                            }
                            else
                            { // Down
                                y = newY + yOffset;
                            }

                            if (y < 0 || y >= YSize)
                            {
                                break;
                            }

                            if (tiles[horizontalTiles[i].X, y].IsCake && tiles[horizontalTiles[i].X, y].Cake == cake)
                            {
                                verticaltiles.Add(tiles[horizontalTiles[i].X, y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (verticaltiles.Count < 2)
                    {
                        verticaltiles.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < verticaltiles.Count; j++)
                        {
                            matchingTiles.Add(verticaltiles[j]);
                        }

                        break;
                    }
                }
            }

            if (matchingTiles.Count >= 3)
            {
                return matchingTiles;
            }

            horizontalTiles.Clear();
            verticaltiles.Clear();
            verticaltiles.Add(tile);

            for (int dir = 0; dir <= 1; dir++)
            {
                for (int yOffset = 1; yOffset < YSize; yOffset++)
                {
                    int y;

                    if (dir == 0)
                    { // Up
                        y = newY - yOffset;
                    }
                    else
                    { // Down
                        y = newY + yOffset;
                    }

                    if (y < 0 || y >= YSize)
                    {
                        break;
                    }

                    if (tiles[newX, y].IsCake && tiles[newX, y].Cake == cake)
                    {
                        verticaltiles.Add(tiles[newX, y]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (verticaltiles.Count >= 3)
            {
                for (int i = 0; i < verticaltiles.Count; i++)
                {
                    matchingTiles.Add(verticaltiles[i]);
                }
            }

            if (verticaltiles.Count >= 3)
            {
                for (int i = 0; i < verticaltiles.Count; i++)
                {
                    for (int dir = 0; dir <= 1; dir++)
                    {
                        for (int xOffset = 1; xOffset < XSize; xOffset++)
                        {
                            int x;

                            if (dir == 0)
                            { // Left
                                x = newX - xOffset;
                            }
                            else
                            { // Right
                                x = newX + xOffset;
                            }

                            if (x < 0 || x >= XSize)
                            {
                                break;
                            }

                            if (tiles[x, verticaltiles[i].Y].IsCake && tiles[x, verticaltiles[i].Y].Cake == cake)
                            {
                                horizontalTiles.Add(tiles[x, verticaltiles[i].Y]);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }

                    if (horizontalTiles.Count < 2)
                    {
                        horizontalTiles.Clear();
                    }
                    else
                    {
                        for (int j = 0; j < horizontalTiles.Count; j++)
                        {
                            matchingTiles.Add(horizontalTiles[j]);
                        }

                        break;
                    }
                }
            }
            if (matchingTiles.Count >= 3)
            {
                return matchingTiles;
            }
        }
        return null;
    }

    public bool ClearAllValidMatches()
    {
        bool needsRefill = false;

        for (int y = 0; y < YSize; y++)
        {
            for (int x = 0; x < XSize; x++)
            {
                if (tiles[x, y].IsClearable)
                {
                    List<Tile> match = GetMatch(tiles[x, y], x, y);

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
        if (tiles[x, y].IsClearable && !tiles[x, y].IsBeginCleared)
        {
            goTile.Remove(tiles[x, y]);
            tiles[x, y].Clear(list);
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
                if (tiles[NearX, y].IsClearable)
                {
                    if (tiles[NearX, y].Type == TileType.ICE)
                    {
                        tiles[NearX, y].obstacleDurability -= 1;
                        tiles[NearX, y].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        if (tiles[NearX, y].obstacleDurability <= 0)
                        {
                            Destroy(tiles[NearX, y].gameObject);
                            goTile.Remove(tiles[NearX, y]);
                            SpawnNewTile(NearX, y, TileType.EMPTY);
                        }
                    }
                    if (tiles[NearX, y].Type == TileType.BOOMB)
                    {
                        Level.OnMove();
                        //ClearTile(NearX, y, goBomb);
                        Destroy(tiles[NearX, y].gameObject);
                        goTile.Remove(tiles[NearX, y]);
                        SpawnNewTile(NearX, y, TileType.EMPTY);
                    }
                }
            }
        }

        for (int NearY = y - 1; NearY <= y + 1; NearY++)
        {
            if (NearY != y && NearY >= 0 && NearY < YSize)
            {
                if (tiles[x, NearY].IsClearable)
                {
                    if (tiles[x, NearY].Type == TileType.ICE)
                    {
                        tiles[x, NearY].obstacleDurability -= 1;
                        tiles[x, NearY].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                        if (tiles[x, NearY].obstacleDurability <= 0)
                        {
                            Destroy(tiles[x, NearY].gameObject);
                            goTile.Remove(tiles[x, NearY]);
                            SpawnNewTile(x, NearY, TileType.EMPTY);
                        }
                    }
                    if (tiles[x, NearY].Type == TileType.BOOMB)
                    {
                        Level.OnMove();
                        Destroy(tiles[x, NearY].gameObject);
                        goTile.Remove(tiles[x, NearY]);
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
                if (tiles[x, y].IsCake && (tiles[x, y].Cake == cake))
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
        if (IsNear(SelectedTile, MovingTile) && (GetMatch(SelectedTile, MovingTile.X, MovingTile.Y) != null || GetMatch(MovingTile, SelectedTile.X, SelectedTile.Y) != null))
        {
            SwapTile(SelectedTile, MovingTile);
        }
        else
        {
            SelectedTile.Deselect();
        }
    }

    public bool IsNear(Tile sel, Tile mov)
    {
        return (sel.X == mov.X && Mathf.Abs(sel.Y - mov.Y) == 1)
            || (sel.Y == mov.Y && Mathf.Abs(sel.X - mov.X) == 1);
    }

    public void SwapTile(Tile sel, Tile mov)
    {
        if (GameOver)
            return;

        if (sel.IsMovable && mov.IsMovable)
        {
            tiles[sel.X, sel.Y] = mov;
            tiles[mov.X, mov.Y] = sel;

            if (GetMatch(sel, mov.X, mov.Y) != null || GetMatch(mov, sel.X, sel.Y) != null)
            {
                int selX = sel.X;
                int selY = sel.Y;

                sel.Move(mov.X, mov.Y, FillTime);
                mov.Move(selX, selY, FillTime);

                ClearAllValidMatches();

                SelectedTile = null;
                MovingTile = null;

                StartCoroutine(Fill());

                if (Level.Type != Level.LevelType.SCORETIMER)
                {
                    Level.OnMove();
                }
            }
            else
            {
                tiles[sel.X, sel.Y] = sel;
                tiles[mov.X, mov.Y] = sel;
            }
        }
    }

    public void ShuffleButton()
    {
        StartCoroutine(ShuffleTile());
    }

    private IEnumerator ShuffleTile()
    {
        bool isShuffle = false;
        if (!IsFilling && !isShuffle)
        {
            audioManager.PlaySound(0);
            isShuffle = true;
            for (int i = 0; i < goTile.Count; i++)
            {
                int randomIndex = Random.Range(i, goTile.Count);

                Tile a = goTile[i].GetComponent<Tile>();
                Tile b = goTile[randomIndex].GetComponent<Tile>();

                int tempX = a.X;
                int tempY = a.Y;
                TileType tempType = a.Type;

                a.Move(b.X, b.Y, FillTime);
                b.Move(tempX, tempY, FillTime);

                a.Init(a.X, a.Y, this, a.Type);
                b.Init(tempX, tempY, this, tempType);

                tiles[a.X, a.Y] = a;
                tiles[b.X, b.Y] = b;
            }
            ClearAllValidMatches();

            SelectedTile = null;
            MovingTile = null;

            Level.OnMove();

            StartCoroutine(Fill());
            yield return new WaitForSeconds(FillTime);
            isShuffle = false;
        }
    }


    public Tile SpawnNewTile(int x, int y, TileType type, List<GameObject> list)
    {
        GameObject newTile = SpawnObject(tilePrefabDict[type], list, GetWorldPosition(x, y)); //Instantiate(tilePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newTile.transform.parent = transform;

        tiles[x, y] = newTile.GetComponent<Tile>();
        tiles[x, y].Init(x, y, this, type);
        return tiles[x, y];
    }

    public Tile SpawnNewTile(int x, int y, TileType type)
    {
        GameObject newTile = Instantiate(tilePrefabDict[type], GetWorldPosition(x, y), Quaternion.identity);
        newTile.transform.parent = transform;

        tiles[x, y] = newTile.GetComponent<Tile>();
        tiles[x, y].Init(x, y, this, type);
        return tiles[x, y];
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
                if (tiles[x, y].Type == type)
                {
                    tilesOfType.Add(tiles[x, y]);
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

