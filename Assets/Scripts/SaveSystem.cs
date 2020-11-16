using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{
    public static void SaveRecordData(int wins, int losses)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/player.cgs";
        FileStream stream = new FileStream(path, FileMode.Create);
        RecordData data = new RecordData(wins, losses);
        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static RecordData LoadRecordData()
    {
        string path = Application.persistentDataPath + "/player.cgs";
        if(File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            RecordData data = formatter.Deserialize(stream) as RecordData;
            stream.Close();
            return data;
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
            return null;
        }
    }
}
