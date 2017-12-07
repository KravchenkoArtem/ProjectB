using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossibleMatch : MonoBehaviour
{
    [SerializeField]
    Grid grid;

    [SerializeField]
    List<Tile> match = new List<Tile>();
    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetPossible(match);
        }
    }


    private bool GetPossible(List<Tile> tile)
    {   
        for (int x = 0; x < grid.XSize; x++)
        {
            for (int y = 0; y < grid.YSize; y++)
            {
                if (matchPattern(x, y, new int[,] { { 1, 0 } }, new int[,] { { -2, 0 }, { -1, -1 }, { -1, 1 }, { 2, -1 }, { 2, 1 }, { 3, 0 } }))
                {
                    tile.Add(grid.tiles[x, y]);
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 2, 0 } }, new int[,] { { 1, -1 }, { 1, 1 } }))
                {
                    tile.Add(grid.tiles[x, y]);
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 0, 1 } }, new int[,] { { 0, -2 }, { -1, -1 }, { 1, -1 }, { -1, 2 }, { 1, 2 }, { 0, 3 } }))
                {
                    tile.Add(grid.tiles[x, y]);
                    return true;
                }
                if (matchPattern(x, y, new int[,] { { 0, 2 } }, new int[,] { { -1, 1 }, { 1, 1 } }))
                {
                    tile.Add(grid.tiles[x, y]);
                    return true;
                }
            }
        }
        return false;
    }

    public bool matchPattern(int x, int y, int[,] mustHave, int[,] needOne)
    {
        Tile.CakeType thisType = grid.tiles[x, y].Cake;

        for (int i = 0; i < mustHave.Length - 1; i++)
        {
            if (!matchType(x + mustHave[i, 0], y + mustHave[i, 1], thisType))
            {
                return false;
            }
            Debug.Log("false");
        }

        for (int i = 0; i < needOne.Length; i++)
        {
            if (matchType(x + needOne[i, 0], y + needOne[i, 1], thisType))
            {
                return true;
            }
        }
        return false;
    }

    public bool matchType(int x, int y, Tile.CakeType type)
    {
        if ((x < 0) || (x > grid.XSize - 1) || (y < 0) || (y > grid.YSize - 1)) return false;
        return (grid.tiles[x, y].Cake == type);
    }
}
