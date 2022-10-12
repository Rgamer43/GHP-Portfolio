using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{

    public static float[,] map = new float[10, 10];
    public static readonly int SMOOTHING_ITERATIONS = 360;
    public static readonly int WIDTH = 2500;
    public static readonly int HEIGHT = 2500;

    // Start is called before the first frame update
    void Start()
    {
        map = new float[WIDTH, HEIGHT];

        for (int i = 0; i < WIDTH; i++)
            for (int j = 0; j < HEIGHT; j++)
                map[i, j] = Random.Range(0.0f, 1.0f);

        float[,] newMap = map;
        for (int i = 0; i < SMOOTHING_ITERATIONS; i++)
        {
            for (int x = 0; x < WIDTH; x++)
                for (int y = 0; y < HEIGHT; y++)
                {
                    float newElev = newMap[x, y];
                    int divisor = 1;
                    if (x > 0)
                    {
                        newElev += newMap[x - 1, y];
                        divisor++;
                    }
                    if (x + 1 < WIDTH)
                    {
                        newElev += newMap[x + 1, y];
                        divisor++;
                    }
                    if (y + 1 < HEIGHT)
                    {
                        newElev += newMap[x, y + 1];
                        divisor++;
                    }
                    if (y > 0)
                    {
                        newElev += newMap[x, y - 1];
                        divisor++;
                    }

                    newElev /= divisor;
                    newMap[x, y] = newElev;
                }
            map = newMap;
        }

        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                //GameObject n = Instantiate(cube, new Vector3(x, 0, y), Quaternion.identity);
                //n.transform.localScale = new Vector3(1, (float)(map[x, y] * 10), 1);
            }
                
        }


        Terrain terrain = GameObject.Find("Terrain").GetComponent<Terrain>();
        terrain.terrainData.size = new Vector3(WIDTH, 1000, HEIGHT);
        terrain.terrainData.SetHeights(0, 0, map);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
