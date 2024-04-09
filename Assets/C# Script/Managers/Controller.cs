using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Controller : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Camera mainCamera;
    [SerializeField] private float _timeCd = 0.1f;

    [SerializeField] private FloatingJoystick _joystick;
    [SerializeField] private Image _stick;
    [SerializeField] private Image _mushroom;

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
    private void Start()
    {
        Stick(false);
        Stick(true);
        Stick(false);
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
            Stick(true);
            dashed = false;
            Harpoon.instance.OpenJaw();
            Vector2 touchPosition = eventData.position;
            // Vector3 worldPosition = mainCamera.ScreenToWorldPoint(new Vector3(touchPosition.x, touchPosition.y, mainCamera.nearClipPlane));
            Harpoon.instance.targetDirection = -1 * _joystick.Direction;
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        Stick(false);

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
    private void Stick(bool b)
    {
        Harpoon.instance.rotating = b;
        Color c1 = _stick.color, c2 = _mushroom.color;
        if (b)
        {
            _stick.color = new Color(c1.r, c1.g, c1.b, 0.5f);
            _mushroom.color = new Color(c2.r, c2.g, c2.b, 0.5f);
        }
        else
        {
            _stick.color = new Color(c1.r, c1.g, c1.b, 0);
            _mushroom.color = new Color(c2.r, c2.g, c2.b, 0);
        }
    }
    IEnumerator Timer()
    {
        timer++;
        yield return new WaitForSeconds(_timeCd);
        timer--;
    }
}