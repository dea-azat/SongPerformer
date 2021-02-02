using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using SongPerformer;

using Log = DefaultLogger;

public class GameMaster : MonoBehaviour
{
    private CutEffectPlayer cutEffectPlayer = new CutEffectPlayer();
    public CutEffectPitchController pitchController;
    public CutEffectLoader cutEffectLoader;
    public MapPlayer mapPlayer;

    bool forEdit = false;
    bool forPlay = true;

    // Start is called before the first frame update
    async void Start()
    {
        string customLevelPath = "D:\\ProgramFiles\\Steam\\steamapps\\common\\Beat Saber\\Beat Saber_Data\\CustomLevels\\";

        cutEffectPlayer.Init(gameObject);

        string cutEffectMapPath = customLevelPath + "2a7 (Night of Nights - squeaksies)\\ExpertPlusEffect.dat";
        if (forPlay){
            await cutEffectPlayer.TryLoadCutEffectMap(cutEffectMapPath);
            mapPlayer.OnSendNote.AddListener(cutEffectPlayer.Play);
        } 
        if (forEdit) await cutEffectPlayer.TryLoadCutEffects();
        
        string songPath = customLevelPath + "2a7 (Night of Nights - squeaksies)\\Night of Nights (Flowering nights remix).egg";
        string mapPath = customLevelPath + "2a7 (Night of Nights - squeaksies)\\ExpertPlus.dat";

        bool mapPlayerInited = await mapPlayer.TryInit(songPath, mapPath, gameObject);
        if (mapPlayerInited){
            //if (forEdit) mapPlayer.InitCutEffectMapForEdit();
            AdjustTapKey(mapPlayer.AudioManager_Song.GetAudioClip(), cutEffectPlayer); //必要なのはEditのときだけでは？
            mapPlayer.Play();
        } else {
            Log.Write("mapPlayer is not Inited");
            return;
        }

        if (pitchController != null)
        {
            pitchController.Init(CutEffectPlayer.TAP_NUM, cutEffectPlayer);
            mapPlayer.OnSendNote.AddListener(pitchController.ShowNote);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mapPlayer.Update();
    }

    private void AdjustTapKey(AudioClip audioClip, CutEffectPlayer cutEffect)
    {
        float pitch = KeyFinder.AdjustPitch(audioClip, 21); //KeyFinder側に定数マクロを持つべきだが...

        cutEffect.SetPitch(pitch);
    }
}
