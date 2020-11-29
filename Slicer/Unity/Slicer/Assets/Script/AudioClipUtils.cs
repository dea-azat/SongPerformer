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

    public async UniTask<AudioClip> LoadByWebRequest(string filename, AudioType audioType = AudioType.OGGVORBIS)
    {
        originalClip = clip = await AudioLoader.LoadByWebRequest(filename, audioType);

        return originalClip;
    }

    public float[] GetData()
    {
        return GetData(clip);
    }

    public float[] GetData(float startTime, float endTime)
    {
        float[] all = GetData(clip);

        int startIndex = (int)(clip.frequency * startTime);
        int endIndex = (int)(clip.frequency * endTime);

        float[] data = new float[(endIndex - startIndex) * clip.channels];

        for (int i = startIndex * clip.channels; i < endIndex * clip.channels; i++)
        {
            data[i - startIndex * clip.channels] = all[i];
        }

        return data;
    }

    public float[] GetData_Mono()
    {

        if (clip.channels == 1) return GetData(clip);

        float[] all = GetData(clip);


        int length = (int)(all.Length / clip.channels);
        float[] data = new float[length];

        for (int i = 0; i < length; i++)
        {
            data[i] = (all[2 * i] + all[2 * i + 1]) / 2;
        }

        return data;
    }

    public double[] GetData_Double()
    {
        float[] data = GetData();
        double[] data_d = new double[data.Length];

        for (int i = 0; i < data.Length; i++)
        {
            data_d[i] = data[i];
        }

        return data_d;
    }

    public double[] GetData_Double(float startTime, float endTime)
    {
        float[] data = GetData();

        int startIndex = (int)(clip.frequency * startTime);
        int endIndex = (int)(clip.frequency * endTime);

        double[] data_d = new double[(endIndex - startIndex) * clip.channels];

        for (int i = startIndex * clip.channels; i < endIndex * clip.channels; i++)
        {
            data_d[i - startIndex * clip.channels] = data[i];
        }

        return data_d;
    }

    static private float[] GetData(AudioClip audioClip)
    {
        if (audioClip == null) return new float[0];

        int dataLength = (int)(audioClip.frequency * audioClip.length);
        float[] data = new float[dataLength * audioClip.channels];

        audioClip.GetData(data, 0);

        return data;
    }

    public AudioClip GetAudioClip(float startTime, float endTime)
    {
        float[] data = GetData(startTime, endTime);

        AudioClip playcClip = AudioClip.Create("playClip", data.Length, clip.channels, clip.frequency, false);
        playcClip.SetData(data, 0);

        return playcClip;

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

    static public AudioClip SetFadeOutCurve(AudioClip _clip)
    {

        AudioClip result;

        float[] data = GetData(_clip);

        if (data.Length == 0) return null;

        for (int i = 0; i < data.Length; i++)
        {
            float rate = (float)(data.Length - i) / (float)data.Length;

            data[i] = data[i] * (rate * (2 - rate)); //http://marupeke296.sakura.ne.jp/TIPS_No19_interpolation.html
        }

        result = AudioClip.Create("effectedclip", data.Length, _clip.channels, _clip.frequency, false);
        result.SetData(data, 0);

        return result;
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