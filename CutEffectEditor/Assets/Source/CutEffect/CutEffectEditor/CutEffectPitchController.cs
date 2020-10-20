using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CutEffectPitchController : MonoBehaviour
{
    private CutEffectPlayer cutEffectManager;

    public GameObject boxSliderPrefab;
    private BoxSlider masterSlider;
    private RevolvingBoxSlider[] sliders;

    public void Init(int tap_num, CutEffectPlayer manager)
    {
        cutEffectManager = manager;

        sliders = new RevolvingBoxSlider[tap_num];

        InitSlider();
    }

    public void ShowNote(NoteInfo noteInfo)
    {
        int index = ConvNoteInfoToSliderIndex(noteInfo);

        sliders[index].FlashBox((int)noteInfo.cutDirection);
    }

    private int ConvNoteInfoToSliderIndex(NoteInfo noteInfo)
    {
        int index;

        switch(noteInfo.type){
            case NoteInfo.Type.LEFT:
                index = (int)BoxSlider.Slider_Type.LEFT_DOWN;
                break;
            case NoteInfo.Type.RIGHT:
                index = (int)BoxSlider.Slider_Type.RIGHT_DOWN;
                break;
            default:
                index = 0;
                break;
        }

        /* 将来的にはこれでよくなるはず
            index = noteInfo.type * DIRECTION_NUM + noteInfo.cutdirection;
        */

        return index;
    }

    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
    void InitSlider()
    {
        Vector3 left_sliders_pos = new Vector3(-4f, 0, 3f);

        RevolvingBoxSlider rSlider = new RevolvingBoxSlider(
            boxSliderPrefab, BoxSlider.Slider_Type.LEFT_BEGIN, CutEffectPlayer.DIRECTION_NUM, 3f, SliderValueChanged
            );
        rSlider.Move(left_sliders_pos);
        sliders[(int)BoxSlider.Slider_Type.LEFT_DOWN] = rSlider;


        Vector3 right_sliders_pos = new Vector3(4f, 0, 3f);

        rSlider = new RevolvingBoxSlider(
            boxSliderPrefab, BoxSlider.Slider_Type.RIGHT_BEGIN, CutEffectPlayer.DIRECTION_NUM, 3f, SliderValueChanged
            );
        rSlider.Move(right_sliders_pos);
        sliders[(int)BoxSlider.Slider_Type.RIGHT_DOWN] = rSlider;
        /*
        boxSlider = Instantiate(boxSliderPrefab, new Vector3(2f, 0, 0), Quaternion.identity);
        slider = boxSlider.GetComponent<BoxSlider>();
        slider.Init(cNote.NoteRight, BoxSlider.Slider_Type.RIGHT_DOWN);
        slider.ValueChanged += SliderValueChanged;
        sliders[(int)BoxSlider.Slider_Type.RIGHT_DOWN] = slider;
        */
        GameObject boxSlider = Instantiate(boxSliderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        BoxSlider slider = boxSlider.GetComponent<BoxSlider>();
        slider.Init(BoxSlider.Slider_Type.MASTER);
        slider.ValueChanged += SliderValueChanged;
        masterSlider = slider;
        
    }

    void SliderValueChanged(object obj, EventArgs e)
    {
        Debug.Log("SliderValueChanged");

        IntSliderEventArgs ise = (IntSliderEventArgs)e;

        if (ise.type == BoxSlider.Slider_Type.MASTER)
        {
            cutEffectManager.SetMasterDiffPitch(ise.index);
        }
        else
        {
            int tapIndex = SliderType2TapIndex(ise.type);

            cutEffectManager.SetCustomDiffPitch(tapIndex, ise.index);
        }
    }

    int SliderType2TapIndex(BoxSlider.Slider_Type type)
    {
        switch (type)
        {
            case BoxSlider.Slider_Type.LEFT_DOWN:
                return 2;
            case BoxSlider.Slider_Type.RIGHT_DOWN:
                return 3;
            default:
                return 0;
        }
    }
}
