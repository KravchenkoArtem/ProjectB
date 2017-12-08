using System.Collections.Generic;
using UnityEngine;

public class PossibleMatch : MonoBehaviour
{
    Grid grid;
    List<Tile> match = new List<Tile>();
    Shuffle shuffle;

    float timer;

    float remainingTimeHelp = 3.5f;

    private void Awake()
    {
        grid = GetComponent<Grid>();
        shuffle = GetComponent<Shuffle>();
    }

    private void Update()
    {
        if (!grid.IsFilling)
        {
            bool light = false;
            timer += Time.deltaTime;
            if (!Input.GetMouseButtonDown(0) && timer >= remainingTimeHelp && !light)
            {
                HighlitedPossibleTile(true);
                light = true;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                HighlitedPossibleTile(false);
                light = false;
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
        Tile.CakeType thisCakeType = grid.tiles[x, y].Cake;

        List<Tile> mustHaveTiles = new List<Tile>();

        for (int i = 0; i < mustHave.GetLength(0); i++)
        {
            if (!matchType(x + mustHave[i, 0], y + mustHave[i, 1], thisCakeType, Grid.TileType.NORMAL))
            {
                return false;
            }
            mustHaveTiles.Add(grid.tiles[x + mustHave[i, 0], y + mustHave[i, 1]]);
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
                match.Add(grid.tiles[x, y]);
                match.Add(grid.tiles[x + needOne[i, 0], y + needOne[i, 1]]);
                return true;
            }
        }
        return false;
    }

    private bool matchType(int x, int y, Tile.CakeType typeCake, Grid.TileType typeTile)
    {
        if ((x < 0) || (x > grid.XSize - 1) || (y < 0) || (y > grid.YSize - 1)) return false;
        //if (!grid.tiles[x, y].IsCake) return false;
        return (grid.tiles[x, y].Cake == typeCake && grid.tiles[x,y].Type == typeTile);
    }
}

