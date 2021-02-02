using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditor : MonoBehaviour
{
    private CutEffectPlayer cutEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //Edit関連をいったん切り出す

    //public void InitCutEffectMapForEdit()
    //{
    //    CutEffectMap cutEffectMap = new CutEffectMap();
    //    for (int i = 0; i < songEvents.Count; i++)
    //    {
    //        NoteInfo noteInfo = new NoteInfo((float)songEvents[i]._time, (int)songEvents[i]._type, (int)songEvents[i]._cutDirection);
    //        cutEffectMap.cutEffectEvents.Add(new CutEffectEvent(noteInfo));
    //    }

    //    cutEffect.SetCutEffectMap(cutEffectMap);
    //}

    //void PlayForEdit(NoteInfo noteInfo)
    //{
    //    int tapIndex = SongEvents2TapIndex(noteInfo);

    //    float diff = beforeTime - nextTime;
    //    if (diff * diff > 0.001)
    //    {
    //        cutEffect.PlayScheduledForEdit(tapIndex, scheduledTime, noteInfo);
    //    }
    //}

    //void StopForEdit()
    //{
    //    cutEffect.WriteOutCutEffectMap(/*mapPath*/""); /*! @todo Modify Edit Process  */
    //}

    //int SongEvents2TapIndex(NoteInfo noteInfo)
    //{
    //    int index = 0;

    //    if (noteInfo.cutDirection == NoteInfo.CutDirection.DOWN || noteInfo.cutDirection == NoteInfo.CutDirection.UP)
    //    {
    //        if (noteInfo.type == NoteInfo.Type.LEFT) index = 2;
    //        if (noteInfo.type == NoteInfo.Type.RIGHT) index = 3;
    //    }
    //    else if (noteInfo.type == NoteInfo.Type.LEFT) { index = 0; }
    //    else if (noteInfo.type == NoteInfo.Type.RIGHT) { index = 1; }

    //    return index;
    //}
}
