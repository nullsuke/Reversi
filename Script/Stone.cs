using System.Collections;
using UnityEngine;

public class Stone : Mark
{
    [SerializeField] float reverseAngle = 0f;
    private Vector3 rotatePosition = new Vector3(0.5f, 0.5f, 0);

    public int X { get; set; }
    public int Y { get; set; }
    public bool HasReversed { get; set; }
    
    public static ColorType GetReverseColor(ColorType color)
    {
        int c = (int)color ^ 1;
        return (ColorType)c;
    }

    public void Reverse()
    {
        HasReversed = false;
        StartCoroutine(CoroutineReverse());
    }

    private IEnumerator CoroutineReverse()
    {
        float angle = 0f;
        
        while (angle < 90f)
        {
            transform.RotateAround(transform.TransformPoint(rotatePosition),
                new Vector3(0, 1, 0), reverseAngle);
            angle += reverseAngle;
            
            yield return null;
        }
        
        ChangeColor();

        while (angle < 180f)
        {
            transform.RotateAround(transform.TransformPoint(rotatePosition),
                new Vector3(0, 1, 0), reverseAngle);
            angle += reverseAngle;
            
            yield return null;
        }

        HasReversed = true;
    }

    private void ChangeColor()
    {
        int c = (int)Color ^ 1;
        Color = (ColorType)c;
    }
}
