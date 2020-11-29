using UnityEngine;
using UnityEngine.UI;

public class PassPanel : MonoBehaviour
{
    [SerializeField] Sprite[] passSprites = default;
    private Image image;

    public void Show(Stone.ColorType color)
    {
        image.sprite = passSprites[(int)color];
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }

    public void Awake()
    {
        image = GetComponent<Image>();
    }
}
