using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    public int _tipe;
    public int tipe => _tipe;
    public void SetColor(Color color, int i)
    {
        _tipe = i;
        spriteRenderer.color = color;
    }
    public void SetSize(float size)
    {
        transform.localScale = new Vector3(size, size, 1);
    }
    public void SetPosition(Vector3 pos)
    {
        transform.position = pos;
    }
}
