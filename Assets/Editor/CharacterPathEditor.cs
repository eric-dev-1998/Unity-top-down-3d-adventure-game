using Assets.Scripts.World.Npc;
using Assets.Scripts.Systems.Character_Path;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(PathContainer))]
public class CharacterPathEditor : UnityEditor.Editor
{
    private PathContainer container;

    private void OnEnable()
    { 
        container = (PathContainer)target;
    }

    public override void OnInspectorGUI()
    { 
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Path editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Add path"))
        {
            container.paths.Add(new Path());
        }

        for (int i = 0; i < container.paths.Count; i++)
        { 
            var path = container.paths[i];
            EditorGUILayout.BeginVertical("box");

            path.name = EditorGUILayout.TextField("Name", path.name);

            if (GUILayout.Button("Add path point"))
            {
                Transform pathParent = container.transform.Find(path.name);
                if (pathParent == null)
                { 
                    GameObject parentObj = new GameObject(path.name);
                    parentObj.transform.SetParent(container.transform);
                    parentObj.transform.localPosition = Vector3.zero;
                    pathParent = parentObj.transform;
                }

                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.name = $"Point_{path.points.Count}";
                point.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                point.transform.SetParent(pathParent);
                point.transform.localPosition = Vector3.zero;
                path.points.Add(new Point(point.transform));
            }

            if (GUILayout.Button("Remove last point"))
            {
                Transform lastPoint = path.points[path.points.Count - 1].transform;
                if(lastPoint != null)
                    DestroyImmediate(lastPoint.gameObject);
                path.points.RemoveAt(path.points.Count - 1);
            }

            if (GUILayout.Button("Remove path"))
            {
                Transform pathParent = container.transform.Find(path.name);
                if(pathParent != null)
                    DestroyImmediate(pathParent.gameObject);
                container.paths.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUI.changed)
        { 
            EditorUtility.SetDirty(container);
        }
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.yellow;
        foreach (var path in container.paths)
        {
            for (int i = 0; i < path.points.Count - 1; i++)
            {
                if (path.points[i] != null && path.points[i + 1] != null)
                {
                    Handles.DrawLine(path.points[i].transform.position, path.points[i + 1].transform.position);
                }
            }
        }
    }
}