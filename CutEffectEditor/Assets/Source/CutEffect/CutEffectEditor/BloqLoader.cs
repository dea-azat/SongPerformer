using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CustomNotes;
using CustomNotes.Data;

public class BloqLoader
{
    // Start is called before the first frame update
    public CustomNote Load()
    {
        Debug.Log("Bloq Loader Start");
        string path = Application.streamingAssetsPath + "/Smol Note.bloq";

        CustomNote note = new CustomNote(path);

        return note;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
