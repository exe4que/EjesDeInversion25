using UnityEditor;
using UnityEngine;
using EjesDeInversion.Data;

namespace EjesDeInversion.Editor
{
    [CustomEditor(typeof(PointersData))]
    public class PointersDataEditor : UnityEditor.Editor
    {
        private PointersData _data;
        private SerializedProperty _pointersProp;
        private GUIStyle _labelStyle;

        void OnEnable()
        {
            _data = (PointersData)target;
            _pointersProp = serializedObject.FindProperty("Pointers");
            SceneView.duringSceneGui += OnSceneGUI;
            _labelStyle = new GUIStyle(EditorStyles.label);
            _labelStyle.normal.textColor = Color.black;
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.PropertyField(_pointersProp, true);
            if (GUILayout.Button("Repaint Scene")) SceneView.RepaintAll();
            serializedObject.ApplyModifiedProperties();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            if (_data == null || _data.Pointers == null) return;

            Handles.color = Color.yellow;
            for (int i = 0; i < _data.Pointers.Length; i++)
            {
                var p = _data.Pointers[i];
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = Handles.PositionHandle(p.Position, Quaternion.identity);
                
                //do raycast to ground plane
                Ray ray = new Ray(newPos, Vector3.forward);
                if (Physics.Raycast(ray, out RaycastHit hit, 10000f))
                {
                    newPos = hit.point;
                }
                
                Vector3 labelOffset = new Vector3(0.5f, 2f, 0);
                Handles.Label(newPos + labelOffset, p.Name, _labelStyle);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(_data, "Move Pointer");
                    _data.Pointers[i].Position = newPos;
                    EditorUtility.SetDirty(_data);
                }
            }
        }
    }
}