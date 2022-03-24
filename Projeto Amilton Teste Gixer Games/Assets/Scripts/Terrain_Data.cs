using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Terrain_Data
{
    public float[] position;

    public Terrain_Data(PlayerTerrain playerterrain)
    {
        position = new float[3];
        position[0] = playerterrain.transform.position.x;
        position[1] = playerterrain.transform.position.y;
        position[2] = playerterrain.transform.position.z;
    }
}
