using UnityEditor;
using UnityEngine;

public class CrossProductEditor : EditorWindow, IUpdateSceneGUI
{
    public Vector3 P;
    public Vector3 Q;
    public Vector3 PxQ;

    private SerializedObject _serializedObject;
    private SerializedProperty _propP;
    private SerializedProperty _propQ;
    private SerializedProperty _propPxQ;
    private GUIStyle _guiStyle;

    [MenuItem("Tools/Cross Product")]
    public static void ShowWindow()
    {
        GetWindow<CrossProductEditor>("Cross Product Editor", true);
    }

    private void OnEnable()
    {
        if (P == Vector3.zero && Q == Vector3.zero)
        {
            SetDefaultValues();
        }

        _serializedObject = new SerializedObject(this);
        _propP = _serializedObject.FindProperty("P");
        _propQ = _serializedObject.FindProperty("Q");
        _propPxQ = _serializedObject.FindProperty("PxQ");

        // Setup style
        _guiStyle = new GUIStyle
        {
            fontSize = 15,
            normal = { textColor = Color.white },
            alignment = TextAnchor.MiddleCenter,
            fontStyle = FontStyle.Bold
        };

        // Handle scene GUI Event
        SceneView.duringSceneGui += SceneGUI;
        Undo.undoRedoPerformed += RepaintOnGUI;
    }

    private void OnDisable()
    {
        SceneView.duringSceneGui -= SceneGUI;
        Undo.undoRedoPerformed -= RepaintOnGUI;
    }

    private void OnGUI()
    {
        _serializedObject.Update();

        EditorCommonUtility.DrawBlockGUI("P", _propP);
        EditorCommonUtility.DrawBlockGUI("Q", _propQ);
        EditorCommonUtility.DrawBlockGUI("PxQ", _propPxQ);

        if (_serializedObject.ApplyModifiedProperties())
        {
            SceneView.RepaintAll();
        }

        if (GUILayout.Button("Reset Values"))
        {
            SetDefaultValues();
        }
    }

    private void SetDefaultValues()
    {
        P = new Vector3(0f, 1, 0f);
        Q = new Vector3(1, 0, 0);
        PxQ = CrossProduct(P, Q);

        if (_serializedObject != null && _serializedObject.ApplyModifiedProperties())
        {
            _serializedObject.Update();
            RepaintOnGUI();
        }
    }

    public void SceneGUI(SceneView view)
    {
        Vector3 p = Handles.PositionHandle(P, Quaternion.identity);
        Vector3 q = Handles.PositionHandle(Q, Quaternion.identity);

        Handles.color = Color.blue;
        Vector3 pxq = CrossProduct(p, q);
        Handles.DrawSolidDisc(pxq, Vector3.forward, 0.1f);

        if (p != P || q != Q)
        {
            Undo.RecordObject(this, "Vector Move");
            RepaintOnGUI();
            P = p;
            Q = q;
            PxQ = pxq;
        }

        DrawLineGUI(p, "P", Color.green);
        DrawLineGUI(q, "Q", Color.red);
        DrawLineGUI(pxq, "PxQ", Color.blue);
    }

    private void DrawLineGUI(Vector3 pos, string text, Color col)
    {
        Handles.color = col;
        Handles.Label(pos, text, _guiStyle);
        Handles.DrawAAPolyLine(3.0f, pos, Vector3.zero);
    }

    private void RepaintOnGUI()
    {
        Repaint();

        //Mathfs.AngleBetween
    }

    private Vector3 CrossProduct(Vector3 p, Vector3 q)
    {
        //float x = p.y * q.z - p.z * q.y;
        //float y = p.y * q.x - p.x * q.z;
        //float z = p.x * q.y - p.y * q.x;

        //return new Vector3(x, y, z);

        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = 0;
        m[0, 1] = q.z;
        m[0, 2] = -q.y;
        m[1, 0] = -q.z;
        m[1, 1] = 0;
        m[1, 2] = q.x;
        m[2, 0] = q.y;
        m[2, 1] = -q.x;
        m[2, 2] = 0;
        return m * p;
    }
}
