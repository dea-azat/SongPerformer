using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

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
        cutEffectPlayer.Init(gameObject);

        string cutEffectMapPath = "D:\\ProgramFiles\\Steam\\steamapps\\common\\Beat Saber\\Beat Saber_Data\\CustomLevels\\2a7 (Night of Nights - squeaksies)\\ExpertPlusEffect.dat";
        if (forPlay){
            await cutEffectPlayer.TryLoadCutEffectMap(cutEffectMapPath);
            mapPlayer.SendNote += cutEffectPlayer.Play;
        } 
        if (forEdit) await cutEffectPlayer.TryLoadCutEffects();
        
        bool mapPlayerInited = await mapPlayer.TryInit(gameObject, cutEffectPlayer);
        if (!mapPlayerInited) return;

        if (forEdit) mapPlayer.InitCutEffectMapForEdit();

        mapPlayer.Play();

        if (pitchController != null)
        {
            pitchController.Init(CutEffectPlayer.TAP_NUM, cutEffectPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        mapPlayer.Update();
    }
}
