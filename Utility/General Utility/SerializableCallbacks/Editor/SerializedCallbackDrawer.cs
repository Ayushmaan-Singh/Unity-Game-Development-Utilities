using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Object = UnityEngine.Object;

namespace Astek.SerializableMethods.Editor
{
    [CustomPropertyDrawer(typeof(SerializedCallback<>), true)]
    public class SerializedCallbackDrawerUI : PropertyDrawer
    {
        // ─────────────────────────────────────────────────────────────────────
        //  Entry point
        // ─────────────────────────────────────────────────────────────────────
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            // ── Outer box that groups everything for this one callback field ──
            // This is the key visual change: every SerializedCallback gets its own
            // bordered box so multiple callbacks in the same component never bleed
            // into each other visually.
            Box box = new Box();
            box.style.paddingTop    = 4;
            box.style.paddingBottom = 4;
            box.style.paddingLeft   = 6;
            box.style.paddingRight  = 6;
            box.style.marginTop     = 2;
            box.style.marginBottom  = 2;

            // ── Serialized property handles ──
            SerializedProperty targetProp = property.FindPropertyRelative("targetObject");
            SerializedProperty methodProp = property.FindPropertyRelative("methodName");
            SerializedProperty parametersProp = property.FindPropertyRelative("parameters");

            // ── Header row: [ObjectField grows+shrinks] [+] [-] ──
            //
            // Layout contract:
            //   • The two buttons have a fixed 22px width and flexShrink=0, so
            //     they are ALWAYS allocated their space first, before anything else.
            //   • The ObjectField gets all remaining space (flexGrow=1). It can
            //     also compress (flexShrink=1, minWidth=0) so a long object name
            //     gets clipped rather than shoving the buttons out of view.
            //   • overflow=Hidden on the row prevents any child from punching
            //     outside the box boundary.
            VisualElement headerRow = new VisualElement();
            headerRow.style.flexDirection = FlexDirection.Row;
            headerRow.style.alignItems    = Align.Center;
            headerRow.style.marginBottom  = 2;
            headerRow.style.overflow      = Overflow.Hidden;

            ObjectField targetField = new ObjectField(property.displayName)
            {
                objectType  = typeof(Object),
                bindingPath = targetProp.propertyPath
            };
            // flexGrow=1   → take all space left after buttons are reserved
            // flexShrink=1 → allowed to shrink when the row is narrow
            // minWidth=0   → overrides UIElements' default that blocks shrinking
            // overflow=Hidden → long names clip instead of expanding the element
            targetField.style.flexGrow   = 1;
            targetField.style.flexShrink = 1;
            targetField.style.minWidth   = 0;
            targetField.style.overflow   = Overflow.Hidden;

            // ── + button: opens method picker ──
            Button addBtn = new Button { text = "+" };
            StyleButton(addBtn);

            // ── - button: removes selected method + clears parameters ──
            Button removeBtn = new Button { text = "−" }; // Unicode minus, looks cleaner than ASCII
            StyleButton(removeBtn);

            headerRow.Add(targetField);
            headerRow.Add(addBtn);
            headerRow.Add(removeBtn);
            box.Add(headerRow);

            // ── Method name label (shown when a method is selected) ──
            // Keeps the inspector readable at a glance without a separate button.
            Label methodLabel = new Label();
            methodLabel.style.unityFontStyleAndWeight = FontStyle.Italic;
            methodLabel.style.color                   = new StyleColor(new Color(0.6f, 0.85f, 1f)); // soft blue
            methodLabel.style.marginLeft              = 2;
            methodLabel.style.marginBottom            = 2;
            UpdateMethodLabel(methodLabel, methodProp.stringValue);
            box.Add(methodLabel);

            // ── Parameter fields container ──
            VisualElement parametersContainer = new VisualElement();
            box.Add(parametersContainer);
            UpdateParameters(parametersProp, parametersContainer);

            // ─────────────────────────────────────────────────────────────────
            //  Button state helpers
            //  Rules:
            //    + is DISABLED when targetObject is null (nothing to pick from)
            //    - is DISABLED when methodName is empty (nothing to remove)
            // ─────────────────────────────────────────────────────────────────
            void RefreshButtonStates()
            {
                bool hasTarget = targetProp.objectReferenceValue != null;
                bool hasMethod = !string.IsNullOrEmpty(methodProp.stringValue);

                addBtn.SetEnabled(hasTarget);
                removeBtn.SetEnabled(hasMethod);
            }

            RefreshButtonStates();

            // ─────────────────────────────────────────────────────────────────
            //  React to target object changes
            //  We track the ObjectField's value-changed event so the + button
            //  immediately enables/disables as the user drags a reference in.
            // ─────────────────────────────────────────────────────────────────
            targetField.RegisterValueChangedCallback(_ => RefreshButtonStates());

            // ─────────────────────────────────────────────────────────────────
            //  Detect external resets (Unity "Reset" context menu on component)
            //
            //  Unity's Reset zeroes the serialized data but never touches our
            //  VisualElements, so the UI would show stale fields.
            //  TrackPropertyValue polls each frame while the inspector is open
            //  and fires the callback the moment a property's serialized value
            //  differs from what it was last frame — catching Reset, Undo, and
            //  any other external write in one place.
            //
            //  • targetProp reset  → null ref  → clear target field + full UI
            //  • methodProp reset  → ""        → clear method label + param fields
            // ─────────────────────────────────────────────────────────────────
            box.TrackPropertyValue(targetProp, prop =>
            {
                // Target was externally cleared (Reset / Undo)
                if (prop.objectReferenceValue == null)
                {
                    // Also wipe the method + parameters so nothing is orphaned
                    SerializedObject so = property.serializedObject;
                    so.Update();
                    methodProp.stringValue   = string.Empty;
                    parametersProp.arraySize = 0;
                    so.ApplyModifiedProperties();

                    parametersContainer.Clear();
                    UpdateMethodLabel(methodLabel, string.Empty);
                }
                RefreshButtonStates();
            });

            box.TrackPropertyValue(methodProp, prop =>
            {
                // Method was externally cleared (Reset / Undo after method select)
                if (string.IsNullOrEmpty(prop.stringValue))
                {
                    SerializedObject so = property.serializedObject;
                    so.Update();
                    parametersProp.arraySize = 0;
                    so.ApplyModifiedProperties();

                    parametersContainer.Clear();
                    UpdateMethodLabel(methodLabel, string.Empty);
                    RefreshButtonStates();
                }
            });

            // ─────────────────────────────────────────────────────────────────
            //  + button: show method picker dropdown
            // ─────────────────────────────────────────────────────────────────
            addBtn.clicked += () =>
            {
                ShowMethodDropdown(
                    targetProp.objectReferenceValue,
                    methodProp,
                    parametersProp,
                    property,
                    methodLabel,
                    parametersContainer,
                    RefreshButtonStates
                );
            };

            // ─────────────────────────────────────────────────────────────────
            //  - button: clear method + parameters
            // ─────────────────────────────────────────────────────────────────
            removeBtn.clicked += () =>
            {
                SerializedObject so = property.serializedObject;
                so.Update();

                methodProp.stringValue   = string.Empty;
                parametersProp.arraySize = 0;

                so.ApplyModifiedProperties();

                // Rebuild UI
                parametersContainer.Clear();
                UpdateMethodLabel(methodLabel, string.Empty);
                RefreshButtonStates();
            };

            property.serializedObject.ApplyModifiedProperties();
            return box;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Shared button styling
        //  Small square icon-style buttons that sit beside the target field.
        // ─────────────────────────────────────────────────────────────────────
        private static void StyleButton(Button btn)
        {
            btn.style.width      = 22;
            btn.style.height     = 22;
            btn.style.marginLeft = 2;
            btn.style.fontSize   = 14;
            // Remove default large padding so the icon stays centered
            btn.style.paddingTop    = 0;
            btn.style.paddingBottom = 0;
            btn.style.paddingLeft   = 0;
            btn.style.paddingRight  = 0;
            // Critical: prevent flexbox from ever compressing the button.
            // flexShrink=0 guarantees the button keeps its 22px no matter how
            // narrow the row gets due to a long object name in the ObjectField.
            // flexGrow=0 stops it from accidentally expanding either.
            btn.style.flexShrink = 0;
            btn.style.flexGrow   = 0;
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Method label helper
        // ─────────────────────────────────────────────────────────────────────
        private static void UpdateMethodLabel(Label label, string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                label.text        = "No method selected";
                label.style.color = new StyleColor(new Color(0.55f, 0.55f, 0.55f));
            }
            else
            {
                label.text        = $"⮕  {methodName}";
                label.style.color = new StyleColor(new Color(0.6f, 0.85f, 1f));
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Method dropdown
        // ─────────────────────────────────────────────────────────────────────
        private void ShowMethodDropdown(
            Object target,
            SerializedProperty methodProp,
            SerializedProperty parametersProp,
            SerializedProperty property,
            Label methodLabel,
            VisualElement parametersContainer,
            Action refreshButtonStates)
        {
            if (target == null) return;

            // Determine the generic return type this callback expects
            Type callbackType = fieldInfo.FieldType;
            if (!callbackType.IsGenericType) return;

            Type genericReturnType = callbackType.GetGenericArguments()[0];
            Type targetType = target.GetType();

            MethodInfo[] methods = targetType
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.ReturnType == genericReturnType)
                .ToArray();

            GenericMenu menu = new GenericMenu();

            if (methods.Length == 0)
            {
                menu.AddDisabledItem(new GUIContent("No matching methods found"));
            }
            else
            {
                foreach (MethodInfo method in methods)
                {
                    // Capture for closure
                    MethodInfo capturedMethod = method;

                    menu.AddItem(
                        new GUIContent(capturedMethod.Name),
                        methodProp.stringValue == capturedMethod.Name, // checkmark on current
                        () => SelectMethod(
                            capturedMethod,
                            methodProp,
                            parametersProp,
                            property,
                            methodLabel,
                            parametersContainer,
                            refreshButtonStates
                        )
                    );
                }
            }

            menu.ShowAsContext();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Apply selected method to serialized data and refresh UI
        // ─────────────────────────────────────────────────────────────────────
        private static void SelectMethod(
            MethodInfo method,
            SerializedProperty methodProp,
            SerializedProperty parametersProp,
            SerializedProperty property,
            Label methodLabel,
            VisualElement parametersContainer,
            Action refreshButtonStates)
        {
            SerializedObject so = property.serializedObject;
            so.Update();

            methodProp.stringValue = method.Name;

            ParameterInfo[] parameters = method.GetParameters();
            parametersProp.arraySize = parameters.Length;

            for (int i = 0; i < parameters.Length; i++)
            {
                SerializedProperty paramProp = parametersProp.GetArrayElementAtIndex(i);
                SerializedProperty typeProp = paramProp.FindPropertyRelative("Type");
                typeProp.enumValueIndex = (int)AnyValue.ValueTypeOf(parameters[i].ParameterType);
            }

            so.ApplyModifiedProperties();

            // Refresh UI
            parametersContainer.Clear();
            UpdateParameters(parametersProp, parametersContainer);
            UpdateMethodLabel(methodLabel, method.Name);
            refreshButtonStates();
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Build parameter fields from the serialized array
        //  Each field reads/writes its specific serialized sub-property directly,
        //  with a fresh serializedObject.Update() → Assign → ApplyModifiedProperties
        //  cycle to avoid stale references.
        // ─────────────────────────────────────────────────────────────────────
        private static void UpdateParameters(SerializedProperty parametersProp, VisualElement container)
        {
            if (!parametersProp.isArray) return;

            for (int i = 0; i < parametersProp.arraySize; i++)
            {
                int capturedIndex = i; // closure-safe index
                ValueType paramType = (ValueType)parametersProp
                    .GetArrayElementAtIndex(capturedIndex)
                    .FindPropertyRelative("Type")
                    .enumValueIndex;

                VisualElement field = BuildField(parametersProp, capturedIndex, paramType);
                container.Add(field);
            }
        }

        // ─────────────────────────────────────────────────────────────────────
        //  Factory: one field per supported ValueType
        //  Pattern: create → set initial value → register change callback
        //  The callback always calls so.Update() first so it works correctly
        //  after domain reloads or Undo operations.
        // ─────────────────────────────────────────────────────────────────────
        private static VisualElement BuildField(SerializedProperty paramsProp, int index, ValueType paramType)
        {
            // Helper: freshly resolved property for this index + sub-field name
            SerializedProperty Fresh(string subField) =>
                paramsProp.GetArrayElementAtIndex(index).FindPropertyRelative(subField);

            // Helper: update → assign → apply cycle
            void Apply(SerializedObject so, Action assign)
            {
                so.Update();
                assign();
                so.ApplyModifiedProperties();
            }

            switch (paramType)
            {
                case ValueType.Int:
                {
                    IntegerField f = new($"Parameter {index + 1} (Int)")
                        { value = Fresh("IntValue").intValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("IntValue").intValue = evt.newValue));
                    return f;
                }

                case ValueType.Float:
                {
                    FloatField f = new($"Parameter {index + 1} (Float)")
                        { value = Fresh("FloatValue").floatValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("FloatValue").floatValue = evt.newValue));
                    return f;
                }

                case ValueType.String:
                {
                    TextField f = new($"Parameter {index + 1} (String)")
                        { value = Fresh("StringValue").stringValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("StringValue").stringValue = evt.newValue));
                    return f;
                }

                case ValueType.Bool:
                {
                    Toggle f = new($"Parameter {index + 1} (Bool)")
                        { value = Fresh("BoolValue").boolValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("BoolValue").boolValue = evt.newValue));
                    return f;
                }

                case ValueType.Vector2:
                {
                    Vector2Field f = new($"Parameter {index + 1} (Vector2)")
                        { value = Fresh("Vector2Value").vector2Value };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("Vector2Value").vector2Value = evt.newValue));
                    return f;
                }

                case ValueType.Vector2Int:
                {
                    Vector2IntField f = new($"Parameter {index + 1} (Vector2Int)")
                        { value = Fresh("Vector2IntValue").vector2IntValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("Vector2IntValue").vector2IntValue = evt.newValue));
                    return f;
                }

                case ValueType.Vector3:
                {
                    Vector3Field f = new($"Parameter {index + 1} (Vector3)")
                        { value = Fresh("Vector3Value").vector3Value };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("Vector3Value").vector3Value = evt.newValue));
                    return f;
                }

                case ValueType.Vector3Int:
                {
                    Vector3IntField f = new($"Parameter {index + 1} (Vector3Int)")
                        { value = Fresh("Vector3IntValue").vector3IntValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("Vector3IntValue").vector3IntValue = evt.newValue));
                    return f;
                }

                case ValueType.Vector4:
                {
                    Vector4Field f = new($"Parameter {index + 1} (Vector4)")
                        { value = Fresh("Vector4Value").vector4Value };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("Vector4Value").vector4Value = evt.newValue));
                    return f;
                }

                case ValueType.Color:
                {
                    ColorField f = new($"Parameter {index + 1} (Color)")
                        { value = Fresh("ColorValue").colorValue };
                    f.RegisterValueChangedCallback(evt =>
                        Apply(paramsProp.serializedObject, () => Fresh("ColorValue").colorValue = evt.newValue));
                    return f;
                }

                default:
                    return new Label($"Parameter {index + 1}: Unsupported Type");
            }
        }
    }
}