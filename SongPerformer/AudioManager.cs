using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

using UnityEngine.Networking;
using UnityEngine.Events;

using Log = DefaultLogger;
using System.Runtime.CompilerServices;

using Cysharp.Threading.Tasks;
using SongPerformer;
using System.Runtime.InteropServices;

public class AudioManager
{
    List<AudioSource> audioSources = new List<AudioSource>();
    AudioClipController clipController;
    int playIndex = 0;

    float masterPitch = 1f;
    int masterDiff = 0;
    int customDiff = 0;

    enum AudioSourceState
    {
        Unload,
        Load,
        Play,
        Stop
    }

    AudioSourceState state = AudioSourceState.Unload;

    public AudioManager(GameObject gameObject, int audioNum = 1)
    {
        for (int i = 0; i < audioNum; i++)
        {
            AudioSource audioSource = gameObject.AddComponent<AudioSource>();
            audioSources.Add(audioSource);
        }

        clipController = new AudioClipController();
    }

    void LoadExecute(AudioClip clip)
    {
        audioSources[playIndex].clip = clip;
        audioSources[playIndex].Play();
    }

    void LoadExecuteWithCallback(AudioClip clip)
    {
        audioSources[playIndex].clip = clip;
    }

    public async UniTask<bool> TryLoad(string path, AudioType audioType)
    {
        AudioClip clip = await clipController.LoadAudioClipWithWebRequest(path, audioType);

        for(int i=0; i<audioSources.Count; i++)
        {
            audioSources[playIndex].clip = clip;
        }

        return true;
    }

    public async void LoadWithCallback(string path, UnityAction<AudioClip> action)
    {
        AudioClip clip = await clipController.LoadAudioClipWithWebRequest(path);

        LoadExecute(clip);
        action(clip);
    }

    public void Load(AudioClip clip)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.clip = clip;
        }

        //PreLoad();
    }

    public void Play()
    {
        audioSources[0].Play();
    }

    public AudioClip GetAudioClip()
    {
        return clipController.GetAudioClip();
    }

    public float GetNowTime()
    {
        return audioSources[playIndex].time;
    }

    public void SetVolume(float volume)
    {
        foreach (var audioSource in audioSources)
        {
            audioSource.volume = volume;
        }
    }

    public IEnumerator Reset()
    {
        Debug.Log("Reset");

        yield return new WaitForSeconds(1);

        state = AudioSourceState.Stop;

        PreLoad();
        
    }

    public void PreLoad()
    {
        Debug.Log("Preload");

        /*
        audioSource.Stop();
        audioSource.volume = 0;
        audioSource.Play();

        if (state == AudioSourceState.Stop)
        {
            audioSource.Pause();
        }

        audioSource.volume = 1;

        state = AudioSourceState.Load;
        */
    }

    public void PlayScheduled(float scheduledTime)
    {
        //Debug.Log("PlaySchedueld " + playIndex);
        audioSources[playIndex].PlayScheduled(scheduledTime);
        playIndex = (playIndex + 1) % audioSources.Count;
    }

    public void SetMasterPitch(float _pitch)
    {
        masterPitch = _pitch;

        float customPitch = Index2Pitch(customDiff);

        foreach (var audioSource in audioSources)
        {
            audioSource.pitch = customPitch;
        }
    }

    public void SetCustomPitch(int diff)
    {
        customDiff = diff;

        float customPitch = Index2Pitch(customDiff);

        foreach (var audioSource in audioSources)
        {
            audioSource.pitch = customPitch;
        }
    }

    private float Index2Pitch(int index)
    {
        float customPitch = masterPitch;

        float semitone;
        if (index > 0)
        {
            semitone = KeyFinder.SEMITONE;
        }
        else
        {
            semitone = 1f / KeyFinder.SEMITONE;
        }

        for (int i = 0; i < Math.Abs(index); i++)
        {
            customPitch *= semitone;
        }

        return customPitch;
    }

    public void SetFadeOutCurve()
    {
        clipController.SetFadeOutCurve();

        for (int i = 0; i < audioSources.Count; i++)
        {
            audioSources[playIndex].clip = clipController.GetAudioClip();
        }
    }
}

public class AudioClipController
{
    private AudioClip clip = null;
    private AudioClip originalClip = null;

    public AudioClip GetAudioClip()
    {
        return clip;
    }

    public async UniTask<AudioClip> LoadAudioClipWithWebRequest(string filename, AudioType audioType=AudioType.OGGVORBIS)
    {
        Log.Write("LoadAudioClipWithWebRequest Start");

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip("file://" + filename, audioType))
        {
            await www.SendWebRequest(); // must wait
            originalClip = clip = DownloadHandlerAudioClip.GetContent(www);
        }

        //Log.Write("loadState = " + clip.loadState);
        //Log.Write("loadType = " + clip.loadType);

        //Log.Write("Clip Load Success");

        return originalClip;
    }

    public void SetAudioClip(AudioClip _clip)
    {
        originalClip = clip = _clip;
    }

    public void SetFadeOutCurve()
    {
        int dataLength = (int)(originalClip.frequency * originalClip.length);

        float[] data = new float[dataLength * originalClip.channels];

        originalClip.GetData(data, 0);

        for(int i=0; i<data.Length; i++)
        {
            float rate = (float)(data.Length - i) / (float)data.Length;

            data[i] = data[i] * ( rate * (2 - rate) ); //http://marupeke296.sakura.ne.jp/TIPS_No19_interpolation.html
        }

        clip = AudioClip.Create("effectedclip", data.Length, clip.channels, clip.frequency, false);
        clip.SetData(data, 0);
    }
}

public class SoundPlayer
{
    protected AudioSource audioSource;

    public virtual void Init(GameObject gameObject)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public virtual void SetAudioClip(AudioClip clip)
    {
        audioSource.clip = clip;
    }

    public virtual void SetAudioClip(AudioClipController controller)
    {
        audioSource.clip = controller.GetAudioClip();
    }

    public virtual void PlaySound()
    {
        if (audioSource.clip == null) return;

        audioSource.Play();
    }

    public virtual float GetNowTime()
    {
        return audioSource.time;
    }

    public virtual void SetEffect<Type>(Type T) //To make data container
    {

    }

    public virtual void SetPitch(float _pitch)
    {
        audioSource.pitch = _pitch;
    }

    public virtual void SetVolume(float _volume)
    {
        audioSource.volume = _volume;
    }

    public virtual void Load()
    {
        audioSource.clip.LoadAudioData();
    }
}

static public class DefaultLogger
{
    static public void Write(string text)
    {
        Debug.Log(text);
    }
}
