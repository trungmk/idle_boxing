using UnityEditor;
using UnityEngine;

public class DotProductEditor : EditorWindow, IUpdateSceneGUI
{
    public Vector3 P0;
    public Vector3 P1;
    public Vector3 C;

    private SerializedObject _serializedObject;
    private SerializedProperty _propP0;
    private SerializedProperty _propP1;
    private SerializedProperty _propC;
    private GUIStyle _guiStyle = new GUIStyle();

    [MenuItem("Tools/Dot Product")]
    public static void ShowWindow()
    {
        DotProductEditor dotProductEditor = GetWindow<DotProductEditor>("Dot Product Editor", true);
        dotProductEditor.Show();

    }
    private void OnEnable()
    {
        // Initialize default values for P0, P1, and C if they are not set
        if (P0 == Vector3.zero && P1 == Vector3.zero)
        {
            P0 = new Vector3(0.0f, 1.0f, 0.0f);
            P1 = new Vector3(0.5f, 0.5f, 0.0f);
            C = Vector3.zero;
        }

        _serializedObject = new SerializedObject(this);
        _propP0 = _serializedObject.FindProperty("P0");
        _propP1 = _serializedObject.FindProperty("P1");
        _propC = _serializedObject.FindProperty("C");

        _guiStyle.fontSize = 15;
        _guiStyle.normal.textColor = Color.white;
        _guiStyle.alignment = TextAnchor.MiddleCenter;
        _guiStyle.fontStyle = FontStyle.Bold;

        SceneView.duringSceneGui += SceneGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= SceneGUI;
    }

    private void OnGUI()
    {
        if (_serializedObject == null)
        {
            return;
        }

        _serializedObject.Update();

        EditorCommonUtility.DrawBlockGUI("P0", _propP0);
        EditorCommonUtility.DrawBlockGUI("P1", _propP1);
        EditorCommonUtility.DrawBlockGUI("C", _propC);

        if (_serializedObject.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }
    }

    public void SceneGUI(SceneView view)
    {
        Handles.color = Color.red;
        Vector3 p0 = SetMovePoint(P0);
        Handles.color = Color.green;
        Vector3 p1 = SetMovePoint(P1);
        Handles.color = Color.white;
        Vector3 c = SetMovePoint(C);

        if (p0 != P0 || p1 != P1 || c != C)
        {
            Repaint();
            P0 = p0;
            P1 = p1;
            C = c;
        }
        
        DrawLabel(p0, p1, c);
    }

    private Vector3 SetMovePoint(Vector3 pos)
    {
        float size = HandleUtility.GetHandleSize(pos) * 0.15f;
        return Handles.FreeMoveHandle(pos, size, Vector3.zero, Handles.SphereHandleCap);
    }

    private void DrawLabel(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Handles.Label(c, DotProduct(p0, p1, c).ToString("F1"), _guiStyle);
        Handles.color = Color.black;

        Vector3 cLeft = WorldRotation(p0, c, new Vector3(0f, 1f, 0.0f));
        Vector3 cRight = WorldRotation(p0, c, new Vector3(0f, -1f, 0.0f));

        Handles.DrawAAPolyLine(3.0f, p0, c);
        Handles.DrawAAPolyLine(3.0f, p1, c);
        Handles.DrawAAPolyLine(3.0f, c, cLeft);
        Handles.DrawAAPolyLine(3.0f, c, cRight);

        //Handles.DrawDottedLine(p0, c, 3);
        //Handles.DrawDottedLine(p1, c, 3);
    }

    private float DotProduct(Vector3 p0, Vector3 p1, Vector3 c)
    {
        Vector3 v0 = (p0 - c).normalized;
        Vector3 v1 = (p1 - c).normalized;
        return (v0.x * v1.x + v0.y * v1.y + v0.z * v1.z);   
    }

    private Vector3 WorldRotation(Vector3 p, Vector3 c, Vector3 pos)
    {
        Vector2 dir = (p - c).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        return c + (rotation * pos);
    }
}