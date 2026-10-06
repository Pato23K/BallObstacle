using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class OpenActivityOnce
{
    [InitializeOnLoadMethod]
    static void Queue()
    {
        EditorApplication.delayCall += Open;
    }

    static void Open()
    {
        const string request = "Temp/OpenActivity.request";
        if (!File.Exists(request) || EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (EditorSceneManager.GetActiveScene().isDirty)
        {
            Debug.Log("Ball Obstacle: escena lista en Assets/Scenes/BallObstacle.unity; la escena actual tiene cambios sin guardar.");
            return;
        }
        File.Delete(request);
        EditorSceneManager.OpenScene("Assets/Scenes/BallObstacle.unity");
        if (SceneView.lastActiveSceneView != null)
            SceneView.lastActiveSceneView.LookAt(new Vector3(0, 0, -1), Quaternion.Euler(50, 0, 0), 15);
        Debug.Log("Ball Obstacle: escena abierta.");
    }
}
