using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System; //to get access to actions
using System.Threading;


//working with meshes and textures in unity is olny allowed from the main thread
public class MapGenerator : MonoBehaviour
{
    public enum DrawMode
    {
        NoiseMap, ColorMap, Mesh
    };
    public DrawMode drawMode;

    public const int mapChunkSize = 241;//because 241-1 is divisible by 2,4,6,8,10,12
    [Range(0,6)]
    public int levelOfDetail;//*2 =2,4,6,8,10,12 - the bigger = the less vertices will be evaluated, (good if too big terrain)
    public float noiseScale;

    public int octaves;
    [Range(0,1)]
    public float persistance;// should always be between 0 and 1
    public float lacunarity;

    public int seed;
    public Vector2 offset;
    public float meshHeightMultiplier;

    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    Queue<MapThreadInfo<MapData>> mapDataThreadInfoQueue = new Queue<MapThreadInfo<MapData>>();
    Queue<MapThreadInfo<MeshData>> meshDataThreadInfoQueue = new Queue<MapThreadInfo<MeshData>>();


    public void DrawMapInEditor()
    {
        MapData mapData = GenerateMapData();
        MapDisplay display = FindObjectOfType<MapDisplay>();
        if (drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(mapData.heightMap));
        }
        else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(mapData.heightMap, meshHeightMultiplier, meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(mapData.colourMap, mapChunkSize, mapChunkSize));
        }
    }
    //MapData is type of parameter method expects
    public void RequestMapData(Action<MapData> callback)
    {

        //delegate

        //mapDataThread with the callback parameter
        ThreadStart threadStart = delegate
        {
            MapDataThread(callback);
        };
        new Thread(threadStart).Start();
        // now MapDataThread is running on the different thread
    }

    void MapDataThread(Action<MapData> callback)
    {
        MapData mapData = GenerateMapData();
        //if we call a method inside of a thread, this method will be executed from the very same thread as well
        //now add this mapdata together with the callback to the queue
        //mutex so that only one thread can access the queue at a time
        lock (mapDataThreadInfoQueue) {
            mapDataThreadInfoQueue.Enqueue(new MapThreadInfo<MapData>(callback, mapData));
        }
       
    }

    //MapData is type of parameter method expects
    public void RequestMeshData(MapData mapData, Action<MeshData> callback)
    {

        //delegate

        //mapDataThread with the callback parameter
        ThreadStart threadStart = delegate
        {
            MeshDataThread(mapData, callback);
        };
        new Thread(threadStart).Start();
        // now MapDataThread is running on the different thread
    }

    void MeshDataThread(MapData mapData, Action<MeshData> callback)
    {
        MeshData meshData = MeshGenerator.GenerateTerrainMesh(mapData.heightMap,meshHeightMultiplier, meshHeightCurve,levelOfDetail);
        //if we call a method inside of a thread, this method will be executed from the very same thread as well
        //now add this mapdata together with the callback to the queue
        //mutex so that only one thread can access the queue at a time
        lock (meshDataThreadInfoQueue)
        {
            meshDataThreadInfoQueue.Enqueue(new MapThreadInfo<MeshData>(callback, meshData));
        }

    }

    void Update()
    {
        if(mapDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < mapDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MapData> threadInfo = mapDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
        if (meshDataThreadInfoQueue.Count > 0)
        {
            for (int i = 0; i < meshDataThreadInfoQueue.Count; i++)
            {
                MapThreadInfo<MeshData> threadInfo = meshDataThreadInfoQueue.Dequeue();
                threadInfo.callback(threadInfo.parameter);
            }
        }
    }

    MapData GenerateMapData()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapChunkSize, mapChunkSize, seed, noiseScale,octaves,persistance,lacunarity,offset);

        Color[] colourMap = new Color[mapChunkSize * mapChunkSize];
        for (int y = 0; y < mapChunkSize; y++)
        {
            for (int x = 0; x < mapChunkSize; x++)
            {
                float currentHeight = noiseMap[x, y];
                for (int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight<= regions[i].height)
                    {
                        colourMap[y * mapChunkSize + x] = regions[i].colour;
                        break;
                    }

                }

            }
        }

        return new MapData(noiseMap, colourMap);


    }

    private void OnValidate() //called every time the variables changed in the inspector
    {
        
        if (lacunarity <1)
        {
            lacunarity = 1;
        }
        if(octaves < 1)
        {
            octaves = 1;
        }
    }
    //generic so that it can handle mapdata and also meshdata
    struct MapThreadInfo<T>
    {
        //readonly, as struct should be immutable = cannot change the values of the variable after initialization
        public readonly Action<T> callback;
        public readonly T parameter;

        public MapThreadInfo(Action<T> callback, T parameter)
        {
            this.callback = callback;
            this.parameter = parameter;
        }
    }
}

[System.Serializable]// so that it will show up in the inspector
public struct TerrainType
{ 
    //not readonly as it will not show in the inspector
    public string name;//label the terrain: water, grass
    public float height;// height at which this colour is applied (from 0 to 1 range-meaning: till(not from) what value of height this stuff is starting to show up as another color)
    public Color colour;//color of the terrain type
}

public struct MapData
{
    public readonly float[,] heightMap;
    public readonly Color[] colourMap;
    
    public MapData(float [,] heightMap, Color[] colourMap)
    {
        this.heightMap = heightMap;
        this.colourMap = colourMap;
    }
}