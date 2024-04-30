using UnityEngine;

[ExecuteAlways]
public class EditModeUpdater : MonoBehaviour
{
#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        if (Application.isPlaying) return;
        UnityEditor.EditorApplication.QueuePlayerLoopUpdate();
        UnityEditor.SceneView.RepaintAll();
    }
#endif
}