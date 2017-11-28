using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public enum CakeType
    {
        BLACKCAKE, CROISSANTCAKE, PINKCAKE, REDCAKE, WHITECAKE, BIGCAKE, YELLOWCAKE
    };

    public int score;
    public int X;
    public int Y;

    public Grid.TileType Type;

    public Grid grid;

    public bool IsClearable = false;
    public bool isBeginCleared = false;
    [SerializeField]
    private AnimationClip clearAnimation;
    [SerializeField]
    private GameObject Selector;

    public bool IsMovable = false;
    private IEnumerator moveCoroutine;

    public bool IsCake = false;

    [System.Serializable]
    public struct CakeSprite
    {
        public CakeType cake;
        public Sprite sprite;
    };

    public CakeSprite[] cakeSprites;

    private CakeType cake;

    public CakeType Cake
    {
        get { return cake; }
        set { SetCake(value); }
    }

    public int NumCakes
    {
        get { return cakeSprites.Length; }
    }

    private SpriteRenderer sprite;

    private Dictionary<CakeType, Sprite> cakeSpriteDict;

    [HideInInspector]
    public int obstacleDurability = 2;

    private void Awake()
    {
        grid = Grid.instance;

        if (!IsCake)
            return;

        sprite = GetComponent<SpriteRenderer>();
        cakeSpriteDict = new Dictionary<CakeType, Sprite>();

        for (int i = 0; i < cakeSprites.Length; i++)
        {
            if (!cakeSpriteDict.ContainsKey(cakeSprites[i].cake))
            {
                cakeSpriteDict.Add(cakeSprites[i].cake, cakeSprites[i].sprite);
            }
        }
    }

    public void Init(int _x, int _y, Grid _grid, Grid.TileType _type)
    {
        X = _x;
        Y = _y;
        grid = _grid;
        Type = _type;
    }

    public void Move(int newX, int newY, float time)
    {
        if (!IsMovable)
            return;

        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = MoveCoroutine(newX, newY, time);
        StartCoroutine(moveCoroutine);
    }

    private IEnumerator MoveCoroutine(int newX, int newY, float time)
    {
        X = newX;
        Y = newY;
        Vector3 startPos = transform.position;
        Vector3 endPos = grid.GetWorldPosition(newX, newY);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }
        transform.position = grid.GetWorldPosition(newX, newY);
    }

    public virtual void Clear()
    {
        if (!IsClearable)
            return;

        grid.level.OnTileCleared(this);

        isBeginCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        Animator animator = GetComponent<Animator>();

        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }

    public void SetCake(CakeType newCake)
    {
        cake = newCake;

        if (cakeSpriteDict.ContainsKey(newCake))
        {
            sprite.sprite = cakeSpriteDict[newCake];
        }
    }

    public void OnMouseOver()
    {
        if (grid.IsFilling)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!grid.selectedTile)
            {
                grid.SelectTile(this);
                Select();
            }
            else if (grid.selectedTile != transform && !grid.movedTile)
            {
                grid.MoveTile(this);
                grid.MovedTile();
                Deselect();
            }
        }
    }

    public void Select()
    {
        Selector.SetActive(true);
    }

    public void Deselect()
    {
        Selector.SetActive(false);
        grid.selectedTile = null;
        grid.movedTile = null;
    }
}
