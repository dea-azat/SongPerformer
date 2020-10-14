using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipTest : MonoBehaviour
{

    const string TEST_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";

    // Start is called before the first frame update
    async void Start()
    {

        AudioSource audioSource = gameObject.AddComponent<AudioSource>();

        AudioClipController controller = new AudioClipController();
        await controller.LoadAudioClipWithWebRequest(TEST_EFFECT_PATH, AudioType.WAV);

        controller.SetFadeOutCurve();

        audioSource.clip = controller.GetAudioClip();

        audioSource.Play();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
