using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DisplayMesh))]
public class EditorDisplayMesh : Editor
{
    public override void OnInspectorGUI()
    {
        DisplayMesh display = (DisplayMesh)target;

        if (GUILayout.Button("Generate"))
        {
            display.GenerateArea();
        }
    }
}
