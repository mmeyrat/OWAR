using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public class FileTagList
{
    public List<string> fileList;
    public List<string> tagList;

    private static string jsonFilePath = $"{Application.streamingAssetsPath} /FileTagList.json";

    public static FileTagList GetFileTagList()
    {
        StreamReader reader = new StreamReader(jsonFilePath);
        return JsonUtility.FromJson<FileTagList>(reader.ReadToEnd());
    }
}
