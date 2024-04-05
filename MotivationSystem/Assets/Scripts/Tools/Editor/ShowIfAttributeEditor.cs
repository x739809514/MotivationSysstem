using System.Reflection;
using AnimSystem.Core;
using UnityEditor;
using UnityEngine;

namespace Tools.Editor
{
    [CustomEditor(typeof(MonoBehaviour), true), CanEditMultipleObjects]
    public class ShowIfAttributeEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // 使用反射找到对应的字段
                FieldInfo fieldInfo = target.GetType().GetField(iterator.name,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (fieldInfo != null)
                {
                    ShowIfAttribute showIf = fieldInfo.GetCustomAttribute<ShowIfAttribute>();

                    bool showProperty = true;
                    if (showIf != null)
                    {
                        SerializedProperty conditionProperty = serializedObject.FindProperty(showIf.ConditionFieldName);
                        if (conditionProperty != null && conditionProperty.propertyType == SerializedPropertyType.Enum)
                        {
                            // 获取枚举值并比较
                            object currentEnumValue = conditionProperty.enumNames[conditionProperty.enumValueIndex];
                            if (currentEnumValue.ToString() != showIf.ShowIfConditionIsTrue.ToString())
                            {
                                showProperty = false;
                            }
                        }
                        else
                        {
                            Debug.LogWarning(
                                $"Error: Cannot find an enum condition field '{showIf.ConditionFieldName}' specified in ShowIf attribute in {target.GetType()}");
                        }
                    }

                    if (showProperty)
                    {
                        EditorGUILayout.PropertyField(iterator, true);
                    }
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}