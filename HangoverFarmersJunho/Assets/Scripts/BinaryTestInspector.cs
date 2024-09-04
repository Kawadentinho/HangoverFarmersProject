using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(binaryArrayTest))]
public class BinaryTestInspector : Editor
{
    SerializedProperty bools;
    SerializedProperty width;
    SerializedProperty size;


    private void OnEnable()
    {
        bools = this.serializedObject.FindProperty("initialBools");
        width = this.serializedObject.FindProperty("width");
        size = this.serializedObject.FindProperty("size");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        //all the heavy lifiting is in a global function so this can be reused
        CollisionDataInspector.Show(bools, size, width);

        serializedObject.ApplyModifiedProperties();
    }

}
