using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMesh : MonoBehaviour
{
    //      ---Public---
    public ComputeShader LerpShader;
    public ComputeShader CaseShader;
    public MarchingCubes marchingCubes;

    //      ---Private---
    private GameObject chunk;

    void Start()
    {
        marchingCubes = new MarchingCubes();
        marchingCubes.SetShaders(LerpShader, CaseShader);

        chunk = new GameObject("Check(0,0,0)");
        chunk.transform.parent = transform;
        
    }

    
    void Update()
    {

    }

    public void GenerateArea()
    {
        Mesh mesh = marchingCubes.GenerateMesh();

        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
        chunk.GetComponent<Renderer>();
        chunk.GetComponent<MeshFilter>().mesh = mesh;
        
    }
}
