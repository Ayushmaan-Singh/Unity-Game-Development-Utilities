#if UNITY_EDITOR
// ═══════════════════════════════════════════════════════════════════════════
//  SerializedCallbackExamples.cs
//  Attach this MonoBehaviour to any GameObject to explore every feature of
//  the SerializedCallback asset.
//
//  HOW TO USE
//  ──────────
//  1. Attach this script to a GameObject in your scene.
//  2. In the Inspector you will see one callback field per section.
//  3. Drag THIS same GameObject into the "Target" slot of each field.
//  4. Click the [+] button to open the method picker and choose the method
//     whose name matches the section (e.g. "Returns_Int" for the Int field).
//  5. Set the parameter values you want in the Inspector.
//  6. Enter Play Mode — results print to the Console via Debug.Log.
//
//  TIP: You can also drag a DIFFERENT GameObject (with its own scripts) into
//  the Target slot to call methods on other components — SerializedCallback
//  works on any UnityEngine.Object.
// ═══════════════════════════════════════════════════════════════════════════

using System;
using UnityEngine;
using Astek.SerializableMethods;

public class SerializedCallbackExamples : MonoBehaviour
{
    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 1 — INT
    //  Demonstrates: returning an int, mixing int + Vector3Int parameters.
    //  Inspector setup: Target = this GameObject, Method = Returns_Int
    //  Try: Parameter 1 = 10, Parameter 2 = 5  →  Console prints 15
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Int from the method list. Uses two int parameters.")]
    [SerializeField] private SerializedCallback<int> intCallback;

    /// <summary>Adds two integers. Shown in the [+] method picker as "Returns_Int".</summary>
    public int Returns_Int(int a, int b) => a + b;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 2 — FLOAT
    //  Demonstrates: returning a float, multiplying with a float parameter.
    //  Inspector setup: Target = this GameObject, Method = Returns_Float
    //  Try: Parameter 1 = 3.5, Parameter 2 = 2.0  →  Console prints 7
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Float. Multiplies two floats.")]
    [SerializeField] private SerializedCallback<float> floatCallback;

    /// <summary>Multiplies two floats.</summary>
    public float Returns_Float(float a, float b) => a * b;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 3 — BOOL
    //  Demonstrates: returning a bool by comparing two ints.
    //  Inspector setup: Method = Returns_Bool
    //  Try: Parameter 1 = 5, Parameter 2 = 3  →  Console prints True
    //       Parameter 1 = 3, Parameter 2 = 5  →  Console prints False
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Bool. Returns true when the first int is greater than the second.")]
    [SerializeField] private SerializedCallback<bool> boolCallback;

    /// <summary>Returns true if a > b.</summary>
    public bool Returns_Bool(int a, int b) => a > b;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 4 — STRING
    //  Demonstrates: returning a string built from two string parameters.
    //  Inspector setup: Method = Returns_String
    //  Try: Parameter 1 = "Hello", Parameter 2 = "World"  →  "Hello World"
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_String. Joins two strings with a space.")]
    [SerializeField] private SerializedCallback<string> stringCallback;

    /// <summary>Concatenates two strings.</summary>
    public string Returns_String(string a, string b) => $"{a} {b}";


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 5 — VECTOR2
    //  Demonstrates: float maths producing a Vector2 result.
    //  Inspector setup: Method = Returns_Vector2
    //  Try: scaleX = 2, scaleY = 3  →  (2, 3)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Vector2. Scales a base Vector2 by two floats.")]
    [SerializeField] private SerializedCallback<Vector2> vector2Callback;

    /// <summary>Scales a Vector2 by two independent floats.</summary>
    public Vector2 Returns_Vector2(Vector2 baseVec, float scaleX, float scaleY)
        => new Vector2(baseVec.x * scaleX, baseVec.y * scaleY);


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 6 — VECTOR2INT
    //  Demonstrates: integer grid arithmetic.
    //  Inspector setup: Method = Returns_Vector2Int
    //  Try: base=(3,4), offset=(1,2)  →  (4,6)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Vector2Int. Adds an integer offset to a base grid cell.")]
    [SerializeField] private SerializedCallback<Vector2Int> vector2IntCallback;

    /// <summary>Adds an integer offset to a base Vector2Int (useful for grid/tile logic).</summary>
    public Vector2Int Returns_Vector2Int(Vector2Int baseCell, Vector2Int offset)
        => baseCell + offset;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 7 — VECTOR3
    //  Demonstrates: 3D position calculation mixing Vector3 + float.
    //  Inspector setup: Method = Returns_Vector3
    //  Try: origin=(0,0,0), direction=(0,1,0), distance=5  →  (0,5,0)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Vector3. Projects a point along a direction by a distance.")]
    [SerializeField] private SerializedCallback<Vector3> vector3Callback;

    /// <summary>Moves an origin point along a direction by a set distance.</summary>
    public Vector3 Returns_Vector3(Vector3 origin, Vector3 direction, float distance)
        => origin + direction.normalized * distance;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 8 — VECTOR3INT
    //  Demonstrates: 3D integer voxel offset.
    //  Inspector setup: Method = Returns_Vector3Int
    //  Try: chunk=(2,0,3), localOffset=(1,1,1)  →  (3,1,4)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Vector3Int. Converts chunk + local offset into a world voxel coordinate.")]
    [SerializeField] private SerializedCallback<Vector3Int> vector3IntCallback;

    /// <summary>Converts a chunk coordinate + local offset to a world voxel position.</summary>
    public Vector3Int Returns_Vector3Int(Vector3Int chunkCoord, Vector3Int localOffset, int chunkSize)
        => chunkCoord * chunkSize + localOffset;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 9 — VECTOR4
    //  Demonstrates: raw Vector4 for use cases like shader parameters or
    //  homogeneous coordinates.
    //  Inspector setup: Method = Returns_Vector4
    //  Try: (1,0,0,0) + (0,1,0,0)  →  (1,1,0,0)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Vector4. Adds two Vector4 values (handy for shader parameter blending).")]
    [SerializeField] private SerializedCallback<Vector4> vector4Callback;

    /// <summary>Adds two Vector4 values together.</summary>
    public Vector4 Returns_Vector4(Vector4 a, Vector4 b) => a + b;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 10 — COLOR
    //  Demonstrates: color blending.
    //  Inspector setup: Method = Returns_Color
    //  Try: colorA=red, colorB=blue, t=0.5  →  purple
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Color. Lerps between two colors by a float t (0=colorA, 1=colorB).")]
    [SerializeField] private SerializedCallback<Color> colorCallback;

    /// <summary>Linearly interpolates between two colors.</summary>
    public Color Returns_Color(Color colorA, Color colorB, float t) => Color.Lerp(colorA, colorB, t);


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 11 — NO PARAMETERS
    //  Demonstrates: a callback that takes zero parameters.
    //  The parameter list in the Inspector will be empty — that is correct.
    //  Inspector setup: Method = Returns_NoParameters
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_NoParameters. Shows that zero-parameter methods work fine.")]
    [SerializeField] private SerializedCallback<float> noParamsCallback;

    /// <summary>Returns the current real time since startup — no parameters needed.</summary>
    public float Returns_NoParameters() => Time.realtimeSinceStartup;


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 12 — MIXED PARAMETER TYPES
    //  Demonstrates: a single method whose parameters span several ValueTypes.
    //  Inspector setup: Method = Returns_MixedTypes
    //  Try: label="Score", baseScore=100, multiplier=1.5, bonus=50
    //       →  "Score: 200"  (100 * 1.5 + 50 = 200, cast to int)
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_MixedTypes. Uses string + int + float + int together.")]
    [SerializeField] private SerializedCallback<string> mixedCallback;

    /// <summary>
    /// Formats a score string from a label, base value, float multiplier, and int bonus.
    /// Showcases that parameters can freely mix all supported ValueTypes.
    /// </summary>
    public string Returns_MixedTypes(string label, int baseScore, float multiplier, int bonus)
        => $"{label}: {(int)(baseScore * multiplier) + bonus}";


    // ───────────────────────────────────────────────────────────────────────
    //  SECTION 13 — PRIVATE METHOD
    //  Demonstrates: SerializedCallback can target private methods — they
    //  appear in the [+] picker just like public ones.
    //  Inspector setup: Method = Returns_Private
    // ───────────────────────────────────────────────────────────────────────
    [Tooltip("Pick Returns_Private. Proves private methods are accessible to the picker.")]
    [SerializeField] private SerializedCallback<int> privateCallback;

    /// <summary>Private methods work exactly like public ones.</summary>
    private int Returns_Private(int value) => value * value;


    // ═══════════════════════════════════════════════════════════════════════
    //  RUNTIME — Invoke all callbacks and print results
    //  All invocations happen in Awake so you see output the moment Play
    //  mode starts, before any game logic runs.
    // ═══════════════════════════════════════════════════════════════════════
    private void Awake()
    {
        Debug.Log("──── SerializedCallback Examples ────");

        // Each Invoke() call uses the parameter values set in the Inspector.
        // If a callback has no method selected, the asset logs a warning and
        // returns the default value for TReturn (0, false, null, etc.).

        Debug.Log($"[1]  Int result         : {intCallback.Invoke()}");
        Debug.Log($"[2]  Float result       : {floatCallback.Invoke()}");
        Debug.Log($"[3]  Bool result        : {boolCallback.Invoke()}");
        Debug.Log($"[4]  String result      : {stringCallback.Invoke()}");
        Debug.Log($"[5]  Vector2 result     : {vector2Callback.Invoke()}");
        Debug.Log($"[6]  Vector2Int result  : {vector2IntCallback.Invoke()}");
        Debug.Log($"[7]  Vector3 result     : {vector3Callback.Invoke()}");
        Debug.Log($"[8]  Vector3Int result  : {vector3IntCallback.Invoke()}");
        Debug.Log($"[9]  Vector4 result     : {vector4Callback.Invoke()}");
        Debug.Log($"[10] Color result       : {colorCallback.Invoke()}");
        Debug.Log($"[11] No-params result   : {noParamsCallback.Invoke():F2}s since startup");
        Debug.Log($"[12] Mixed types result : {mixedCallback.Invoke()}");
        Debug.Log($"[13] Private result     : {privateCallback.Invoke()}");

        Debug.Log("─────────────────────────────────────");
    }

    private void OnValidate()
    {
        if (!CompareTag("EditorOnly"))
            tag = "EditorOnly";
    }
}

#endif