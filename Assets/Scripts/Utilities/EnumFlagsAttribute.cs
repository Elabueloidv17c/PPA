using UnityEngine;
using UnityEditor;
using System;

namespace Lau_Utilities
{
  public class EnumFlagsAttribute : PropertyAttribute
  {
    public string name;

    public EnumFlagsAttribute() { }

    public EnumFlagsAttribute(string name)
    {
      this.name = name;
    }
  }
#if UNITY_EDITOR

  [CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
  public class EnumFlagDrawer : PropertyDrawer
  {
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
      EnumFlagsAttribute flagSettings = (EnumFlagsAttribute)attribute;
      Enum targetEnum = (Enum)fieldInfo.GetValue(property.serializedObject.targetObject);

      string propName = flagSettings.name;
      if (string.IsNullOrEmpty(propName))
        propName = ObjectNames.NicifyVariableName(property.name);

      EditorGUI.BeginProperty(position, label, property);
      Enum enumNew = EditorGUI.EnumFlagsField(position, propName, targetEnum);
      property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
      EditorGUI.EndProperty();
    }
  }
#endif
}
