using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UIWaveVisualizer : MonoBehaviour
{

    public RectTransform contentRect;
    public GraphRenderer graphRenderer;

    [Serializable] public class WidthChangeEvent : UnityEvent<int> { }
    [SerializeField] public WidthChangeEvent OnWidthChanged;

    private float visualizerWidth = 2000;
    const float WIDTH_MIN = 1000;
    const float WIDTH_MAX = 8000;

    private AudioClipUtils audioClip = new AudioClipUtils();
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();

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
        const string ALL_PATH = "D:/BeatSaberMod/EffectSamples/GDFH_kit2_break_vocal_chop_loop_124_g_minor.wav";
        SetWave(ALL_PATH);
    }

    public async void SetWave(string path)
    {
        await audioClip.LoadByWebRequest(path, AudioType.WAV);
        graphRenderer.Render(audioClip);
    }

    public void SetWave(AudioClip clip)
    {
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

        OnWidthChanged.Invoke((int)visualizerWidth);
    }

    public void OnMinusButtonClick()
    {
        if (visualizerWidth <= WIDTH_MIN) return;
        visualizerWidth /= 2;
        contentRect.sizeDelta = new Vector2(visualizerWidth, contentRect.sizeDelta.y);
        graphRenderer.width = visualizerWidth;
        graphRenderer.ReRender();

        OnWidthChanged.Invoke((int)visualizerWidth);
    }

    public AudioClipUtils GetAudioClip()
    {
        return audioClip;
    }

    public float GetWidth()
    {
        return visualizerWidth;
    }

    public float GetHeight()
    {
        return contentRect.sizeDelta.y;
    }

    public void Play(float start, float end)
    {
        AudioClip playClip = audioClip.GetAudioClip(start, end);
        playClip = AudioClipUtils.SetFadeOutCurve(playClip);
        audioSource.PlayOneShot(playClip);
    }
}
