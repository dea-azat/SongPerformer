using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using System;
using HC.Debug;

public class BoxSlider : MonoBehaviour
{

    public enum Slider_Type{
        LEFT_DOWN,
        RIGHT_DOWN,
        MASTER
    }

    private GameObject cubePrefab;
    private List<GameObject> cubes = new List<GameObject>();

    private BoxCollider sliderCollider;

    public event EventHandler ValueChanged;

    private Slider_Type type;

    // Start is called before the first frame update
    void Start()
    {
        //Init();
    }

    public void Init(GameObject _cubePrefab, Slider_Type _type)
    {
        cubePrefab = _cubePrefab;

        BoxInfoForSlider.size = 1f;

        for (int i = 0; i < BoxInfoForSlider.count; i++)
        {
            var cube = Instantiate(cubePrefab);
            cube.transform.SetParent(this.transform);
            cube.transform.localPosition = new Vector3(0f, BoxInfoForSlider.size * i - BoxInfoForSlider.size * (BoxInfoForSlider.count - 1) / 2);
            cubes.Add(cube);
        }

        float margin = 1.2f;
        sliderCollider = gameObject.AddComponent<BoxCollider>();
        sliderCollider.size = new Vector3(BoxInfoForSlider.size * margin, BoxInfoForSlider.TotalSize(), BoxInfoForSlider.size * margin);

        var ev = gameObject.AddComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerDown;
        entry.callback.AddListener((eventData) => { MoveBox((PointerEventData)eventData); });
        ev.triggers.Add(entry);



        ColliderVisualizer visualizer = GetComponent<ColliderVisualizer>();
        ColliderVisualizer.VisualizerColorType vColor = ColliderVisualizer.VisualizerColorType.Green;
        Color color = Color.green;

        type = _type;

        switch (type) {
            case Slider_Type.LEFT_DOWN:
                vColor = ColliderVisualizer.VisualizerColorType.Red;
                color = Color.red;
                break;
            case Slider_Type.RIGHT_DOWN:
                vColor = ColliderVisualizer.VisualizerColorType.Blue;
                color = Color.blue;
                break;
            case Slider_Type.MASTER:
                vColor = ColliderVisualizer.VisualizerColorType.Green;
                color = Color.green;
                break;
        }

        foreach (var cube in cubes)
        {
            var cubeCore = cube.transform.GetChild(0).GetChild(0);
            cubeCore.GetComponent<Renderer>().material.color = color;
        }
        var message = "";
        var fontSize = 36;
        visualizer.Initialize(vColor, message, fontSize);
    }

    // Update is called once per frame
    void Update()
    {
 
    }

    public void MoveBox(PointerEventData eventData)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 10.0f))
        {
            Debug.Log(hit.point);
            int index = CalcBoxIndex(hit.point.y);
            Debug.Log(index);

            foreach(var cube in cubes) {
                cube.SetActive(false);
            }

            cubes[index].SetActive(true);

            EventArgs e = new IntSliderEventArgs(index, type);
            OnValueChanged(e);
        }
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

class IntSliderEventArgs : EventArgs
{
    public IntSliderEventArgs(int _index, BoxSlider.Slider_Type _type)
        : base()
    {
        index = _index - BoxInfoForSlider.HalfCount();
        type = _type;
    }

    public int index { get; }
    public BoxSlider.Slider_Type type { get; }
}
