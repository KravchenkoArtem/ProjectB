using System.Collections.Generic;
using UnityEngine;

public class Matching : MonoBehaviour
{
    private Grid grid;
    private List<Tile> match = new List<Tile>();
    private Shuffle shuffle;

    private float timer;
    [Range(2, 5)]
    [SerializeField]
    private float remainingTimeTint = 3.5f;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        shuffle = GetComponent<Shuffle>();
    }

    private void Update()
    {
        if (!grid.IsFilling)
        {
            bool b = false;
            timer += Time.deltaTime;
            if (!Input.GetMouseButtonDown(0) && timer >= remainingTimeTint && !b)
            {
                HighlitedPossibleTile(true);
                b = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                HighlitedPossibleTile(false);
                b = false;
                timer = 0;
            }
        }
    }

    public void HighlitedPossibleTile(bool highlighted)
    {
        GetPossible();
        if (highlighted)
        {
            for (int i = 0; i < match.Count; i++)
            {
                match[i].GetComponent<Animator>().SetBool("Highlight", true);
            }
        }
        else
        {
            for (int i = 0; i < match.Count; i++)
            {
                match[i].GetComponent<Animator>().SetBool("Highlight", false);
            }
            match.Clear();
        }
    }

    private bool GetPossible()
    {
        for (int x = 0; x < grid.XSize; x++)
        {
            for (int y = 0; y < grid.YSize; y++)
            {
                if (matchPattern(x, y, new int[,] { { 1, 0 } }, new int[,] { { -1, 0 }, { 2, 0 } }, new int[,] { { -2, 0 }, { -1, -1 }, { -1, 1 }, { 2, -1 }, { 2, 1 }, { 3, 0 } }))
                {
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 2, 0 } }, new int[,] { { 1, 0 } }, new int[,] { { 1, -1 }, { 1, 1 } }))
                {
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 0, 1 } }, new int[,] { { 0, 2 }, { 0, -1 } }, new int[,] { { 0, -2 }, { -1, -1 }, { 1, -1 }, { -1, 2 }, { 1, 2 }, { 0, 3 } }))
                {
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 0, 2 } }, new int[,] { { 0, 1 } }, new int[,] { { -1, 1 }, { 1, 1 } }))
                {
                    return true;
                }
            }
        }
        timer = 0;
        shuffle.ShuffleIfPossibleFalse();
        return false;
    }

    private bool matchPattern(int x, int y, int[,] mustHave, int[,] checkWrongTile, int[,] needOne)
    {
        Tile.CakeType thisCakeType = grid.Tiles[x, y].Cake;

        List<Tile> mustHaveTiles = new List<Tile>();

        for (int i = 0; i < mustHave.GetLength(0); i++)
        {
            if (!matchType(x + mustHave[i, 0], y + mustHave[i, 1], thisCakeType, Grid.TileType.NORMAL))
            {
                return false;
            }
            mustHaveTiles.Add(grid.Tiles[x + mustHave[i, 0], y + mustHave[i, 1]]);
        }

        for (int i = 0; i < checkWrongTile.GetLength(0); i++)
        {
            if (matchType(x + checkWrongTile[i, 0], y + checkWrongTile[i, 1], Tile.CakeType.NULL, Grid.TileType.ICE)
                || matchType(x + checkWrongTile[i, 0], y + checkWrongTile[i, 1], Tile.CakeType.NULL, Grid.TileType.GRIDSPACE)
                || matchType(x + checkWrongTile[i, 0], y + checkWrongTile[i, 1], Tile.CakeType.NULL, Grid.TileType.EMPTY) 
                || matchType(x + checkWrongTile[i, 0], y + checkWrongTile[i, 1], Tile.CakeType.NULL, Grid.TileType.BOOMB))
            {
                return false;
            }
        }

        for (int i = 0; i < needOne.GetLength(0); i++)
        {
            if (matchType(x + needOne[i, 0], y + needOne[i, 1], thisCakeType, Grid.TileType.NORMAL))
            {
                match.AddRange(mustHaveTiles);
                match.Add(grid.Tiles[x, y]);
                match.Add(grid.Tiles[x + needOne[i, 0], y + needOne[i, 1]]);
                return true;
            }
        }
        return false;
    }

    private bool matchType(int x, int y, Tile.CakeType typeCake, Grid.TileType typeTile)
    {
        if ((x < 0) || (x > grid.XSize - 1) || (y < 0) || (y > grid.YSize - 1)) return false;
        //if (!grid.Tiles[x, y].IsCake) return false;
        return (grid.Tiles[x, y].Cake == typeCake && grid.Tiles[x,y].Type == typeTile);
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
                for (int xOffset = 1; xOffset < grid.XSize; xOffset++)
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

                    if (x < 0 || x >= grid.XSize)
                    {
                        break;
                    }

                    if (grid.Tiles[x, newY].IsCake && grid.Tiles[x, newY].Cake == cake)
                    {
                        horizontalTiles.Add(grid.Tiles[x, newY]);
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
                        for (int yOffset = 1; yOffset < grid.YSize; yOffset++)
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

                            if (y < 0 || y >= grid.YSize)
                            {
                                break;
                            }

                            if (grid.Tiles[horizontalTiles[i].X, y].IsCake && grid.Tiles[horizontalTiles[i].X, y].Cake == cake)
                            {
                                verticaltiles.Add(grid.Tiles[horizontalTiles[i].X, y]);
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
                for (int yOffset = 1; yOffset < grid.YSize; yOffset++)
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

                    if (y < 0 || y >= grid.YSize)
                    {
                        break;
                    }

                    if (grid.Tiles[newX, y].IsCake && grid.Tiles[newX, y].Cake == cake)
                    {
                        verticaltiles.Add(grid.Tiles[newX, y]);
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
                        for (int xOffset = 1; xOffset < grid.XSize; xOffset++)
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

                            if (x < 0 || x >= grid.XSize)
                            {
                                break;
                            }

                            if (grid.Tiles[x, verticaltiles[i].Y].IsCake && grid.Tiles[x, verticaltiles[i].Y].Cake == cake)
                            {
                                horizontalTiles.Add(grid.Tiles[x, verticaltiles[i].Y]);
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


    public bool TileIsNear(Tile sel, Tile mov)
    {
        return (sel.X == mov.X && Mathf.Abs(sel.Y - mov.Y) == 1)
            || (sel.Y == mov.Y && Mathf.Abs(sel.X - mov.X) == 1);
    }
}

