using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomNotes;
using CustomNotes.Data;

public static class BloqLoader
{
    static CustomNote note = null;
    // Start is called before the first frame update
    public static CustomNote Load()
    {
        if(note != null) return note;
        
        Debug.Log("Bloq Loader Start");
        string path = Application.streamingAssetsPath + "/Smol Note.bloq";

        note = new CustomNote(path);

        return note;
    }
}
