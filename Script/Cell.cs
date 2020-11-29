using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class Cell : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] Stone stonePrefab = default;
    [SerializeField] Mark markPrefab = default;
    [SerializeField] RectTransform movedPrefab = default;
    //セルをクリックしたときのイベント
    public event EventHandler<EventArgs> PointerClick;
    private RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;

    public int X { get => (int)rectTransform.anchoredPosition.x; }
    //リバーシ盤に合わせるためY軸は反転させる
    public int Y { get => -(int)rectTransform.anchoredPosition.y; }

    public void Setup(Vector3 vector, Sprite sprite)
    {
        //リバーシ盤に合わせるため左上を原点にする
        rectTransform.anchoredPosition = vector;
        spriteRenderer.sprite = sprite;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClick?.Invoke(this, EventArgs.Empty);
    }

    public Stone CreateStone(Stone.ColorType color)
    {
        var stone = Instantiate(stonePrefab, transform, false);
        stone.X = X;
        stone.Y = Y;
        stone.Color = color;

        return stone;
    }
    
    public Mark CreateMark(Stone.ColorType color)
    {
        var mark = Instantiate(markPrefab, transform, false);
        mark.Color = color;

        return mark;
    }

    public RectTransform CreateMoved()
    {
        return Instantiate(movedPrefab, transform, false);
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
