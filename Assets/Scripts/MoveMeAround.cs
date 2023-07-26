using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveMeAround : MonoBehaviour
{
    float speed = 20;
    float nx, ny, nz;
    float nC = 0.03f;
    // Start is called before the first frame update
    void Start()
    {
         nx = Random.value * 100;
         ny = Random.value * 100;
         nz = Random.value * 100;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.position += new Vector3((Mathf.PerlinNoise(nx,nx)-0.5f)*speed, (Mathf.PerlinNoise(ny, ny) - 0.5f) * speed, (Mathf.PerlinNoise(nz, nz) - 0.5f) * speed);
        nx += nC;
        ny += nC;
        nz += nC;
    }
}
