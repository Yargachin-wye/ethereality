using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float _timeCd = 0.1f;
    private int timer = 0;
    private bool onColliderButton = false;
    private bool isDragging = false;
    private bool dashed = true;
    public static Controller instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Controller.instance != null");
            enabled = false;
            return;
        }
        instance = this;
    }
    public void OnColliderButtonDown()
    {
        onColliderButton = true;
        StartCoroutine(Timer());
    }
    
    public void OnUp(Vector3 vector)
    {
        
        if (timer == 0 && !dashed)
        {
            Harpoon.instance.Dash();
            
        }
        else
        {
            Harpoon.instance.ShotToVector(vector);
        }
        dashed = true;
        onColliderButton = false;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        StartCoroutine(Timer());
    }
    public void OnDrag(PointerEventData eventData)
    {
        if (timer <= 0 && isDragging)
        {
            if (Harpoon.instance.dashCD)
            {
                return;
            }
            dashed = false;
            Harpoon.instance.OpenJaw();
            Harpoon.instance.rotating = true;

            Vector2 touchPosition = eventData.position;
            Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, mainCamera.nearClipPlane));
            Harpoon.instance.targetPosition = worldPosition;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Harpoon.instance.rotating = false;

        if (onColliderButton)
        {
            isDragging = false;
            return;
        }

        Vector2 touchPosition = eventData.position;
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, mainCamera.nearClipPlane));

        OnUp(worldPosition);
        isDragging = false;
    }
    IEnumerator Timer()
    {
        timer++;
        yield return new WaitForSeconds(_timeCd);
        timer--;
    }
}