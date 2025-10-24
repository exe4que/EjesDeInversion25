using UnityEditor;
using UnityEngine;
using EjesDeInversion.Data;

namespace EjesDeInversion.Editor
{
    [CustomEditor(typeof(PointersData))]
    public class PointersDataEditor : UnityEditor.Editor
    {
        PointersData data;
        SerializedProperty pointersProp;

        void OnEnable()
        {
            data = (PointersData)target;
            pointersProp = serializedObject.FindProperty("Pointers");
            SceneView.duringSceneGui += OnSceneGUI;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(pointersProp, true);
            if (GUILayout.Button("Repaint Scene")) SceneView.RepaintAll();
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (data == null || data.Pointers == null) return;

            Handles.color = Color.yellow;
            for (int i = 0; i < data.Pointers.Length; i++)
            {
                var p = data.Pointers[i];
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(p.Position, Quaternion.identity);
                
                //do raycast to ground plane
                Ray ray = new Ray(newPos + Vector3.up * 10f, Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, 10000f))
                {
                    newPos = hit.point;
                }
                
                Handles.Label(newPos + Vector3.up * 0.2f, p.Name);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(data, "Move Pointer");
                    data.Pointers[i].Position = newPos;
                    EditorUtility.SetDirty(data);
                }
            }
        }
    }
}