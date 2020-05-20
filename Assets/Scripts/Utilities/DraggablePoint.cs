using UnityEngine;
#if UNITY_EDITOR
using System.Reflection;
using UnityEditor;
#endif

namespace Lau_Utilities
{
  public class DraggablePoint : PropertyAttribute
  {
    public DraggablePoint() {}
  }


  #if UNITY_EDITOR
  [CustomEditor(typeof(MonoBehaviour), true, isFallback = true)]
  public class DraggablePointDrawer : Homebrew.EditorOverride
  {

    readonly GUIStyle boldStyle = new GUIStyle();

    new void OnEnable()
    {
      base.OnEnable();
      boldStyle.fontStyle = FontStyle.Bold;
      boldStyle.normal.textColor = Color.white;
    }

    public void OnSceneGUI()
    {
      SerializedObject serializedObject = new UnityEditor.SerializedObject(target);

      var property = serializedObject.GetIterator();
      while (property.Next(true))
      {
        var bindFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
        var field = target.GetType().GetField(property.name, bindFlags);
        if (field == null) { continue; }

        var draggablePoint = field.GetCustomAttribute(typeof(DraggablePoint), false);
        if (draggablePoint == null) { continue; }

        switch (property.propertyType)
        {
          case SerializedPropertyType.Vector2:
            Handles.Label(property.vector2Value, property.displayName);
            property.vector2Value = Handles.PositionHandle(property.vector2Value, Quaternion.identity);
            serializedObject.ApplyModifiedProperties();
          break;
          case SerializedPropertyType.Vector3:
            Handles.Label(property.vector3Value, property.displayName);
            property.vector3Value = Handles.PositionHandle(property.vector3Value, Quaternion.identity);
            serializedObject.ApplyModifiedProperties();
          break;
          default:
          if (field.FieldType.IsArray)
            {
              switch (property.arrayElementType)
              {
                case "Vector3":
                  foreach (var child in property.GetChildren())
                  {
                    if(child.propertyType !=  SerializedPropertyType.Vector3) { continue; }

                    Handles.Label(child.vector3Value, property.displayName + ":" + child.displayName);
                    child.vector3Value = Handles.PositionHandle(child.vector3Value, Quaternion.identity);
                    serializedObject.ApplyModifiedProperties();
                  }
                break;
                case "Vector2":
                  foreach (var child in property.GetChildren())
                  {
                    if(child.propertyType !=  SerializedPropertyType.Vector2) { continue; }

                    Handles.Label(child.vector2Value, property.displayName + ":" + child.displayName);
                    child.vector2Value = Handles.PositionHandle(child.vector2Value, Quaternion.identity);
                    serializedObject.ApplyModifiedProperties();
                  }
                break;
              }
            }
          break;
        }

      }
    }
  }
  #endif
}
