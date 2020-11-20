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
        SetWave(DOWN_EFFECT_PATH);
    }

    public async void SetWave(string path)
    {
        AudioClipUtils audioClip = new AudioClipUtils();
        await audioClip.LoadByWebRequest(path, AudioType.WAV);
        graphRenderer.Render(audioClip);
    }

    public void SetWave(AudioClip clip)
    {
        AudioClipUtils audioClip = new AudioClipUtils();
        audioClip.SetAudioClip(clip);
        graphRenderer.Render(audioClip);
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
