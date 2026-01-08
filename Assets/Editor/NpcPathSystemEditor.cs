using Main;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

[CustomEditor(typeof(NpcPathSystem))]
public class NpcPathSystemEditor : UnityEditor.Editor
{
    private NpcPathSystem pathSystem;

    private void OnEnable()
    { 
        pathSystem = (NpcPathSystem)target;
    }

    public override void OnInspectorGUI()
    { 
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Path editor", EditorStyles.boldLabel);

        if (GUILayout.Button("Add path"))
        {
            pathSystem.paths.Add(new NpcPath());
        }

        for (int i = 0; i < pathSystem.paths.Count; i++)
        { 
            var path = pathSystem.paths[i];
            EditorGUILayout.BeginVertical("box");

            path.pathName = EditorGUILayout.TextField("Name", path.pathName);

            if (GUILayout.Button("Add path point"))
            {
                Transform pathParent = pathSystem.transform.Find(path.pathName);
                if (pathParent == null)
                { 
                    GameObject parentObj = new GameObject(path.pathName);
                    parentObj.transform.SetParent(pathSystem.transform);
                    parentObj.transform.localPosition = Vector3.zero;
                    pathParent = parentObj.transform;
                }

                GameObject point = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                point.name = $"Point_{path.pathPoints.Count}";
                point.transform.localScale = new Vector3(0.25f, 0.25f, 0.25f);
                point.transform.SetParent(pathParent);
                point.transform.localPosition = Vector3.zero;
                path.pathPoints.Add(new PathPoint(point.transform));
            }

            if (GUILayout.Button("Remove last point"))
            {
                Transform lastPoint = path.pathPoints[path.pathPoints.Count - 1].transform;
                if(lastPoint != null)
                    DestroyImmediate(lastPoint.gameObject);
                path.pathPoints.RemoveAt(path.pathPoints.Count - 1);
            }

            if (GUILayout.Button("Remove path"))
            {
                Transform pathParent = pathSystem.transform.Find(path.pathName);
                if(pathParent != null)
                    DestroyImmediate(pathParent.gameObject);
                pathSystem.paths.RemoveAt(i);
                break;
            }

            EditorGUILayout.EndVertical();
        }

        if (GUI.changed)
        { 
            EditorUtility.SetDirty(pathSystem);
        }
    }

    private void OnSceneGUI()
    {
        Handles.color = Color.yellow;
        foreach (var path in pathSystem.paths)
        {
            for (int i = 0; i < path.pathPoints.Count - 1; i++)
            {
                if (path.pathPoints[i] != null && path.pathPoints[i + 1] != null)
                {
                    Handles.DrawLine(path.pathPoints[i].transform.position, path.pathPoints[i + 1].transform.position);
                }
            }
        }
    }
}