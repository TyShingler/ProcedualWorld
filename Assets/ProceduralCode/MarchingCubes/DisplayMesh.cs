using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class DisplayMesh : MonoBehaviour
{
    //      ---Public---
    public ComputeShader LerpShader;
    public ComputeShader CaseShader;

    //      ---Private---
    private MarchingCubes marchingCubes;
    private GameObject chunk;

    void Start()
    {
        marchingCubes = new MarchingCubes();
        marchingCubes.SetShaders(LerpShader, CaseShader);

        chunk = new GameObject("Check(0,0,0)");
        chunk.transform.parent = transform;

        GenerateArea();
        
    }

    
    void Update()
    {

    }

    public void GenerateArea()
    {
        Mesh mesh = marchingCubes.GenerateMesh();

        chunk.AddComponent<MeshFilter>();
        chunk.AddComponent<MeshRenderer>();
        chunk.GetComponent<Renderer>().material = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/DefaultTerrain.mat");
        chunk.GetComponent<MeshFilter>().mesh = mesh;


    }
}
