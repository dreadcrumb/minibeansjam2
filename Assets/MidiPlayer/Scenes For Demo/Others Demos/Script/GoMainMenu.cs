using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.MidiPlayer.Scenes_For_Demo.Others_Demos.Script
{
    public class GoMainMenu : MonoBehaviour
    {
        static string sceneMainMenu = "ScenesDemonstration";
       static public void Go()
        {
            SceneManager.LoadScene(sceneMainMenu, LoadSceneMode.Single);
        }
        public void GoToMainMenu()
        {
            SceneManager.LoadScene(sceneMainMenu, LoadSceneMode.Single);
        }
    }
}