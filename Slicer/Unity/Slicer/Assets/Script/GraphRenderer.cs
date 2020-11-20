using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.UI.Extensions;

using System.Linq;

using Cysharp.Threading.Tasks;


interface IGraphRenderer{
    void Render(string path);
    void ReRender();

}

public class GraphRenderer : MonoBehaviour, IGraphRenderer
{
    public float width = 2000;
    float height = 300;
    float marginY = 10;

    private float[] data;

    // Start is called before the first frame update
    void Start()
    {
        //TestGraphRenderer();
    }


    public void Render(string path)
    {
        RenderFromPath(path);
    }

    public void ReRender()
    {
        if (data.Length == 0) return;

        Render(data);
    }

    private async void RenderFromPath(string path)
    {

        AudioClipUtils audioClip = new AudioClipUtils();
        await audioClip.LoadByWebRequest(path, AudioType.WAV);
        data = audioClip.GetData();
        Render(data);
    }

    void TestAvaragingData()
    {
        float[] data = AveragingData(new float[4] { 1, 2, 3, 4}, 2);
        for (int i = 0; i < data.Length; i++)
        {
            Debug.Log(data[i]);
        }
    }

    void TestGraphRenderer()
    {
        const string DOWN_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";
        Render(DOWN_EFFECT_PATH);
    }

    async void TestAvaraginDataWithAudioData()
    {
        const string DOWN_EFFECT_PATH = "D:/BeatSaberMod/EffectSamples/Down_Effect.wav";

        AudioClipUtils audioClip = new AudioClipUtils();
        await audioClip.LoadByWebRequest(DOWN_EFFECT_PATH, AudioType.WAV);
        float[] data = audioClip.GetData();
        data = AveragingData(data, 2000);

        for (int i = 0; i < data.Length; i++)
        {
            Debug.Log(data[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private float[] AveragingData(float[] data, int totalNum)
    {
        float[] ave = new float[totalNum];
        int diff = totalNum - data.Length % totalNum;
        int cnt = (diff == totalNum) ? (data.Length / totalNum) : ((data.Length + diff) / totalNum);

        for (int i = 0; i < totalNum; i++)
        {
            ave[i] = 0f;

            int total = 0;
            for (int j = i * cnt; j < (i+1)*cnt; j++)
            {
                if (data.Length <= j) break;
                ave[i] += data[j];
                total++;
            }

            ave[i] = ave[i].Equals(0f) ? 0f : ave[i]/(float)total;

            if (float.IsNaN(ave[i])) Debug.Log("This is Nan index " + i);
        }

        return ave;
    }

    private Vector2[] Data2Positions(float[] data)
    {

        int length = data.Length;

        Vector2[] positions = new Vector2[length];
        
        float intervalX = width / (length - 1);

        float maxY = data.Max() > Mathf.Abs(data.Min()) ? data.Max() : Mathf.Abs(data.Min());
        float rateY = (height / 2 - marginY) / maxY;
        float bottomY = height / 2 + marginY;

        for (int i = 0; i < length; i++)
        {
            positions[i].x = intervalX * i;
            positions[i].y = data[i] * rateY + bottomY;
        }

        return positions;
    }

    private void RenderAll(float[] data)
    {
        Vector2[] posisions = Data2Positions(data);

        for (int i=0; i< posisions.Length-1; i++)
        {
            Vector2 start = posisions[i];
            Vector2 end = posisions[i + 1];
            RenderOne(start, end);
        }
    }

    private void RenderOne(Vector2 start, Vector2 end)
    {
        GameObject connection = new GameObject("connection", typeof(Image));
        connection.transform.SetParent(this.transform, false);
        RectTransform rect = connection.GetComponent<RectTransform>();

        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        Vector2 dir = (end - start).normalized;
        float distance = Vector2.Distance(start, end);

        rect.sizeDelta = new Vector2(distance, 2f);
        rect.anchoredPosition = new Vector2(0, 0);
        rect.anchoredPosition = start + dir * distance * .5f;
        rect.localEulerAngles = new Vector3(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);
    }

    private void Render(float[] data)
    {
        UILineRenderer lineRenderer = gameObject.GetComponent<UILineRenderer>();

        Vector2[] posisions = Data2Positions(AveragingData(data, (int)width));

        lineRenderer.SetPoints(posisions);

        lineRenderer.SetAllDirty(); //なぜかこれで再描画できる
    }
}
