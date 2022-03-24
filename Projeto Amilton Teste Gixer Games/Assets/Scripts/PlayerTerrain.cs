using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class PlayerTerrain : MonoBehaviour
{
    public enum DeformMode { RaiseLower }
    DeformMode deformMode = DeformMode.RaiseLower;
    string[] deformModeNames = new string[] { "Raise Lower" };

    public Terrain terrain;
    public Texture2D deformTexture;
    public float strength = 4;
    public float area = 2;
    
    Transform buildTarget;
    Vector3 buildTargPos;
    Light spotLight;

    //GUI
    Rect windowRect = new Rect(10, 10, 400, 75);
    bool onWindow = false;
    bool onTerrain;
    Texture2D newTex;
    float strengthSave;
   
    //Raycast
    private RaycastHit hit;

    //Deformation variables
    private int xRes;
    private int yRes;
    private float[,] saved;
    private float[,] newsave;
    Color[] craterData;

    TerrainData tData;

    float strengthNormalized
    {
        get
        {
            return (strength) / 9.0f;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //Create build target object
        GameObject tmpObj = new GameObject("BuildTarget");
        buildTarget = tmpObj.transform;

        //Add Spot Light to build target
        GameObject spotLightObj = new GameObject("SpotLight");
        spotLightObj.transform.SetParent(buildTarget);
        spotLightObj.transform.localPosition = new Vector3(0, 2, 0);
        spotLightObj.transform.localEulerAngles = new Vector3(90, 0, 0);
        spotLight = spotLightObj.AddComponent<Light>();
        spotLight.type = LightType.Spot;
        spotLight.range = 20;

        tData = terrain.terrainData;
        if (tData)
        {
            //Save original height data
            xRes = tData.heightmapResolution;
            yRes = tData.heightmapResolution;
            saved = tData.GetHeights(0, 0, xRes, yRes);
        }

        //Change terrain layer to UI
        terrain.gameObject.layer = 5;
        strength = 4;
        area = 2;
        brushScaling();
    }

    void FixedUpdate()
    {
        raycastHit();
        wheelValuesControl();

        if (onTerrain && !onWindow)
        {
            terrainDeform();
        }

        //Update Spot Light Angle according to the Area value
        spotLight.spotAngle = area * 25f;
    }

    //Raycast
    //______________________________________________________________________________________________________________________________
    void raycastHit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = new RaycastHit();
        //Do Raycast hit only against UI layer
        if (Physics.Raycast(ray, out hit, 300, 1 << 5))
        {
            onTerrain = true;
            if (buildTarget)
            {
                buildTarget.position = Vector3.Lerp(buildTarget.position, hit.point + new Vector3(0, 1, 0), Time.time);
            }
        }
        else
        {
            if (buildTarget)
            {
                Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 200);
                Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
                buildTarget.position = curPosition;
                onTerrain = false;
            }
        }
    }

    //TerrainDeformation
    //___________________________________________________________________________________________________________________
    void terrainDeform()
    {
        if (Input.GetMouseButtonDown(0))
        {
            buildTargPos = buildTarget.position - terrain.GetPosition();
            float x = Mathf.Clamp01(buildTargPos.x / tData.size.x);
            float y = Mathf.Clamp01(buildTargPos.z / tData.size.z);
        }

        if (Input.GetMouseButtonDown(1))
        {
            buildTargPos = buildTarget.position - terrain.GetPosition();
            float x = Mathf.Clamp01(buildTargPos.x / tData.size.x);
            float y = Mathf.Clamp01(buildTargPos.z / tData.size.z);
        }

        //Terrain deform up
        if (Input.GetMouseButton(0))
        {
            buildTargPos = buildTarget.position - terrain.GetPosition();

            strengthSave = -strength;

            if (newTex && tData && craterData != null)
            {
                int x = (int)Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, tData.size.x, buildTargPos.x));
                int z = (int)Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, tData.size.z, buildTargPos.z));
                x = Mathf.Clamp(x, newTex.width / 2, xRes - newTex.width / 2);
                z = Mathf.Clamp(z, newTex.height / 2, yRes - newTex.height / 2);
                int startX = x - newTex.width / 2;
                int startY = z - newTex.height / 2;
                float[,] areaT = tData.GetHeights(startX, startY, newTex.width, newTex.height);
                for (int i = 0; i < newTex.height; i++)
                {
                    for (int j = 0; j < newTex.width; j++)
                    {
                        if (deformMode == DeformMode.RaiseLower)
                        {
                            areaT[i, j] = areaT[i, j] - craterData[i * newTex.width + j].a * strengthSave / 15000;
                        }
                    }
                }
                tData.SetHeights(x - newTex.width / 2, z - newTex.height / 2, areaT);
            }
        }

        //Terrain deform down
        if (Input.GetMouseButton(1))
        {
            buildTargPos = buildTarget.position - terrain.GetPosition();

            strengthSave = strength;

            if (newTex && tData && craterData != null)
            {
                int x = (int)Mathf.Lerp(0, xRes, Mathf.InverseLerp(0, tData.size.x, buildTargPos.x));
                int z = (int)Mathf.Lerp(0, yRes, Mathf.InverseLerp(0, tData.size.z, buildTargPos.z));
                x = Mathf.Clamp(x, newTex.width / 2, xRes - newTex.width / 2);
                z = Mathf.Clamp(z, newTex.height / 2, yRes - newTex.height / 2);
                int startX = x - newTex.width / 2;
                int startY = z - newTex.height / 2;
                float[,] areaT = tData.GetHeights(startX, startY, newTex.width, newTex.height);
                for (int i = 0; i < newTex.height; i++)
                {
                    for (int j = 0; j < newTex.width; j++)
                    {
                        if (deformMode == DeformMode.RaiseLower)
                        {
                            areaT[i, j] = areaT[i, j] - craterData[i * newTex.width + j].a * strengthSave / 15000;
                        }
                    }
                }
                tData.SetHeights(x - newTex.width / 2, z - newTex.height / 2, areaT);
            }
        }
    }

    void brushScaling()
    {
        //Apply current deform texture resolution 
        newTex = Instantiate(deformTexture) as Texture2D;
        TextureScale.Point(newTex, deformTexture.width * (int)area / 10, deformTexture.height * (int)area / 10);
        newTex.Apply();
        craterData = newTex.GetPixels();
    }

    void wheelValuesControl()
    {
        float mouseWheel = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(mouseWheel) > 0.0)
        {
            if (mouseWheel > 0.0)
            {
                //More
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (area < 13)
                    {
                        area += 0.5f;
                    }
                    else
                    {
                        area = 13;
                    }
                }
                else
                {
                    if (strength < 13)
                    {
                        strength += 0.5f;
                    }
                    else
                    {
                        strength = 13;
                    }
                }
            }
            else if (mouseWheel < 0.0)
            {
                //Less
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    if (area > 2)
                    {
                        area -= 0.5f;
                    }
                    else
                    {
                        area = 2;
                    }
                }
                else
                {
                    if (strength > 4)
                    {
                        strength -= 0.5f;
                    }
                    else
                    {
                        strength = 4;
                    }
                }
            }
            if (area > 1)
                brushScaling();
        }
    }

    //GUI
    //______________________________________________________________________________________________________________________________
    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 50, 30), "Save"))
        {
            SaveTerrain();
        }

        if (GUI.Button(new Rect(10, 50, 50, 30), "Load"))
        {
           LoadTerrain();
        }

        //Detect when mouse cursor is inside region 
        GUILayout.BeginArea(new Rect(0, 0, 100, 120));

        if (GUILayoutUtility.GetRect(0, 0, 100, 120).Contains(Event.current.mousePosition))
        {
            onWindow = true;
        }
        else
        {
            onWindow = false;
        }

        GUILayout.EndArea();

    }
                
    void OnApplicationQuit()
    {
        //Reset terrain height when exiting play mode
        tData.SetHeights(0, 0, saved);
    }

    public void SaveTerrain()
    {
        xRes = tData.heightmapResolution;
        yRes = tData.heightmapResolution;
        newsave = tData.GetHeights(0, 0, xRes, yRes);
        SaveSystem.SaveTerrain(this);
        Debug.Log("Clicou em salvar");
    }

    public void LoadTerrain()
    {
        tData.SetHeights(0, 0, newsave);

        Terrain_Data data = SaveSystem.LoadTerrain();

        Vector3 position;
        position.x = data.position[0];
        position.y = data.position[1];
        position.z = data.position[2];
        transform.position = position;

        Debug.Log("Clicou em carregar");
    }
        
}
