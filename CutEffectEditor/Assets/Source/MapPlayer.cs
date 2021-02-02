using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;

public delegate void SendNoteHandler(NoteInfo noteInfo);
public delegate void PlayStopHandler();

public class MapPlayer : MonoBehaviour 
{
    string songPath;
    dynamic songEvents;

    float nextTime = 0;
    int nowIndex = 0;
    float BPM;
    float measureTime;
    float startOffset = 0.135f;

    public AudioManager AudioManager_Song { get; set; }

    bool isPlaying = false;
    bool isEnabled = true;

    [Serializable] public class SendNoteEvent : UnityEvent<NoteInfo> { }
    [SerializeField] public SendNoteEvent OnSendNote;

    [Serializable] public class PlayStopEvent : UnityEvent { }
    [SerializeField] public PlayStopEvent OnPlayStop;

    // Start is called before the first frame update
    public async UniTask<bool> TryInit(string songPath, string mapPath, GameObject gameObject)
    {
        if (!isEnabled) return false;

        if (!TryInitMap(mapPath)) return false;

        AudioManager_Song = new AudioManager(gameObject);
        
        bool song_loaded = await AudioManager_Song.TryLoad(songPath, AudioType.OGGVORBIS);
        if(!song_loaded) return false;

        AudioManager_Song.SetVolume(0.7f);

        return true;
    }

    public bool TryInitMap(string mapPath)
    {
        dynamic map = JsonParser.Parse(mapPath);
        if(map is null) return false;

        songEvents = map._notes;

        BPM = 180f;
        measureTime = 60f / BPM;
        nextTime = (float)(songEvents[0]._time) * measureTime;

        return true;
    }

    public void Play()
    {
        AudioManager_Song.Play();
        isPlaying = true;
    }

    const float scheduledTime = 0.38f;
    float beforeTime = 0f;
    // Update is called once per frame
    public void Update()
    {
        if (!isEnabled) return;

        if (!isPlaying) return;

        while (AudioManager_Song.GetNowTime() > nextTime)
        {
            NoteInfo noteInfo = new NoteInfo((float)songEvents[nowIndex]._time, (int)songEvents[nowIndex]._type, (int)songEvents[nowIndex]._cutDirection);
            
            //SendNote(noteInfo); //PlayForEdit(noteInfo); //cutEffect.Play(noteInfo);
            OnSendNote.Invoke(noteInfo);

            int length = songEvents.Count;
            if (length - 1 == nowIndex)
            {
                nextTime = AudioManager_Song.GetAudioClip().length;
                //PlayStop(); //StopForEdit();
                OnPlayStop.Invoke();
                return;
            }

            beforeTime = nextTime;
            nextTime = songEvents[++nowIndex]._time * measureTime;           
        }
    }
}
