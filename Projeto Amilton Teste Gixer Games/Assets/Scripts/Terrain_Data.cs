using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Terrain_Data
{
    public int xRes;
    public int yRes;
    public float[,] newsave;
    public TerrainData tData;

    public Terrain_Data(PlayerTerrain playerterrain)
    {
        //remover o modo apenas leitura da pasta do windows

        xRes = playerterrain.xRes;
        yRes = playerterrain.yRes;
        tData = playerterrain.tData;
        newsave = playerterrain.newsave;

    }
}
