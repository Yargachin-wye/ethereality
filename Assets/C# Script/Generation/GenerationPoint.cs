using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GenerationPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] float spawnerDistance = 20;
    public BotsList _tipe;
    public BotsList tipe => _tipe;
    public Vector3 position => transform.position;
    
    private bool tipeSet = false;
    
    public void SetColor(Color color)
    {
        tipeSet = true;
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
