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

    private Grid grid;

    public bool IsClearable = false;
    public bool IsBeginCleared = false;

    [SerializeField]
    private AnimationClip clearAnimation;
    Animator animator;

    [SerializeField]
    private GameObject selector;

    public bool IsMovable = false;
    private IEnumerator moveCoroutine;

    public bool IsCake = false;

    [System.Serializable]
    public struct CakeSprite
    {
        public CakeType cake;
        public Sprite sprite;
    };

    public CakeSprite[] CakeSprites;

    private CakeType cake;

    public CakeType Cake
    {
        get { return cake; }
        set { SetCake(value); }
    }

    public int NumCakes
    {
        get { return CakeSprites.Length; }
    }

    private SpriteRenderer sprite;

    public Dictionary<CakeType, Sprite> cakeSpriteDict;

    public int obstacleDurability = 2;

    private void Awake()
    {
        grid = Grid.Instance;
        animator = GetComponent<Animator>();

        if (!IsCake)
            return;

        sprite = GetComponent<SpriteRenderer>();
        cakeSpriteDict = new Dictionary<CakeType, Sprite>();

        for (int i = 0; i < CakeSprites.Length; i++)
        {
            if (!cakeSpriteDict.ContainsKey(CakeSprites[i].cake))
            {
                cakeSpriteDict.Add(CakeSprites[i].cake, CakeSprites[i].sprite);
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

    public virtual void Clear(List<GameObject> list)
    {
        if (!IsClearable)
            return;

        grid.Level.OnTileCleared(this);

        IsBeginCleared = true;
        StartCoroutine(ClearCoroutine(list));
    }

    private IEnumerator ClearCoroutine(List<GameObject> list)
    {
        if (animator)
        {
            animator.Play(clearAnimation.name, -1, 0);
            yield return new WaitForSeconds(clearAnimation.length);
            OnDespawnObject(list);
        }
    }

    public virtual void Clear()
    {
        if (!IsClearable)
            return;

        grid.Level.OnTileCleared(this);

        IsBeginCleared = true;
        StartCoroutine(ClearCoroutine());
    }

    private IEnumerator ClearCoroutine()
    {
        if (animator)
        {
            animator.Play(clearAnimation.name);
            yield return new WaitForSeconds(clearAnimation.length);
            Destroy(gameObject);
        }
    }

    private void OnDespawnObject(List<GameObject> list)
    {
        if (list.Count == 0)
        {
            return;
        }
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].GetInstanceID() == gameObject.GetInstanceID())
            {
                Reset();
                PrefabPoolingSystem.Despawn(list[i]);
                list.RemoveAt(i);
            }
        }
    }

    private void Reset()
    {
        IsBeginCleared = false;
        animator.Play(clearAnimation.name, -1, 0);
        if (Type == Grid.TileType.NORMAL)
        {
            Deselect();
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
        if (grid.IsFilling || !IsCake)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            if (!grid.SelectedTile)
            {
                grid.SelectTile(this);
                Select();
            }
            else if (grid.SelectedTile != transform && !grid.MovingTile)
            {
                grid.MoveTile(this);
                grid.MovedTile();
                Deselect();
            }
        }
    }

    public void Select()
    {
        selector.SetActive(true);
        animator.Play("SelectorAnim");
    }

    public void Deselect()
    {
        selector.SetActive(false);
        grid.SelectedTile = null;
        grid.MovingTile = null;
    }
}
