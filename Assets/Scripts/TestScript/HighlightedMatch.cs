using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Test Script.
/// </summary>
public class HighlightedMatch : MonoBehaviour
{
    [SerializeField]
    private List<Tile> possibleMatchTile = new List<Tile>();

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetHighlitedMatch();
        }
    }

    public void GetHighlitedMatch() // Частично работающая помойка (только диагональ).
    {
        for (int y = Grid.Instance.YSize - 2; y >= 0; y--)
        {
            for (int loopX = 0; loopX < Grid.Instance.XSize; loopX++)
            {
                int xRight = loopX;

                Tile tile = Grid.Instance.tiles[xRight, y];
                for (int diag = -1; diag <= 1; diag++)
                {
                    if (diag != 0)
                    {
                        int diagXRight = xRight + diag;

                        if (diagXRight >= 0 && diagXRight < Grid.Instance.XSize)
                        {
                            Tile diagonalTileRight = Grid.Instance.tiles[diagXRight, y + 1];
                            if (diagonalTileRight.Cake == tile.Cake)
                            {
                                possibleMatchTile.Add(tile);
                                possibleMatchTile.Add(diagonalTileRight);
                            }
                        }
                    }
                }
                for (int i = 0; i < possibleMatchTile.Count; i++) // Debug
                {
                    //if (grid.GetMatch(tile, xRight, y) != null)
                    //{
                        possibleMatchTile[i].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.5f);
                    //}
                }
            }
        }

    }
}
