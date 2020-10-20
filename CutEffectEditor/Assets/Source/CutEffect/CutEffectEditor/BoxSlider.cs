using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

using System;
using HC.Debug;

using Cysharp.Threading.Tasks;
using System.Threading;

using CustomNotes.Data;
using System.CodeDom;

public class BoxSlider : MonoBehaviour
{

    public enum Slider_Type{
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

    private GameObject cubePrefab;
    private List<GameObject> cubes = new List<GameObject>();

    private BoxCollider sliderCollider;

    public event EventHandler ValueChanged;

    private Slider_Type type;

    private Color boxColor;

    public int value {get; set;}

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Init(Slider_Type _type)
    {
        type = _type;
        CustomNote cNote = BloqLoader.Load();
        ColliderVisualizer.VisualizerColorType vColor = ColliderVisualizer.VisualizerColorType.Green;

        if (Slider_Type.LEFT_BEGIN < type && type < Slider_Type.LEFT_END) 
        {
            cubePrefab = cNote.NoteLeft;
            cubePrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 * (int)(type - Slider_Type.LEFT_BEGIN - 1)));

            vColor = ColliderVisualizer.VisualizerColorType.Red;
            boxColor = Color.red;
        } 
        else if (Slider_Type.RIGHT_BEGIN < type && type < Slider_Type.RIGHT_END)
        {
            cubePrefab = cNote.NoteRight;
            cubePrefab.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45 * (int)(type - Slider_Type.RIGHT_BEGIN - 1)));

            vColor = ColliderVisualizer.VisualizerColorType.Blue;
            boxColor = Color.blue;
        } else
        {
            cubePrefab = cNote.NoteDotLeft;

            vColor = ColliderVisualizer.VisualizerColorType.Green;
            boxColor = Color.green;
        }               
                
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

        foreach (var cube in cubes)
        {
            var cubeCore = cube.transform.GetChild(0).GetChild(0);
            cubeCore.GetComponent<Renderer>().material.color = boxColor;
        }
        var message = "";
        var fontSize = 36;
        visualizer.Initialize(vColor, message, fontSize);

        RenewBoxSlider(BoxInfoForSlider.HalfCount());
    }

    // Update is called once per frame
    void Update()
    {
 
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

    public async UniTask FlashBox(int flashTime, int flashHz, CancellationToken cancellationToken){
        var cubeCore = cubes[value].transform.GetChild(0).GetChild(0);

        Color targetColor = (boxColor + Color.white) / 2;

        for(int i=0; i<=flashHz; i++){
            float rate = (float)i / (float)flashHz;

            cubeCore.GetComponent<Renderer>().material.color = boxColor * rate + (targetColor * (1f - rate));
            try
            {
                await UniTask.Delay((int)(flashTime / (flashHz+1)), cancellationToken: cancellationToken);
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
        foreach(var cube in cubes) {
            cube.SetActive(false);
        }

        cubes[index].SetActive(true);

        EventArgs e = new IntSliderEventArgs(index, type);
        OnValueChanged(e);

        value = index;
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

public class RevolvingBoxSlider
{
    private BoxSlider[] sliders;

    public RevolvingBoxSlider(GameObject boxSliderPrefab, BoxSlider.Slider_Type type, int sliderCnt=8, float radius=3f, EventHandler SliderValueChanged=null){
        float radian = 2f * Mathf.PI / (float)sliderCnt;
        float angle = 360f / (float)sliderCnt;
        
        sliders = new BoxSlider[sliderCnt];

        for(int i=0; i<sliderCnt; i++){
            Vector3 slider_pos = new Vector3(radius*Mathf.Sin(i*radian), 0, radius*(-Mathf.Cos(i*radian)));
            GameObject boxSlider = GameObject.Instantiate(boxSliderPrefab, slider_pos, Quaternion.identity);
            BoxSlider slider = boxSlider.GetComponent<BoxSlider>();
            slider.Init(type + i + 1);
            boxSlider.transform.rotation = Quaternion.Euler(0, i*45, 0);
            slider.ValueChanged += SliderValueChanged;
            sliders[i] = slider;
        }
    }

    public void Move(Vector3 vec){
        foreach(BoxSlider slider in sliders){
            slider.transform.position += vec;
        }
    }

    public void FlashBox(int index){
        int sliderIndex = (int)(ConvTypeCutDirectionToSliderIndex((NoteInfo.CutDirection)index) - (BoxSlider.Slider_Type.LEFT_BEGIN + 1));
        Debug.Log(sliderIndex);
        if (sliderIndex >= sliders.Length) return;

        sliders[sliderIndex].FlashBox();
    }

    private BoxSlider.Slider_Type ConvTypeForNoteInfoToSlider(NoteInfo noteInfo)
    {
        return ConvTypeCutDirectionToSliderIndex(noteInfo.cutDirection) + (int)noteInfo.type * ((int)BoxSlider.Slider_Type.RIGHT_BEGIN);
    }

    private BoxSlider.Slider_Type ConvTypeCutDirectionToSliderIndex(NoteInfo.CutDirection cutDirection)
    {
        Debug.Log(cutDirection);
        switch (cutDirection)
        {
            case NoteInfo.CutDirection.DOWN:
                return BoxSlider.Slider_Type.LEFT_DOWN;
            case NoteInfo.CutDirection.UP:
                return BoxSlider.Slider_Type.LEFT_UP;
            case NoteInfo.CutDirection.LEFT:
                return BoxSlider.Slider_Type.LEFT_LEFT;
            case NoteInfo.CutDirection.RIGHT:
                return BoxSlider.Slider_Type.LEFT_RIGHT;

            case NoteInfo.CutDirection.LEFT_UP:
                return BoxSlider.Slider_Type.LEFT_LEFT_UP;
            case NoteInfo.CutDirection.LEFT_DOWN:
                return BoxSlider.Slider_Type.LEFT_LEFT_DOWN;
            case NoteInfo.CutDirection.RIGHT_UP:
                return BoxSlider.Slider_Type.LEFT_RIGHT_UP;
            case NoteInfo.CutDirection.RIGHT_DOWN:
                return BoxSlider.Slider_Type.LEFT_RIGHT_DOWN;

            default:
                return BoxSlider.Slider_Type.Slider_Type_Max;
        }
    }
}