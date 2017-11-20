using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameTile : MonoBehaviour
{
    [SerializeField]
    private GameObject SelectorSprite;
    private static GameTile previousSelected = null;

    private SpriteRenderer render;
    private bool isSelected = false;

    private Vector2[] adjacentDirections = new Vector2[] {
        Vector2.up,
        Vector2.down,
        Vector2.left,
        Vector2.right };

    void Awake()
    {
        render = GetComponent<SpriteRenderer>();
    }

    private void Select()
    {
        isSelected = true;
        SelectorSprite.SetActive(true);
        previousSelected = gameObject.GetComponent<GameTile>();
    }

    private void Deselect()
    {
        isSelected = false;
        SelectorSprite.SetActive(false);
        previousSelected = null;
    }

    private void OnMouseDown()
    {
        if (render.sprite == null || GameBoardManager.instance.IsShuffle)
        {
            return;
        }

        if (isSelected)
        { // Is it already selected?
            Deselect();
        }
        else
        {
            if (previousSelected == null)
            { // Is it the first tile selected?
                Select();
            }
            else
            {
                if (GetAllAdjacentTiles().Contains(previousSelected.gameObject))
                { // Is it an adjacent tile?
                    SwapSprite(previousSelected.render);
                    previousSelected.Deselect();
                }
                else
                {
                    previousSelected.GetComponent<GameTile>().Deselect();
                    Select();
                }
            }
        }

    }

    private GameObject GetAdjacent(Vector2 castDir)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, castDir);
        if (hit.collider != null)
        {
            return hit.collider.gameObject;
        }
        return null;
    }

    private List<GameObject> GetAllAdjacentTiles()
    {
        List<GameObject> adjacentTiles = new List<GameObject>();
        for (int i = 0; i < adjacentDirections.Length; i++)
        {
            adjacentTiles.Add(GetAdjacent(adjacentDirections[i]));
        }
        return adjacentTiles;
    }



    public void SwapSprite(SpriteRenderer render2)
    {
        if (render.sprite == render2.sprite)
            return;

        Sprite tempSprite = render2.sprite;
        render2.sprite = render.sprite;
        render.sprite = tempSprite;
        // Play some sound.
    }
}
