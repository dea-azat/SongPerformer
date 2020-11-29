using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System;

public class SlicedBlock : MonoBehaviour, IBeginDragHandler, IDragHandler 
{
    public UISlicer slicer;

    public RectTransform rect;


    private float startTime = 0;
    private float endTime = 0.46f;

    [Serializable] public class PlayEvent : UnityEvent<float, float> { }
    [SerializeField] public PlayEvent OnPlayClick;

    enum DRAG_MODE
    {
        LEFT,
        RIGHT,
        NONE
    }

    private void Start()
    {

    }

    public void SetPosition(float startX, float endX)
    {
        rect.pivot = Vector2.zero;
        rect.anchoredPosition = new Vector2(startX, 0);
        rect.sizeDelta = new Vector2(endX - startX, 0);

        startTime = SlicerLibrary.ConvertPointToTime(rect.anchoredPosition.x, slicer.waveViz);
        endTime = SlicerLibrary.ConvertPointToTime(rect.anchoredPosition.x + rect.sizeDelta.x, slicer.waveViz);
    }

    DRAG_MODE drag_mode = DRAG_MODE.NONE;
    public void OnBeginDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out localPointerPosition);

        Debug.Log(localPointerPosition.x);

        int margin = 40;

        if (localPointerPosition.x < margin)
        {
            drag_mode = DRAG_MODE.LEFT;
            return;
        }

        if (localPointerPosition.x > rect.sizeDelta.x - margin)
        {
            drag_mode = DRAG_MODE.RIGHT;
            return;
        }

        drag_mode = DRAG_MODE.NONE;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPointerPosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, eventData.position, eventData.pressEventCamera, out localPointerPosition);

        float offsetX = localPointerPosition.x;
        float diff = localPointerPosition.x - rect.sizeDelta.x;

        switch (drag_mode)
        {
            case DRAG_MODE.LEFT:
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + offsetX, rect.anchoredPosition.y);
                rect.sizeDelta = new Vector2(rect.sizeDelta.x - offsetX, rect.sizeDelta.y);
                break;

            case DRAG_MODE.RIGHT:
                rect.sizeDelta = new Vector2(offsetX, rect.sizeDelta.y);
                break;

            case DRAG_MODE.NONE:
                break;

            default:
                break;

        }

        startTime = SlicerLibrary.ConvertPointToTime(rect.anchoredPosition.x, slicer.waveViz);
        endTime = SlicerLibrary.ConvertPointToTime(rect.anchoredPosition.x + rect.sizeDelta.x, slicer.waveViz);
    }

    public void OnClick(bool onoff)
    {
        Debug.Log("startTime = " + startTime + " endTime = " + endTime);
        OnPlayClick.Invoke(startTime, endTime);
    }
}


