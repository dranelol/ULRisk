using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(SerializedColor))]
public class SerializedColorInspector : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        float r = property.FindPropertyRelative("r").floatValue;
        float g = property.FindPropertyRelative("g").floatValue;
        float b = property.FindPropertyRelative("b").floatValue;
        float a = property.FindPropertyRelative("a").floatValue;

        SerializedColor c = new SerializedColor(EditorGUI.ColorField(position, label, new Color(r, g, b, a)));

        property.FindPropertyRelative("r").floatValue = c.GetColor.r;
        property.FindPropertyRelative("g").floatValue = c.GetColor.g;
        property.FindPropertyRelative("b").floatValue = c.GetColor.b;
        property.FindPropertyRelative("a").floatValue = c.GetColor.a;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16F;
    }
}
