using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestDoTween))]
public class TestDoTweenEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        //GUILayout.Button("asd");
    }
}

[CustomPropertyDrawer(typeof(TestClass))]
public class TestClassPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
    }
}

[CustomPropertyDrawer(typeof(Gen<>))]
public class GenPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        GUILayout.Label("Gen");
        EditorGUI.PropertyField(position, property.FindPropertyRelative("Value"), label, true);
    }
}

