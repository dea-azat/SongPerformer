using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Cysharp.Threading.Tasks;

public class CutEffectPlayer
{
    public static int DIRECTION_NUM = 9;
    public static int HAND_NUM = 2;
    public static int TAP_NUM = DIRECTION_NUM * HAND_NUM;

    private AudioManager[] audioManager_tap = new AudioManager[TAP_NUM];

    private GameObject[] cutEffectObjects = new GameObject[TAP_NUM];

    private CutEffectMap cutEffectMap;

    // Start is called before the first frame update
    public void Init(GameObject master)
    {
        GameObject cutEffectPrefab = new GameObject();
        InitCutEffects(master, cutEffectPrefab);
    }

    public async UniTask<bool> TryLoadCutEffects()
    {
        const string LEFT_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Left_Effect.wav";
        const string RIGHT_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Right_Effect.wav";
        const string DOWN_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";

        await audioManager_tap[0].TryLoad(LEFT_EFFECT_PATH, AudioType.WAV);
        audioManager_tap[0].SetVolume(0.25f);
        audioManager_tap[0].SetFadeOutCurve();
        cutEffectMap.cutEffectPaths.Add(LEFT_EFFECT_PATH);

        await audioManager_tap[1].TryLoad(RIGHT_EFFECT_PATH, AudioType.WAV);
        audioManager_tap[1].SetVolume(0.25f);
        audioManager_tap[1].SetFadeOutCurve();
        cutEffectMap.cutEffectPaths.Add(RIGHT_EFFECT_PATH);

        await audioManager_tap[2].TryLoad(DOWN_EFFECT_PATH, AudioType.WAV);
        audioManager_tap[2].SetVolume(0.25f);
        audioManager_tap[2].SetFadeOutCurve();
        cutEffectMap.cutEffectPaths.Add(DOWN_EFFECT_PATH);

        await audioManager_tap[3].TryLoad(DOWN_EFFECT_PATH, AudioType.WAV);
        audioManager_tap[3].SetVolume(0.25f);
        audioManager_tap[3].SetFadeOutCurve();
        cutEffectMap.cutEffectPaths.Add(DOWN_EFFECT_PATH);

        /*
        if (false)
        {
            AudioClip left = cutEffectLoader.Load("left");
            audioManager_tap[0].Load(left);
            AudioClip center = cutEffectLoader.Load("center");
            audioManager_tap[1].Load(center);
            AudioClip right = cutEffectLoader.Load("right");
            audioManager_tap[2].Load(right);
        }
        */

        return true;
    }

    public async UniTask<bool> TryLoadCutEffectMap(string mapPath)
    {
        cutEffectMap = CutEffectMapLoader.Load(mapPath);

        if (cutEffectMap == null) return false;

        for (int i = 0; i<cutEffectMap.cutEffectPaths.Count; i++)
        {
            await audioManager_tap[i].TryLoad(cutEffectMap.cutEffectPaths[i], AudioType.WAV);
            audioManager_tap[i].SetVolume(0.25f);
            audioManager_tap[i].SetFadeOutCurve();
        }

        SetPitch(cutEffectMap.masterPitch);

        return true;
    }

    void InitCutEffects(GameObject master, GameObject cutEffectObjPrefab)
    {
        for (int i = 0; i < TAP_NUM; i++)
        {
            GameObject cutEffectObj = GameObject.Instantiate(cutEffectObjPrefab);
            cutEffectObj.transform.SetParent(master.transform);
            AudioManager audioManager = new AudioManager(cutEffectObj);
            cutEffectObjects[i] = cutEffectObj;
            audioManager_tap[i] = audioManager;
        }
    }

    int masterDiff = 0;
    int[] customDiff = new int[TAP_NUM];

    public void SetMasterDiffPitch(int diff)
    {
        masterDiff = diff;

        for (int i = 0; i < TAP_NUM; i++)
        {
            audioManager_tap[i].SetCustomPitch(masterDiff + customDiff[i]);
        }
    }

    public void SetCustomDiffPitch(int index, int diff)
    {
        customDiff[index] = diff;
        audioManager_tap[index].SetCustomPitch(masterDiff + customDiff[index]);
    }

    public void PlayScheduledForEdit(int tapIndex, float scheduledTime, NoteInfo noteInfo=null)
    {
        audioManager_tap[tapIndex % audioManager_tap.Length].PlayScheduled(scheduledTime);


        //分けるべきだが...
        if (noteInfo == null) return;

        int index = cutEffectMap.SearchCutEffectEventIndex(noteInfo);
        cutEffectMap.cutEffectEvents[index].SetEvent(tapIndex, masterDiff, customDiff[tapIndex]);
    }

    public void Play(NoteInfo noteInfo)
    {
        PlayScheduled(noteInfo, 0);
    }

    public void PlayScheduled(NoteInfo noteInfo, float scheduledTime)
    {
        int index = cutEffectMap.SearchCutEffectEventIndex(noteInfo);
        if (index == -1) return;

        CutEffectEvent cutEffectEvent = cutEffectMap.cutEffectEvents[index];
        if (cutEffectEvent.pathIndex == -1) return;

        SetMasterDiffPitch(cutEffectEvent.masterPitchDiff);
        SetCustomDiffPitch(cutEffectEvent.pathIndex, cutEffectEvent.customPitchDiff);

        if (scheduledTime.Equals(0))
        {
            audioManager_tap[cutEffectEvent.pathIndex % audioManager_tap.Length].Play();
        }
        else
        {
            audioManager_tap[cutEffectEvent.pathIndex % audioManager_tap.Length].PlayScheduled(scheduledTime);
        }
        
    }

    public void SetPitch(float _pitch)
    {
        for (int i = 0; i < 4/* 制限を外すのを忘れるな */; i++)
        {
            audioManager_tap[i].SetMasterPitch(_pitch);
        }

        cutEffectMap.masterPitch = _pitch;
    }

    //Edit用だが紛らわしい...
    public void SetCutEffectMap(CutEffectMap map)
    {
        cutEffectMap = map;
    }

    //なぜplayerがwriteoutを持っているのか
    public void WriteOutCutEffectMap(string mapPath)
    {
        string path = mapPath.Replace(".dat", "Effect.dat");
        JsonWriter.WriteOut(cutEffectMap, path);
    }
}
