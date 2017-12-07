using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuffle : MonoBehaviour {

    Grid grid;

    private void Awake()
    {
        grid = Grid.Instance;
    }
    
    public void ClickShuffle()
    {
        StartCoroutine(ShuffleTile());
    }

    private IEnumerator ShuffleTile()
    {
        bool isShuffle = false;
        if (!grid.IsFilling && !isShuffle)
        {
            AudioManager.Instance.PlaySound(0);
            isShuffle = true;
            for (int i = 0; i < grid.goTile.Count; i++)
            {
                int randomIndex = Random.Range(i, grid.goTile.Count);

                Tile a = grid.goTile[i].GetComponent<Tile>();
                Tile b = grid.goTile[randomIndex].GetComponent<Tile>();

                int tempX = a.X;
                int tempY = a.Y;
                Grid.TileType tempType = a.Type;

                a.Move(b.X, b.Y, grid.FillTime);
                b.Move(tempX, tempY, grid.FillTime);

                a.Init(a.X, a.Y, grid, a.Type);
                b.Init(tempX, tempY, grid, tempType);

                grid.tiles[a.X, a.Y] = a;
                grid.tiles[b.X, b.Y] = b;
            }

            grid.ClearAllValidMatches();

            grid.SelectedTile = null;
            grid.MovingTile = null;

            grid.Level.OnMove();

            StartCoroutine(grid.Fill());
            yield return new WaitForSeconds(grid.FillTime);
            isShuffle = false;
        }
    }
}
