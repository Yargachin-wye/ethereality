using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture : MonoBehaviour
{
    [SerializeField]
    private List<Vector2> parallaxSmove;// Õ≈ ƒŒÀ∆ÕŒ ¡€“‹ 0 Õ≈ œ–»  ¿ »’ Œ¡—“Œﬂ“≈À‹—“¬¿’

    public List<Material> materials;

    private Transform cameraTransform;

    Vector2 offset = Vector2.zero;

    private void Start()
    {
        cameraTransform = transform.root;
    }
    private void LateUpdate()
    {
        for ( int i = 0; i < parallaxSmove.Count; i++)
        {
            offset = new Vector2(cameraTransform.position.x / 100f / parallaxSmove[i].x, cameraTransform.position.y / 100f / parallaxSmove[i].y);

            materials[i].mainTextureOffset = offset;
        }
    }
}
