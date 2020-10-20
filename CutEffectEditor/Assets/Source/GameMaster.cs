using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

using Log = DefaultLogger;

public class GameMaster : MonoBehaviour
{
    private MapPlayer mapPlayer = new MapPlayer();

    private CutEffectPlayer cutEffectPlayer = new CutEffectPlayer();
    public CutEffectPitchController pitchController;
    public CutEffectLoader cutEffectLoader;

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
            mapPlayer.SendNote += cutEffectPlayer.Play;
        } 
        if (forEdit) await cutEffectPlayer.TryLoadCutEffects();
        
        string songPath = customLevelPath + "2a7 (Night of Nights - squeaksies)\\Night of Nights (Flowering nights remix).egg";
        string mapPath = customLevelPath + "2a7 (Night of Nights - squeaksies)\\ExpertPlus.dat";

        bool mapPlayerInited = await mapPlayer.TryInit(songPath, mapPath, gameObject, cutEffectPlayer);
        if (!mapPlayerInited){
            Log.Write("mapPlayer is not Inited");
            return;
        } else {
            if (forEdit) mapPlayer.InitCutEffectMapForEdit();
            mapPlayer.Play();
        }

        if (pitchController != null)
        {
            pitchController.Init(CutEffectPlayer.TAP_NUM, cutEffectPlayer);
            mapPlayer.SendNote += pitchController.ShowNote;
        }
    }

    // Update is called once per frame
    void Update()
    {
        mapPlayer.Update();
    }
}
