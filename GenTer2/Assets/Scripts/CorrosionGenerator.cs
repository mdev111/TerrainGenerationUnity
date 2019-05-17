using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CorrosionMapGenerator
{
    static float[,] map;
    static int size;
    static float val = 0.2f;
    static float[,] GenerateCorrosionMap(int size)
    {
        map = new float[size, size];
        CorrosionMapGenerator.size = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //*2 - 1 for getting range from 0 to 1 into range from -1 to 1
                float x = i / (float)size * 2 - 1;
                float y = j / (float)size * 2 - 1;

               float value = val;

                map[i, j] = value;

            }
        }
        return map;
    }
    public static float[,] GetCorrosionMap(int size)
    {
        if(map == null)
        {
            return CorrosionMapGenerator.GenerateCorrosionMap(size);
        }
        return map;
    }
        static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow((b - b * value), a));
    }

    public static void ReevaluateMap(float value)
    {
        val = -value;
         CorrosionMapGenerator.GenerateCorrosionMap(size);
        //for (int i = 0; i < size; i++)
        //{
        //    for (int j = 0; j < size; j++)
        //    {
        //        //*2 - 1 for getting range from 0 to 1 into range from -1 to 1
        //        float x = i / (float)size * 2 - 1;
        //        float y = j / (float)size * 2 - 1;

        //        //float value = 0;

        //        map[i, j] = Evaluate(map[i, j] - value);

        //    }
        //}
        //  return map;
    }

   
  
}
