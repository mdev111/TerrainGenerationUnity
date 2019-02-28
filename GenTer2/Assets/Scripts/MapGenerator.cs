using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public void GenerateMap()
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

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(drawMode == DrawMode.NoiseMap)
        {
display.DrawTexture(TextureGenerator.TextureFromHeightMap( noiseMap));
        }else if (drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap,mapChunkSize,mapChunkSize));
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier,meshHeightCurve, levelOfDetail), TextureGenerator.TextureFromColourMap(colourMap, mapChunkSize, mapChunkSize));
        }


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
}

[System.Serializable]// so that it will show up in the inspector
public struct TerrainType
{
    public string name;//label the terrain: water, grass
    public float height;// height at which this colour is applied (from 0 to 1 range-meaning: till(not from) what value of height this stuff is starting to show up as another color)
    public Color colour;//color of the terrain type
}