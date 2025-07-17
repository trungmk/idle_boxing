using UnityEditor;
using UnityEngine;

public class EditorCommonUtility
{
    public static void DrawBlockGUI(string lable, SerializedProperty property)
    {
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(lable, GUILayout.Width(50));
        EditorGUILayout.PropertyField(property, GUIContent.none);
        EditorGUILayout.EndHorizontal();
    }
}
