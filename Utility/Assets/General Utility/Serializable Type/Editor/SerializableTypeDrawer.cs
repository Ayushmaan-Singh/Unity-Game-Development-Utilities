using System;
using UnityEditor;
using UnityEngine;

namespace Astek.Editor
{
    [CustomPropertyDrawer(typeof(SerializableType))]
    public class SerializableTypeDrawer : PropertyDrawer
    {
        private TypeFilterAttribute typeFilter;
        string[] typeNames, typeFullNames;

        private void Initialize()
        {
            if (typeFullNames != null) return;
            typeFilter = (TypeFilterAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TypeFilterAttribute));

            Type[] filteredTypes = AppDomain.CurrentDomain.GetAssemblies()
                                            .SelectMany(assembly => assembly.GetTypes())
                                            .Where(t => typeFilter == null ? DefaultFilter(t) : typeFilter.Filter(t))
                                            .ToArray();

            typeNames = filteredTypes.Select(t => t.ReflectedType == null ? t.Name : $"t.ReflectedType.Name + t.Name").ToArray();
            typeFullNames = filteredTypes.Select(t => t.AssemblyQualifiedName).ToArray();
        }

        private static bool DefaultFilter(Type type) =>
            !type.IsAbstract && !type.IsInterface && !type.IsGenericType;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            Initialize();
            SerializedProperty typeIdProperty = property.FindPropertyRelative("assemblyQualifiedName");

            if (string.IsNullOrEmpty(typeIdProperty.stringValue))
            {
                typeIdProperty.stringValue = typeFullNames.FirstOrDefault();
                property.serializedObject.ApplyModifiedProperties();
            }

            int currentIndex = typeFullNames.FindIndex(t => t == typeIdProperty.stringValue);
            int selectedIndex = EditorGUI.Popup(position, label.text, currentIndex, typeFullNames);

            if (selectedIndex >= 0 && selectedIndex != currentIndex)
            {
                typeIdProperty.stringValue = typeFullNames[selectedIndex];
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}