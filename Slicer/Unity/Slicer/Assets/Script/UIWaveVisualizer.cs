using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIWaveVisualizer : MonoBehaviour
{

    public RectTransform contentRect;
    public GraphRenderer graphRenderer;

    private float visualizerWidth = 2000;
    const float WIDTH_MIN = 1000;
    const float WIDTH_MAX = 8000;

    // Start is called before the first frame update
    void Start()
    {
        contentRect.sizeDelta = new Vector2(4000, contentRect.sizeDelta.y);
        graphRenderer.width = 4000;

        TestGraphRender();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void TestGraphRender()
    {
        const string DOWN_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";
        graphRenderer.Render(DOWN_EFFECT_PATH);
    }

    public void SetWave(string path)
    {
        graphRenderer.Render(path);
    }

    public void OnPlusButtonClick()
    {
        if (visualizerWidth >= WIDTH_MAX) return;
        visualizerWidth *= 2;
        contentRect.sizeDelta = new Vector2(visualizerWidth, contentRect.sizeDelta.y);
        graphRenderer.width = visualizerWidth;
        graphRenderer.ReRender();
    }

    public void OnMinusButtonClick()
    {
        if (visualizerWidth <= WIDTH_MIN) return;
        visualizerWidth /= 2;
        contentRect.sizeDelta = new Vector2(visualizerWidth, contentRect.sizeDelta.y);
        graphRenderer.width = visualizerWidth;
        graphRenderer.ReRender();
    }
}
