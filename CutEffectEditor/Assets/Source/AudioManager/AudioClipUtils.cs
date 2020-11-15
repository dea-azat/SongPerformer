using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Cysharp.Threading.Tasks;

public class AudioClipUtils
{
    private AudioClip clip = null;
    private AudioClip originalClip = null;

    public AudioClip GetAudioClip()
    {
        return clip;
    }

    public void SetAudioClip(AudioClip _clip)
    {
        originalClip = clip = _clip;
    }

    public async UniTask<AudioClip> LoadWithWebRequest(string filename, AudioType audioType = AudioType.OGGVORBIS)
    {
        originalClip = clip = await AudioLoader.LoadByWebRequest(filename, audioType);

        return originalClip;
    }

    public float[] GetData()
    {
        return GetData(clip);
    }

    private float[] GetData(AudioClip audioClip)
    {
        if (audioClip == null) return new float[0];

        int dataLength = (int)(audioClip.frequency * audioClip.length);
        float[] data = new float[dataLength * audioClip.channels];

        audioClip.GetData(data, 0);

        return data;
    }

    public void SetFadeOutCurve()
    {
        float[] data = GetData(originalClip);

        if (data.Length == 0) return;

        for (int i = 0; i < data.Length; i++)
        {
            float rate = (float)(data.Length - i) / (float)data.Length;

            data[i] = data[i] * (rate * (2 - rate)); //http://marupeke296.sakura.ne.jp/TIPS_No19_interpolation.html
        }

        clip = AudioClip.Create("effectedclip", data.Length, clip.channels, clip.frequency, false);
        clip.SetData(data, 0);
    }
}

public static class AudioLoader
{
    public static async UniTask<AudioClip> LoadByWebRequest(string filename, AudioType audioType = AudioType.WAV)
    {
        AudioClip clip = null;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filename, audioType))
        {
            await www.SendWebRequest(); // must wait
            clip = DownloadHandlerAudioClip.GetContent(www);
        }

        return clip;
    }
}