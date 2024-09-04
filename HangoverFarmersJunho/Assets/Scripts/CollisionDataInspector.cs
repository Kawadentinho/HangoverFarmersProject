using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionDataInspector : MonoBehaviour
{
    public static void Show(SerializedProperty bools, SerializedProperty size, SerializedProperty width)
    {
        EditorGUILayout.LabelField("Collision Data", EditorStyles.boldLabel); //header
        EditorGUI.indentLevel++; //padding
        EditorGUILayout.PropertyField(size);
        EditorGUILayout.PropertyField(width);

        bools.arraySize = size.intValue; // control the size of the array via the exposed size variable
        //make sure the values don't go below 0
        if (size.intValue < 1) { size.intValue = 1; }
        if (width.intValue < 1) { width.intValue = 1; }

        //make the text of the selection Grid buttuns correspont to the contents of the array; size of the selection grid determined by the size of the string array we created here
        string[] buttunText = new string[bools.arraySize];
        for (int i = 0; i < buttunText.Length; i++) { buttunText[i] = bools.GetArrayElementAtIndex(i).boolValue ? "+" : " "; } //representing trues and falses as blocks and spaces. ternarys are useful

        //padding
        GUILayout.Space(3);
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        //selection grid hack; normaly it works like radio buttons, but setting the selected button to -1 means none of them are selected. number of columns determined by width. then just asthetic stuff for spacing the buttons
        int selected = GUILayout.SelectionGrid(-1, buttunText, width.intValue, new GUILayoutOption[] { GUILayout.Width(21 * width.intValue), GUILayout.Height(21 * ((size.intValue + width.intValue - 1) / width.intValue)) });
        GUILayout.EndHorizontal();

        //padding
        GUILayout.Space(3);
        GUILayout.BeginHorizontal();
        GUILayout.Space(15);
        //empty and fill buttons; set all values in the array to false/true respectively
        if (GUILayout.Button("Empty", new GUILayoutOption[] { GUILayout.Width(50) }))
        {
            for (int i = 0; i < bools.arraySize; i++) { bools.GetArrayElementAtIndex((int)i).boolValue = false;}
        }
        if (GUILayout.Button("Fill", new GUILayoutOption[] { GUILayout.Width(30) }))
        {
            for (int i = 0; i < bools.arraySize; i++) { bools.GetArrayElementAtIndex((int)i).boolValue = true; }
        }
        GUILayout.EndHorizontal() ;

        //when a button is pressed, check which one it is, toggle the boolean value at that position in the array, and then
        if (selected  != -1)
        {
            bools.GetArrayElementAtIndex(selected).boolValue = !bools.GetArrayElementAtIndex (selected).boolValue;
            selected = -1;
        }

        EditorGUI.indentLevel--;

    }
}
