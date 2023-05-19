using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainMesh))]
public class TerrainMeshEditor : Editor
{
    private TerrainMesh _target;

    void OnEnable()
    {
        _target = (TerrainMesh)target;
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (_target.colorSettings != null)
        {
            CreateEditor(_target.cellGeometrySettings).OnInspectorGUI();
            CreateEditor(_target.colorSettings).OnInspectorGUI();
            CreateEditor(_target.elevationSettings).OnInspectorGUI();
        }
        if (EditorGUI.EndChangeCheck())
        {
            _target.OnInspectorChange();
        }

        if (GUILayout.Button("Regenerate Mesh"))
        {
            _target.GenerateMesh();
        }
    }
}