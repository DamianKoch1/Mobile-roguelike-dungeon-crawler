using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MobileInput : MonoBehaviour
{
    [SerializeField]
    private Image stick;

    [SerializeField]
    private Image stickHandle;

    [SerializeField]
    private Image actionBtn;

    private Vector2 dragOffset;

    [SerializeField]
    private float maxStickOffset = 10;

    private Canvas canvas;

    [SerializeField]
    private bool keyboardMode;

    public static Vector2 Axes { private set; get; }

    public static bool ActionDown { private set; get; }

    public static bool ActionUp { private set; get; }

    public static bool Action { private set; get; }

    void Start()
    {
        canvas = GetComponentInParent<Canvas>();
        Axes = new Vector2();
    }

    private void Update()
    {
        if (!keyboardMode) return;

        Axes = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        ActionDown = Input.GetButtonDown("Jump");
        Action = Input.GetButton("Jump");
        ActionUp = Input.GetButtonUp("Jump");
    }


    private void LateUpdate()
    {
        if (ActionDown) ActionDown = false;
        if (ActionUp) ActionUp = false;
    }

    public void OnBeginDragStick(BaseEventData data)
    {
        dragOffset = stick.rectTransform.anchoredPosition - ((PointerEventData)data).position;
    }

    public void OnDragStick(BaseEventData data)
    {
        var stickPos = (((PointerEventData)data).position + dragOffset) / canvas.scaleFactor;
        var magnitude = stickPos.magnitude;
        if (magnitude == 0)
        {
            Axes = Vector2.zero;
        }
        else
        {
            if (magnitude > maxStickOffset)
            {
                stickPos.Normalize();
                Axes = stickPos;
                stickPos *= maxStickOffset;
            }
            else
            {
                Axes = stickPos / maxStickOffset;
            }
        }
        stick.rectTransform.anchoredPosition = stickPos;
    }

    public void OnEndDragStick(BaseEventData data)
    {
        Axes = Vector2.zero;
        stick.rectTransform.anchoredPosition = Vector3.zero;
    }

    public void OnActionPointerDown(BaseEventData data)
    {
        ActionDown = true;
        Action = true;
        actionBtn.color = new Color(0.7f, 0.7f, 0.7f);
    }

    public void OnActionPointerUp(BaseEventData data)
    {
        ActionUp = true;
        Action = false;
        actionBtn.color = Color.white;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(stick.transform.position, maxStickOffset * GetComponentInParent<Canvas>().scaleFactor);
    }
}
