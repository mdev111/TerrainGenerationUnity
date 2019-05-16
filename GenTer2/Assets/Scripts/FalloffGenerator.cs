using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FalloffGenerator
{
    public static float [,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                //*2 - 1 for getting range from 0 to 1 into range from -1 to 1
                float x = i /(float) size*2 -1;
                float y = j / (float)size * 2 - 1;

                //determine if x or y is closer to the edge of the square
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(y));

                map[i, j] = Evaluate(value);

            }
        }
        return map;
    }
    static float Evaluate(float value)
    {
        float a = 3;
        float b = 2.2f;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow((b - b * value), a));
    }
}
