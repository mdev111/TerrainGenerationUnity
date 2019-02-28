using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndlessTerrain : MonoBehaviour
{
    public const float maxViewDistance = 300;//const to not be able to change at runtime

    public static Vector2 viewerPoistion;
    int chunkSize;
    //based on the chunkSize and max view distance: how many chunks are visible
    int chunksVisibleInViewDistance;

    void Start()
    {
        chunkSize = MapGenerator.mapChunkSize - 1;
        chunksVisibleInViewDistance = Mathf.RoundToInt(maxViewDistance / chunkSize);
    }

}
