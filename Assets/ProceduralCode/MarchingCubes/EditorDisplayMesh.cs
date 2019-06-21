
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DisplayMesh))]
[CanEditMultipleObjects]
public class EditorDisplayMesh : Editor
{
    public ComputeShader LerpShader;
    public ComputeShader CaseShader;

    public override void OnInspectorGUI()
    {
        DisplayMesh display = (DisplayMesh)target;

        if (GUILayout.Button("Generate"))
        {
            display.GenerateArea();
        }
    }
}
