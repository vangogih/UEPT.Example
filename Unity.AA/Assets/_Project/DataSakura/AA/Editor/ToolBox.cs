using UnityEditor;
using UnityEditor.SceneManagement;

namespace DataSakura.Editor
{
    public class ToolBox
    {
        [MenuItem("DataSakura/Scenes/Bootstrap &1", priority = 202)]
        public static void OpenBootstrapScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/0.Bootstrap.unity");
        }

        [MenuItem("DataSakura/Scenes/Battle &2", priority = 202)]
        public static void OpenCoreScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/_Project/Scenes/1.Battle.unity");
        }
    }
}