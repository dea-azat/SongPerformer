using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class UISlicer : MonoBehaviour
{

    public UIWaveVisualizer waveViz;

    public GameObject slicedBlockPrefab;

    private List<SlicedBlock> slicedBlocks = new List<SlicedBlock>();
    private List<float> sliceTimes = new List<float>();

    [DllImport("OnsetDetector.dll")]
    static extern double OnsetDetect(float[] data, int length, int sampleRate, [In, Out] double[] onset, int onsetLength);

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Slice()
    {
        AudioClipUtils audioClip = waveViz.GetAudioClip();
        AudioClip clip = audioClip.GetAudioClip();
        // データ取得を書く

        sliceTimes = GetSliceTimes(audioClip);
        //List<float> sliceTimes = new List<float> { 0, 3, 5 };

        List<float> slicePoints = SlicerLibrary.ConvertTimeToPoint(sliceTimes, waveViz);

        for (int i = 0; i < slicePoints.Count-1; i++)
        {
            if (slicePoints[i + 1] > waveViz.GetWidth()) break;

            GameObject slicedBlockObject = Instantiate(slicedBlockPrefab, Vector2.zero, Quaternion.identity, waveViz.contentRect.transform);
            SlicedBlock slicedBlock = slicedBlockObject.GetComponent<SlicedBlock>();
            slicedBlock.slicer = this;
            slicedBlock.SetPosition(slicePoints[i], slicePoints[i + 1]);
            slicedBlock.OnPlayClick.AddListener(waveViz.Play);

            slicedBlocks.Add(slicedBlock);
        }
    }

    List<float> GetSliceTimes(AudioClipUtils audioClip)
    {
        List<float> sliceTimes = new List<float>();

        AudioClip clip = audioClip.GetAudioClip();

        float[] data = audioClip.GetData_Mono();
        double[] onset = new double[100];
        OnsetDetect(data, data.Length, clip.frequency, onset, onset.Length);

        sliceTimes.Add((float)onset[0]);

        for(int i=1; i<onset.Length; i++)
        {
            if (onset[i] < onset[i - 1]) break;

            sliceTimes.Add((float)onset[i]);
        }

        return sliceTimes;
    }

    public void OnWaveVisualizerWidthChanged(int width)
    {
        //すべて元どおりにしてしまう。 本来は残っているものだけを拡大縮小すべきだが...
        List<float> slicePoints = SlicerLibrary.ConvertTimeToPoint(sliceTimes, waveViz);

        for (int i = 0; i < slicePoints.Count - 1; i++)
        {
            if (slicePoints[i + 1] > waveViz.GetWidth()) break;

            slicedBlocks[i].SetPosition(slicePoints[i], slicePoints[i + 1]);
        }
    }

}

//あとで分離するという意味をこめて
static public class SlicerLibrary{

    static public List<float> ConvertTimeToPoint(List<float> times, UIWaveVisualizer waveViz)
    {
        AudioClipUtils audioClip = waveViz.GetAudioClip();
        AudioClip clip = audioClip.GetAudioClip();

        return ConvertTimeToPoint(times, (int)waveViz.GetWidth(), clip.frequency, clip.samples);
    }
    static public List<float> ConvertTimeToPoint(List<float> times, int width, int sampleRate, int dataLen)
    {
        List<float> slicePoints = new List<float>();

        int diff = width - dataLen % width;
        int cnt = (diff == width) ? (dataLen / width) : (dataLen + diff) / width;

        for (int i = 0; i < times.Count; i++)
        {
            slicePoints.Add(times[i] * sampleRate / cnt);
        }

        return slicePoints;
    }

    static public List<float> ConvertPointToTime(List<float> points, UIWaveVisualizer waveViz)
    {
        AudioClipUtils audioClip = waveViz.GetAudioClip();
        AudioClip clip = audioClip.GetAudioClip();

        return ConvertPointToTime(points, (int)waveViz.GetWidth(), clip.frequency, clip.samples);
    }

    static public List<float> ConvertPointToTime(List<float> points, int width, int sampleRate, int dataLen)
    {
        List<float> sliceTimes = new List<float>();

        for (int i = 0; i < points.Count; i++)
        {
            sliceTimes.Add(ConvertPointToTime(points[i], width, sampleRate, dataLen));
        }

        return sliceTimes;
    }

    static public float ConvertPointToTime(float point, UIWaveVisualizer waveViz)
    {
        AudioClipUtils audioClip = waveViz.GetAudioClip();
        AudioClip clip = audioClip.GetAudioClip();

        return ConvertPointToTime(point, (int)waveViz.GetWidth(), clip.frequency, clip.samples);
    }

    static public float ConvertPointToTime(float point, int width, int sampleRate, int dataLen)
    {
        int diff = width - dataLen % width;
        int cnt = (diff == width) ? (dataLen / width) : (dataLen + diff) / width;

        return point / sampleRate * cnt;

    }
}

//ConvertPointToTimeにwidthの情報を渡したいが、その情報はWaveVisualizerが持っている。どのように情報を取得するとモジュールを分離できるか？
// WaveVisualizerから通知してあげればよい
// WaveVisualizereにOnWidthChangeというUnityEventを追加して、UISlicerで受け取るのがよいのでは？
// AudioClipも通知してもらえばよい
// と思ったけどcontentRectもあるのでめんどくさい、やはり直でとってきてよい気がする