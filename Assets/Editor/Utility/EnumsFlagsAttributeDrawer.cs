using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnumsFlagsAttribute))]
public class EnumsFlagsAttributeDrawer : PropertyDrawer {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        property.intValue = EditorGUI.MaskField(position, property.intValue, property.enumNames);
    }
}
