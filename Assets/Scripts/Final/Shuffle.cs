using System.Collections;
using UnityEngine;

public class Shuffle : MonoBehaviour
{
    Grid grid;

    private void Awake()
    {
        grid = Grid.Instance;
    }

    public void ClickShuffle()
    {
        StartCoroutine(ShuffleTile(true));
    }

    public void ShuffleIfPossibleFalse()
    {
        StartCoroutine(ShuffleTile(false));
    }

    private IEnumerator ShuffleTile(bool onMove)
    {
        bool isShuffle = false;
        if (!grid.IsFilling && !isShuffle)
        {
            AudioManager.Instance.PlaySound(0);
            isShuffle = true;
            for (int i = 0; i < grid.GoTile.Count; i++)
            {
                int randomIndex = Random.Range(i, grid.GoTile.Count);

                Tile a = grid.GoTile[i].GetComponent<Tile>();
                Tile b = grid.GoTile[randomIndex].GetComponent<Tile>();

                int tempX = a.X;
                int tempY = a.Y;
                Grid.TileType tempType = a.Type;

                a.Move(b.X, b.Y, grid.FillTime * 1.5f);
                b.Move(tempX, tempY, grid.FillTime * 1.5f);

                a.Init(a.X, a.Y, grid, a.Type);
                b.Init(tempX, tempY, grid, tempType);

                grid.Tiles[a.X, a.Y] = a;
                grid.Tiles[b.X, b.Y] = b;
            }

            grid.ClearAllValidMatches();

            grid.SelectedTile = null;
            grid.MovingTile = null;

            if (onMove)
                grid.Level.OnMove();

            StartCoroutine(grid.Fill());
            yield return new WaitForSeconds(grid.FillTime * 1.5f);
            isShuffle = false;
        }
    }
}
