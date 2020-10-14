using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CutEffectMapLoader
{
    static string cutEffectMapPath;

    public static CutEffectMap Load(string path)
    {
        return JsonParser.Parse<CutEffectMap>(path);
    }
}
