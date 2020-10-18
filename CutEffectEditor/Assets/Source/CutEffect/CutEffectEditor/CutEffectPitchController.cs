using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using CustomNotes.Data;

public class CutEffectPitchController : MonoBehaviour
{
    private CutEffectPlayer cutEffectManager;

    public GameObject boxSliderPrefab;
    private BoxSlider masterSlider;
    private BoxSlider[] sliders;

    private BloqLoader bloqLoader = new BloqLoader();

    public void Init(int tap_num, CutEffectPlayer manager)
    {
        cutEffectManager = manager;

        sliders = new BoxSlider[tap_num];

        InitSlider();
    }

    public void ShowNote(NoteInfo noteInfo)
    {
        int index = ConvNoteInfoToSliderIndex(noteInfo);

        sliders[index].FlashBox();
    }

    private int ConvNoteInfoToSliderIndex(NoteInfo noteInfo)
    {
        int index;

        switch(noteInfo.type){
            case 0:
                index = (int)BoxSlider.Slider_Type.LEFT_DOWN;
                break;
            case 1:
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
        CustomNote cNote = bloqLoader.Load();

        GameObject boxSlider = Instantiate(boxSliderPrefab, new Vector3(-2f, 0, 0), Quaternion.identity);
        BoxSlider slider = boxSlider.GetComponent<BoxSlider>();
        slider.Init(cNote.NoteLeft, BoxSlider.Slider_Type.LEFT_DOWN);
        slider.ValueChanged += SliderValueChanged;
        sliders[(int)BoxSlider.Slider_Type.LEFT_DOWN] = slider;

        boxSlider = Instantiate(boxSliderPrefab, new Vector3(2f, 0, 0), Quaternion.identity);
        slider = boxSlider.GetComponent<BoxSlider>();
        slider.Init(cNote.NoteRight, BoxSlider.Slider_Type.RIGHT_DOWN);
        slider.ValueChanged += SliderValueChanged;
        sliders[(int)BoxSlider.Slider_Type.RIGHT_DOWN] = slider;

        boxSlider = Instantiate(boxSliderPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        slider = boxSlider.GetComponent<BoxSlider>();
        slider.Init(cNote.NoteDotLeft, BoxSlider.Slider_Type.MASTER);
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
