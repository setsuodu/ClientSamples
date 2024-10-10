using TMPro;
using UnityEngine;

public class GridView2D : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public TMP_Text text_G;
    public TMP_Text text_H;
    public TMP_Text text_F;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        text_G = transform.Find("G").GetComponent<TMP_Text>();
        text_H = transform.Find("H").GetComponent<TMP_Text>();
        text_F = transform.Find("F").GetComponent<TMP_Text>();
    }

    public void Refresh()
    {
        
    }
}