using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Texture : MonoBehaviour
{
    [SerializeField]
    private Vector2 parallaxEffectMultiplier;// �� ������ ���� 0 �� ��� ����� ���������������

    public Material material;

    private Transform cameraTransform;

    Vector2 offset = Vector2.zero;

    private void Start()
    {
        cameraTransform = transform.root;
        //material = GetComponent<Renderer>().sharedMaterial;
    }
    private void LateUpdate()
    {
        offset = new Vector2(cameraTransform.position.x / 100f / parallaxEffectMultiplier.x, cameraTransform.position.y / 100f / parallaxEffectMultiplier.y);

        material.mainTextureOffset = offset;
    }
}
