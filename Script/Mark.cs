using UnityEngine;

public class Mark : MonoBehaviour
{
    [SerializeField] Sprite[] colorSprites = default;

    public enum ColorType { Black = 0, White = 1 }
    protected RectTransform rectTransform;
    private SpriteRenderer spriteRenderer;
    private ColorType color;

    public ColorType Color
    {
        get => color;
        set
        {
            color = value;
            spriteRenderer.sprite = colorSprites[(int)color];
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
}
