using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using UnityEngine;

public class CutEffectMap
{
    public List<string> cutEffectPaths = new List<string>();
    public float masterPitch;
    public List<CutEffectEvent> cutEffectEvents = new List<CutEffectEvent>();

    public int SearchCutEffectEventIndex(NoteInfo noteInfo)
    {
        //https://qiita.com/RyotaMurohoshi/items/118d4aa4d5be8c71c9ce
        var elements = cutEffectEvents.Select((cutEffectEvent, index) => new { cutEffectEvent.time, index });

        float e = 0.01f;
        var result = elements.Where(element => noteInfo.time - e < element.time && element.time < noteInfo.time + e).ToList();

        foreach(var r in result)
        {
            bool typeIsSame = (int)noteInfo.type == cutEffectEvents[r.index].type;
            bool cutDirectionIsSame = (int)noteInfo.cutDirection == cutEffectEvents[r.index].cutDirection;

            if (typeIsSame && cutDirectionIsSame) return r.index;
        }

        return -1;
    }
}

public class CutEffectEvent
{

    public CutEffectEvent()
    {

    }

    public CutEffectEvent(NoteInfo noteInfo)
    {
        pathIndex = -1;

        time = noteInfo.time;
        type = (int)noteInfo.type;
        cutDirection = (int)noteInfo.cutDirection;
    }

    public CutEffectEvent(int _pathIndex, int _masterPitch, int _customPitch, NoteInfo noteInfo)
    {
        SetEvent(_pathIndex, _masterPitch, _customPitch, noteInfo);
    }

    public void SetEvent(int _pathIndex, int _masterPitch, int _customPitch, NoteInfo noteInfo=null)
    {
        pathIndex = _pathIndex;
        masterPitchDiff = _masterPitch;
        customPitchDiff = _customPitch;

        if (noteInfo == null) return;
        time = noteInfo.time;
        type = (int)noteInfo.type;
        cutDirection = (int)noteInfo.cutDirection;
    }
    
    public int pathIndex { get; set; }
    public int masterPitchDiff { get; set; }
    public int customPitchDiff { get; set; }
    public float time { get; set; }
    public int type { get; set; } // want to use enum but I'm afraid of bugs of Desilializer for json
    public int cutDirection { get; set; }
}

public class NoteInfo
{
    public enum CutDirection
    {
        UP,
        DOWN,
        LEFT,
        RIGHT,
        LEFT_UP,
        RIGHT_UP,
        LEFT_DOWN,
        RIGHT_DOWN
    }

    public enum Type
    {
        LEFT,
        RIGHT,
        DOT
    }

    public NoteInfo(float _time, int _type, int _cutDirection)
    {
        time = _time;
        type = (Type)_type;
        cutDirection = (CutDirection)_cutDirection;
    }

    public void ConvertTime2Measure(float BPM)
    {
        float measureTime = 60f / BPM;
        time /= measureTime;
    }

    public float time { get; set; }
    public Type type { get; set; }
    public CutDirection cutDirection { get; set; }
}