using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public static class JsonParser
{
    public static dynamic Parse(string path)
    {
        string jsonText = File2String(ReadFile(path));

        dynamic json = JsonConvert.DeserializeObject(jsonText);

        return json;
    }

    public static T Parse<T>(string path)
    {
        string jsonText = File2String(ReadFile(path));

        T json = JsonConvert.DeserializeObject<T>(jsonText);

        return json;
    }

    static FileInfo ReadFile(string path)
    {

        string filepath = path;
        Debug.Log(filepath);

        // FileReadTest.txtファイルを読み込む
        FileInfo fi = new FileInfo(filepath);
        return fi;
        
    }

    static string File2String(FileInfo fi)
    {
        string text = "";

        try
        {
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                text = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            // 改行コード
            text += "error";
        }

        return text;
    }
}

public static class JsonWriter
{
    public static void WriteOut<T>(T obj, string path)
    {
        string output = JsonConvert.SerializeObject(obj);
        File.WriteAllText(path, output);
    }
}