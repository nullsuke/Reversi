using UnityEngine;
using UnityEngine.UI;

public class ResultPanel : MonoBehaviour
{
    [SerializeField] Image image = default;
    [SerializeField] Sprite[] resultSprites = default;

    public void Show(int n)
    {
        image.sprite = resultSprites[n];
    }

    public void SetActive(bool value)
    {
        gameObject.SetActive(value);
    }
}
