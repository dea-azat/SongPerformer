using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using CustomNotes.Data;
using HC.Debug;

public enum SliderType_BeatSaber
{
    Slider_Type_Min = 0,

    LEFT_BEGIN = Slider_Type_Min,
    LEFT_DOWN,
    LEFT_LEFT_DOWN,
    LEFT_LEFT,
    LEFT_LEFT_UP,
    LEFT_UP,
    LEFT_RIGHT_UP,
    LEFT_RIGHT,
    LEFT_RIGHT_DOWN,
    LEFT_END,

    RIGHT_BEGIN,
    RIGHT_DOWN,
    RIGHT_LEFT_DOWN,
    RIGHT_LEFT,
    RIGHT_LEFT_UP,
    RIGHT_UP,
    RIGHT_RIGHT_UP,
    RIGHT_RIGHT,
    RIGHT_RIGHT_DOWN,
    RIGHT_END,

    MASTER,

    Slider_Type_Num,
    Slider_Type_Max = Slider_Type_Num - 1
}

public class BoxSlider_BeatSaber : BoxSlider<SliderType_BeatSaber>
{
    private SliderType_BeatSaber type;

    public void Init(SliderType_BeatSaber _type)
    {
        type = _type;
        CustomNote cNote = BloqLoader.Load();
        ColliderVisualizer.VisualizerColorType vColor = ColliderVisualizer.VisualizerColorType.Green;
        GameObject cubePrefab;
        Color boxColor;

        if (SliderType_BeatSaber.LEFT_BEGIN < type && type < SliderType_BeatSaber.LEFT_END)
        {
            cubePrefab = cNote.NoteLeft;
            cubePrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 * (int)(type - SliderType_BeatSaber.LEFT_BEGIN - 1)));

            vColor = ColliderVisualizer.VisualizerColorType.Red;
            boxColor = Color.red;
        }
        else if (SliderType_BeatSaber.RIGHT_BEGIN < type && type < SliderType_BeatSaber.RIGHT_END)
        {
            cubePrefab = cNote.NoteRight;
            cubePrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 * (int)(type - SliderType_BeatSaber.RIGHT_BEGIN - 1)));

            vColor = ColliderVisualizer.VisualizerColorType.Blue;
            boxColor = Color.blue;
        }
        else
        {
            cubePrefab = cNote.NoteDotLeft;

            vColor = ColliderVisualizer.VisualizerColorType.Green;
            boxColor = Color.green;
        }

        Init(cubePrefab, boxColor);
        InitVisualizer(vColor);
    }
}

public class RevolvingBoxSlider
{
    private BoxSlider_BeatSaber[] sliders;

    public RevolvingBoxSlider(GameObject boxSliderPrefab, SliderType_BeatSaber type, int sliderCnt = 8, float radius = 3f, EventHandler SliderValueChanged = null)
    {
        float radian = 2f * Mathf.PI / (float)sliderCnt;
        float angle = 360f / (float)sliderCnt;

        sliders = new BoxSlider_BeatSaber[sliderCnt];

        for (int i = 0; i < sliderCnt; i++)
        {
            Vector3 slider_pos = new Vector3(radius * Mathf.Sin(i * radian), 0, radius * (-Mathf.Cos(i * radian)));
            GameObject boxSlider = GameObject.Instantiate(boxSliderPrefab, slider_pos, Quaternion.identity);
            BoxSlider_BeatSaber slider = boxSlider.GetComponent<BoxSlider_BeatSaber>();
            slider.Init(type + i + 1);
            boxSlider.transform.rotation = Quaternion.Euler(0, i * 45, 0);
            slider.ValueChanged += SliderValueChanged;
            sliders[i] = slider;
        }
    }

    public void Move(Vector3 vec)
    {
        foreach (BoxSlider_BeatSaber slider in sliders)
        {
            slider.transform.position += vec;
        }
    }

    public void FlashBox(int index)
    {
        int sliderIndex = (int)(ConvTypeCutDirectionToSliderIndex((NoteInfo.CutDirection)index) - (SliderType_BeatSaber.LEFT_BEGIN + 1));
        Debug.Log(sliderIndex);
        if (sliderIndex >= sliders.Length) return;

        sliders[sliderIndex].FlashBox();
    }

    private SliderType_BeatSaber ConvTypeForNoteInfoToSlider(NoteInfo noteInfo)
    {
        return ConvTypeCutDirectionToSliderIndex(noteInfo.cutDirection) + (int)noteInfo.type * ((int)SliderType_BeatSaber.RIGHT_BEGIN);
    }

    private SliderType_BeatSaber ConvTypeCutDirectionToSliderIndex(NoteInfo.CutDirection cutDirection)
    {
        Debug.Log(cutDirection);
        switch (cutDirection)
        {
            case NoteInfo.CutDirection.DOWN:
                return SliderType_BeatSaber.LEFT_DOWN;
            case NoteInfo.CutDirection.UP:
                return SliderType_BeatSaber.LEFT_UP;
            case NoteInfo.CutDirection.LEFT:
                return SliderType_BeatSaber.LEFT_LEFT;
            case NoteInfo.CutDirection.RIGHT:
                return SliderType_BeatSaber.LEFT_RIGHT;

            case NoteInfo.CutDirection.LEFT_UP:
                return SliderType_BeatSaber.LEFT_LEFT_UP;
            case NoteInfo.CutDirection.LEFT_DOWN:
                return SliderType_BeatSaber.LEFT_LEFT_DOWN;
            case NoteInfo.CutDirection.RIGHT_UP:
                return SliderType_BeatSaber.LEFT_RIGHT_UP;
            case NoteInfo.CutDirection.RIGHT_DOWN:
                return SliderType_BeatSaber.LEFT_RIGHT_DOWN;

            default:
                return SliderType_BeatSaber.Slider_Type_Max;
        }
    }
}