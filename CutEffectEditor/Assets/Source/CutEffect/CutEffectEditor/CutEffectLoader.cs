using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.MemoryMappedFiles;

using UnityNamedPipe;
public class CutEffectLoader : MonoBehaviour
{

    AudioClip clip;

    [SerializeField]
    private MainThreadInvoker mainThreadInvoker;

    private NamedPipeServer server;

    // Start is called before the first frame update
    void Start()
    {
        InitServer();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitServer()
    {
        server = new NamedPipeServer();
        server.ReceivedEvent += Server_Received;
        server.Start("SamplePipeName");
    }

    private void OnApplicationQuit()
    {
        server.ReceivedEvent -= Server_Received;
        server.Stop();
    }

    private async void Server_Received(object sender, DataReceivedEventArgs e)
    {
        if (e.CommandType == typeof(PipeCommands.SendMessage))
        {
            var d = (PipeCommands.SendMessage)e.Data;
            mainThreadInvoker.BeginInvoke(() => //別スレッドからGameObjectに触るときはメインスレッドで処理すること
            {
                //audioManager_song.Play();
            });
        }
    }

    public AudioClip Load(string memory_name)
    {

        Debug.Log("CutEffectLoader.Load");

        // Open shared memory
        MemoryMappedFile share_mem = MemoryMappedFile.OpenExisting(memory_name);
        MemoryMappedViewAccessor accessor = share_mem.CreateViewAccessor();

        // Write data to shared memory
        int size = accessor.ReadInt32(0);
        byte[] data = new byte[size];
        accessor.ReadArray<byte>(sizeof(int), data, 0, data.Length);

        Debug.Log("DataSize = " + size);

        // Dispose resource
        accessor.Dispose();
        share_mem.Dispose();

        clip = WavUtility.ToAudioClip(data, 0, "wav");

        Debug.Log("clip = " + clip.loadState);

        return clip;
    }

    public AudioClip LoadWithPath(string path)
    {
        return null;
    }
}
