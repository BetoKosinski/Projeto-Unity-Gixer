using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem

{
    public static void SaveTerrain (PlayerTerrain playerterrain)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        //string path = "C:/System/" pra salvar em alguma pasta específica do PC
        string path = Application.persistentDataPath + "/terrain.bin";
        FileStream stream = new FileStream(path, FileMode.Create);

        Terrain_Data data = new Terrain_Data(playerterrain);

        formatter.Serialize(stream, data);
        stream.Close();
    }

    public static Terrain_Data LoadTerrain()
    {
        //string path = "C:/System/" pra salvar em alguma pasta específica do PC
        string path = Application.persistentDataPath + "/terrain.bin";
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            Terrain_Data data = formatter.Deserialize(stream) as Terrain_Data;
            stream.Close();

            return data;
        }
        else
        {
            Debug.LogError("Arquivo de terreno não encontrado" + path);
            return null;
        }
    }
}