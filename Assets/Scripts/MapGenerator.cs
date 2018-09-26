using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour {

    public enum DrawMode
    {
        NoiseMap, ColourMap, Mesh
    };

    public DrawMode drawMode;


    const int mapChunkSize = 241;
    [Range(0,6)]
    public int levelOfDetail;

    public int octaves;
    [Range(0, 1)]
    public float persistance;
    public float lacunarity;

    public float meshheightMultiplier;
    public AnimationCurve meshHeightCurve;

    public float noiseScale;
    public bool autoUpdate;
   
    public int seed;
    public Vector2 offset;
    public TerrainType[] regions;
    public GameObject FenceModel;
    public GameObject[] CropModel;
    public GameObject[] HayModel;
    public GameObject[] TreeModel;
    public GameObject[] Emptys;
    public GameObject StumpModel;
    public GameObject CornRegion, GrainRegion, EmptyRegion; 
    GameObject Crops, Fences, Hay, Tree, Stump;
    GameObject[] EmptyClones;
    bool hay = true;
    int treecount, haycount, cropcount = 0;
    int croptype, treetype, haytype = 0;
    float treegenerator, stumpgenerator, haygenerator = 0;
    Vector3 pos;
    Quaternion quat;
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale, octaves, persistance, lacunarity, offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];

        //Destroy all current assets for reload
        //for (int i = 0; i < Emptys.Length; i++) 
        //{
        //    EmptyClones[i] = Emptys[i];
        ////    Destroy(EmptyClones[i]);
        //}

        //Generate emptys to store the assets, clarity for editor
        for (int j = 0; j < Emptys.Length; j++) 
            {
            Instantiate(Emptys[j], Emptys[j].transform.position, Quaternion.identity);
            }

      

        Random.InitState(seed);
        //var Hay = Instantiate(FenceEmpty, transform.position, Quaternion.identity);
        //Destroy(FenceClone);

        for (int y = 0; y < mapChunkSize; y++)
        {
            for(int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if (currentHeight <= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].color;
                        break;
                    }
                }


                //Tree
                if (currentHeight < 0.65 && currentHeight >= 0.57)
                {
                    if (treecount % 20 == 0)
                        treetype = Random.Range(0, 2);

                    pos = new Vector3(y, 0.5f, x);
                    quat = Quaternion.Euler(0, 0, 0);

                    //tree dispersal
                    treegenerator = Random.Range(0f, 1f);
                    if (treegenerator >= 0.8)
                    {
                        Tree = Instantiate(TreeModel[treetype], pos, quat);
                        Tree.transform.parent = GameObject.Find("TreeEmpty(Clone)").transform;
                        treecount++;
                   }
                }


                //RedTree
                if (currentHeight < 0.69 && currentHeight >= 0.65)
                {

                    pos = new Vector3(y, 0.5f, x);
                    quat = Quaternion.Euler(0, 0, 0);

                    //tree dispersal
                    treegenerator = Random.Range(0f, 1f);
                    if (treegenerator >= 0.9)
                    {
                        
                        Tree = Instantiate(TreeModel[2], pos, quat);
                        Tree.transform.parent = GameObject.Find("TreeEmpty(Clone)").transform;
                        
                    }
                }


                //Tree Stumps
                if (currentHeight > 0.56 && currentHeight < 0.57)
                {
                    pos = new Vector3(y, currentHeight, x);
                    quat = Quaternion.Euler(0, 0, 0);

                    //stump dispersal
                    stumpgenerator = Random.Range(0f, 1f);
                    if (stumpgenerator >= 0.8)
                   {
                        //Generate stump
                        Stump = Instantiate(StumpModel, pos, quat);
                        Stump.transform.parent = GameObject.Find("StumpEmpty(Clone)").transform;
                    }

                }

            


                //Fences
                if (currentHeight <= 0.33 && currentHeight >= 0.3)
                {
                    //Check inrange of Mesh
                    if (((x - 1) >= 0 && (y - 1) >= 0) && (x + 1) < mapChunkSize && (y + 1) < mapChunkSize)
                    {

                        //Checks surrounding area of fence height, if there is grain next to the height, place a fence.
                        if (noiseMap[x - 1, y] <= 0.3 || //West
                            noiseMap[x + 1, y] <= 0.3 || //East
                            noiseMap[x, y - 1] <= 0.3 || //South
                            noiseMap[x, y + 1] <= 0.3 || //North
                            noiseMap[x + 1, y + 1] <= 0.3 || // North-East
                            noiseMap[x - 1, y + 1] <= 0.3 || // North-West
                            noiseMap[x - 1, y - 1] <= 0.3 || // South-West
                            noiseMap[x + 1, y - 1] <= 0.3    // South-East
                            )

                        {
                            //Generate fence
                            pos = new Vector3(y, 0, (x-0.5f));
                            quat = Quaternion.Euler(0, 0, 0);
                            Fences = Instantiate(FenceModel, pos, quat);
                            Fences.transform.parent = GameObject.Find("FenceEmpty(Clone)").transform;
                        }


                    }
        
                }



                //Could create 3 separate gameobject arrays for corn, grain and empty, and have one random number generated at the start of the function to generate all of one type for each region. 
                //Crop
                if (currentHeight < 0.25 && currentHeight >= 0.1)
                {

                    pos = new Vector3(y, -0.25f, x);
                    quat = Quaternion.Euler(0, 0, 0);

                //Grain Region
                    if (pos.x <= (GrainRegion.transform.position.x + 40) && pos.x >= (GrainRegion.transform.position.x - 40) && pos.z <= (GrainRegion.transform.position.z + 120) && pos.z >= (GrainRegion.transform.position.z - 120)) {
                        //Random grain variant
                        croptype = Random.Range(4, 6);
                        Crops = Instantiate(CropModel[croptype], pos, quat);
                        Crops.transform.parent = GameObject.Find("Grain Region(Clone)").transform;
                    }

                //Empty Crop region
                    if (pos.x <= (EmptyRegion.transform.position.x + 80) && pos.x >= (EmptyRegion.transform.position.x - 80) && pos.z <= (EmptyRegion.transform.position.z + 48) && pos.z >= (EmptyRegion.transform.position.z - 44)) {
                        //Random Empty crop variant
                        croptype = Random.Range(2, 4);
                        Crops = Instantiate(CropModel[croptype], pos, quat);
                        Crops.transform.parent = GameObject.Find("Empty Region(Clone)").transform;
                    }

                //Corn region
                    if (pos.x <= (CornRegion.transform.position.x + 80) && pos.x >= (CornRegion.transform.position.x - 80) && pos.z <= (CornRegion.transform.position.z + 76) && pos.z >= (CornRegion.transform.position.z - 70)) 
                    {
                        //Random corn variant
                        croptype = Random.Range(0, 2);
                        Crops = Instantiate(CropModel[croptype], pos, quat);
                        Crops.transform.parent = GameObject.Find("Corn Region(Clone)").transform;
                    }

                }


                //Hay
                if (currentHeight < 0.045 && currentHeight >= 0)
                {
                    if(haycount % 2 == 0)
                    {
                        pos = new Vector3(y, -.3f, x);
                        quat = Quaternion.Euler(0, 0, 0);

                        //Hay dispersal
                    haygenerator = Random.Range(0f, 1f);
                    if (haygenerator > 0.9)
                    {
                        haytype = Random.Range(0, 2);
                        Hay = Instantiate(HayModel[haytype], pos, quat);
                    
                        //To allow for full rows of a type of crop, rather than individual crops being randomly interlaced.
                        croptype = Random.Range(0, 5);

                        Hay.transform.parent = GameObject.Find("HayEmpty(Clone)").transform;
                    }
                   }

                     haycount++;

                }

               
            }
        }

       
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        }
        else if (drawMode == DrawMode.ColourMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshheightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }

        //Fence spawn
        //Vector3 pos = new Vector3(0, 0, 0);
        //Quaternion quat = Quaternion.Euler(0, 0, 0);
        //Instantiate(Model, pos, quat);
    }

    void OnValidate()
    {
        if(lacunarity < 1)
        {
            lacunarity = 1;
        }
        if(octaves < 0 )
        {
            octaves = 0;
        }

    }

}


[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}