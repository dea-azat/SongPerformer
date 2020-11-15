using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISlicer : MonoBehaviour
{

    public RectTransform contentRect;
    public GraphRenderer graphRenderer;

    private float visualizerWidth = 2000;
    const float WIDTH_MIN = 1000;
    const float WIDTH_MAX = 16000;

    // Start is called before the first frame update
    void Start()
    {
        contentRect.sizeDelta = new Vector2(4000, contentRect.sizeDelta.y);
        graphRenderer.width = 4000;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlusButtonClick()
    {
        if (visualizerWidth >= WIDTH_MAX) return;
        visualizerWidth *= 2;
        contentRect.sizeDelta = new Vector2(visualizerWidth, contentRect.sizeDelta.y);
        graphRenderer.width = visualizerWidth;
        graphRenderer.UpdateGraph();
    }

    public void OnMinusButtonClick()
    {
        if (visualizerWidth <= WIDTH_MIN) return;
        visualizerWidth /= 2;
        contentRect.sizeDelta = new Vector2(visualizerWidth, contentRect.sizeDelta.y);
        graphRenderer.width = visualizerWidth;
        graphRenderer.UpdateGraph();
    }
}
