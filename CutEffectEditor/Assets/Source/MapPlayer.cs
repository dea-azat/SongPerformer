using Cysharp.Threading.Tasks;
using SongPerformer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEngine;

public delegate void SendNoteHandler(NoteInfo noteInfo);

public class MapPlayer
{
    string songPath;
    dynamic songEvents;

    float nextTime = 0;
    int nowIndex = 0;
    float BPM;
    float measureTime;
    float startOffset = 0.135f;

    private AudioManager audioManager_song;
    private CutEffectPlayer cutEffect;

    bool isPlaying = false;

    bool isEnabled = true;

    public event SendNoteHandler SendNote;

    // Start is called before the first frame update
    public async UniTask<bool> TryInit(string songPath, string mapPath, GameObject gameObject, CutEffectPlayer cutEffectManager)
    {
        if (!isEnabled) return false;

        if (!TryInitMap(mapPath)) return false;

        cutEffect = cutEffectManager;

        audioManager_song = new AudioManager(gameObject);
        
        bool song_loaded = await audioManager_song.TryLoad(songPath, AudioType.OGGVORBIS);
        if(!song_loaded) return false;

        audioManager_song.SetVolume(0.7f);
        AdjustTapKey(audioManager_song.GetAudioClip());

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

    public void InitCutEffectMapForEdit()
    {
        CutEffectMap cutEffectMap = new CutEffectMap();
        for (int i = 0; i < songEvents.Count; i++)
        {
            NoteInfo noteInfo = new NoteInfo((float)songEvents[i]._time, (int)songEvents[i]._type, (int)songEvents[i]._cutDirection);
            cutEffectMap.cutEffectEvents.Add(new CutEffectEvent(noteInfo));
        }

        cutEffect.SetCutEffectMap(cutEffectMap);
    }

    public void AdjustTapKey(AudioClip audioClip)
    {
        float pitch = KeyFinder.AdjustPitch(audioClip, 21); //KeyFinder側に定数マクロを持つべきだが...

        cutEffect.SetPitch(pitch);
    }

    public void Play()
    {
        audioManager_song.Play();
        isPlaying = true;
    }

    const float scheduledTime = 0.38f;
    float beforeTime = 0f;
    // Update is called once per frame
    public void Update()
    {
        if (!isEnabled) return;

        if (!isPlaying) return;

        while (audioManager_song.GetNowTime() > nextTime)
        {
            NoteInfo noteInfo = new NoteInfo((float)songEvents[nowIndex]._time, (int)songEvents[nowIndex]._type, (int)songEvents[nowIndex]._cutDirection);
            
            SendNote(noteInfo);
            
            //PlayForEdit(noteInfo);
            //cutEffect.Play(noteInfo);

            int length = songEvents.Count;
            if (length - 1 == nowIndex)
            {
                nextTime = audioManager_song.GetAudioClip().length;
                //StopForEdit();
                return;
            }

            beforeTime = nextTime;
            nextTime = songEvents[++nowIndex]._time * measureTime;
            // sound play            
        }
    }

    void PlayForEdit(NoteInfo noteInfo)
    {
        int tapIndex = SongEvents2TapIndex(noteInfo);

        float diff = beforeTime - nextTime;
        if (diff * diff > 0.001)
        {
            cutEffect.PlayScheduledForEdit(tapIndex, scheduledTime, noteInfo);
        }
    }

    void StopForEdit()
    {
        cutEffect.WriteOutCutEffectMap(/*mapPath*/""); /*! @todo Modify Edit Process  */
    }

    int SongEvents2TapIndex(NoteInfo noteInfo)
    {
        int index = 0;

        if (noteInfo.cutDirection == NoteInfo.CutDirection.DOWN || noteInfo.cutDirection == NoteInfo.CutDirection.UP)
        {
            if (noteInfo.type == NoteInfo.Type.LEFT) index = 2;
            if (noteInfo.type == NoteInfo.Type.RIGHT) index = 3;
        }
        else if (noteInfo.type == NoteInfo.Type.LEFT) { index = 0; }
        else if (noteInfo.type == NoteInfo.Type.RIGHT) { index = 1; }

        return index;
    }
}
