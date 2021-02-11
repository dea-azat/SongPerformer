using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using System;
using HC.Debug;

using Cysharp.Threading.Tasks;
using System.Threading;


public class BoxSlider<SliderType> : MonoBehaviour
{
    private List<GameObject> _boxes = new List<GameObject>();
    private BoxCollider _sliderCollider;
    private Color _boxColor;

    public event EventHandler ValueChanged;
    public int _value { get; set; }

    SliderType _type;
    
    public void Init(GameObject boxPrefab, Color boxColor)
    {
        BoxInfoForSlider.size = 1f;

        for (int i = 0; i < BoxInfoForSlider.count; i++)
        {
            var cube = Instantiate(boxPrefab);
            cube.transform.SetParent(this.transform);
            cube.transform.localPosition = new Vector3(0f, BoxInfoForSlider.size * i - BoxInfoForSlider.size * (BoxInfoForSlider.count - 1) / 2);
            _boxes.Add(cube);
        }

        float margin = 1.2f;
        _sliderCollider = gameObject.AddComponent<BoxCollider>();
        _sliderCollider.size = new Vector3(BoxInfoForSlider.size * margin, BoxInfoForSlider.TotalSize(), BoxInfoForSlider.size * margin);

        var ev = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { MoveBox((PointerEventData)eventData); });
        ev.triggers.Add(entry);


        foreach (var box in _boxes)
        {
            var boxCore = box.transform.GetChild(0).GetChild(0);
            boxCore.GetComponent<Renderer>().material.color = boxColor;
        }

        RenewBoxSlider(BoxInfoForSlider.HalfCount());
    }

    public void InitVisualizer(ColliderVisualizer.VisualizerColorType vColor)
    {
        ColliderVisualizer visualizer = GetComponent<ColliderVisualizer>();
        var message = "";
        var fontSize = 36;
        visualizer.Initialize(vColor, message, fontSize);
    }

    private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
    public void FlashBox()
    {
        const int flashTime = 256;
        const int flashHz = 15;

        cancellationTokenSource.Cancel();
        cancellationTokenSource = new CancellationTokenSource();

        FlashBox(flashTime, flashHz, cancellationTokenSource.Token).Forget(e => { });
    }

    public async UniTask FlashBox(int flashTime, int flashHz, CancellationToken cancellationToken)
    {
        var cubeCore = _boxes[_value].transform.GetChild(0).GetChild(0);

        Color targetColor = (_boxColor + Color.white) / 2;

        for (int i = 0; i <= flashHz; i++)
        {
            float rate = (float)i / (float)flashHz;

            cubeCore.GetComponent<Renderer>().material.color = _boxColor * rate + (targetColor * (1f - rate));
            try
            {
                await UniTask.Delay((int)(flashTime / (flashHz + 1)), cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }
    }

    public void MoveBox(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 15.0f))
        {
            Debug.Log(hit.point);
            int index = CalcBoxIndex(hit.point.y);
            Debug.Log(index);

            RenewBoxSlider(index);
        }
    }

    private void RenewBoxSlider(int index)
    {
        foreach (var box in _boxes)
        {
            box.SetActive(false);
        }

        _boxes[index].SetActive(true);

        EventArgs e = new IntSliderEventArgs<SliderType>(index, _type);
        OnValueChanged(e);

        _value = index;
    }

    private int CalcBoxIndex(float y)
    {
        return (int)((y + BoxInfoForSlider.TotalSize() / 2) / BoxInfoForSlider.size);
    }

    protected virtual void OnValueChanged(EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }
}

public static class BoxInfoForSlider
{
    public static float size;
    public static int count = 13;
    public static float TotalSize()
    {
        return size * count;
    }

    public static int HalfCount()
    {
        // 偶数の場合は例えば4ならば3/2=1.5で2になるはず
        return (count - 1) / 2;
    }
}

class IntSliderEventArgs<SliderType> : EventArgs
{
    public IntSliderEventArgs(int _index, SliderType _type)
        : base()
    {
        index = _index - BoxInfoForSlider.HalfCount();
        type = _type;
    }

    public int index { get; }
    public SliderType type { get; }
}